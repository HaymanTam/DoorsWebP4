using DoorsWeb.API.Authorization;
using DoorsWeb.API.Reports;
using DoorsWeb.API.Services;
using DoorsWeb.API.Services.DoorState;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.Auth;
using DoorsWeb.Shared.Enums;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Structured logging to rolling files — the persisted record a bug-report support bundle draws on.
// Files land on the same mounted volume as the rest of the app's data (Storage:LogDirectory, else a
// "Logs" folder beside the settings volume) so they survive container restarts on offline sites.
// Every line carries the per-request {CorrelationId} (pushed by CorrelationIdMiddleware via
// LogContext), which is what ties a user-quoted reference back to the exact request.
var logDirectory = builder.Configuration["Storage:LogDirectory"];
if (string.IsNullOrWhiteSpace(logDirectory))
    logDirectory = Path.Combine(SystemSettingsService.ResolveDirectory(builder.Configuration), "Logs");
Directory.CreateDirectory(logDirectory);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    // Keep the support-bundle logs focused on app events: framework/EF chatter (e.g. every SQL
    // command logged at Information) is dropped to Warning so real problems stand out.
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(logDirectory, "doorsweb-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        shared: true,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] " +
            "{SourceContext}{NewLine}    {Message:lj}{NewLine}{Exception}"));

// QuestPDF (report PDF export) requires a license type to be declared before any PDF is generated.
// NOTE: the Community license is free only for organisations/individuals under USD $1M annual
// revenue (and small open-source projects). If this product is sold by a company above that
// threshold, a paid QuestPDF Professional/Enterprise license is REQUIRED — revisit before shipping.
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddDbContext<DoorsEnterpriseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    )
);

var jwtSection = builder.Configuration.GetSection("Jwt");
// Signing secret is supplied per environment and never committed: dev via
// user-secrets, deployed environments via the Jwt__SecretKey env var. Fail fast
// rather than silently signing tokens with a weak/empty key.
var jwtSecret = jwtSection["SecretKey"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
    throw new InvalidOperationException(
        "Jwt:SecretKey is missing or shorter than 32 bytes. Set it via user-secrets " +
        "(dev) or the Jwt__SecretKey environment variable (deployed environments).");
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        // Browsers can't set the Authorization header on a WebSocket handshake, so SignalR
        // passes the JWT on the query string. Accept it for the live-events hub.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/eventHub"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });
// Per-area authorization: one Read (≥ Read) and one Write (≥ ReadWrite) policy per area.
// A Super bypasses every requirement (see AreaAccessHandler).
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AreaPolicies.CardManagerRead,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.CardManager, AreaAccess.Read)));
    options.AddPolicy(AreaPolicies.CardManagerWrite,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.CardManager, AreaAccess.ReadWrite)));

    options.AddPolicy(AreaPolicies.SiteSettingsRead,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.SiteSettings, AreaAccess.Read)));
    options.AddPolicy(AreaPolicies.SiteSettingsWrite,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.SiteSettings, AreaAccess.ReadWrite)));

    options.AddPolicy(AreaPolicies.UserSettingsRead,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.UserSettings, AreaAccess.Read)));
    options.AddPolicy(AreaPolicies.UserSettingsWrite,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.UserSettings, AreaAccess.ReadWrite)));

    options.AddPolicy(AreaPolicies.ReportsRead,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.Reports, AreaAccess.Read)));
    options.AddPolicy(AreaPolicies.ReportsWrite,
        p => p.Requirements.Add(new AreaAccessRequirement(PermissionClaims.Reports, AreaAccess.ReadWrite)));
});
builder.Services.AddSingleton<IAuthorizationHandler, AreaAccessHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCorsPolicy",
        policy =>
        {
            policy.WithOrigins("https://localhost:60880")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required for SignalR
        });
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddMapster();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

// Authentication against legacy T_Users
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPwHashService, PwHashService>();

// Password policy: flat 12-char minimum + offline screen against a bundled list of ~1M
// known-breached/common passwords. Singleton so the list is loaded into memory just once.
builder.Services.AddSingleton<IPasswordPolicyService, PasswordPolicyService>();

// Users & Passwords page CRUD (legacy T_Users)
builder.Services.AddScoped<IUsersService, UsersService>();

// Alarms (read-only list for the Alarms page)
builder.Services.AddScoped<IAlarmService, AlarmService>();

// Change-audit log: viewer list + per-service write. Needs the HTTP context to capture
// the acting user and client IP.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();

// Events (read-only list for the Events page)
builder.Services.AddScoped<IEventService, EventService>();

// Doors (list + editor, mapped to T_Doors)
builder.Services.AddScoped<IDoorService, DoorService>();

// CRUD services (legacy DoorsEnterprise entities)
builder.Services.AddScoped<IAccessLevelService, AccessLevelService>();
builder.Services.AddScoped<IApbZoneService, ApbZoneService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<ICardManagerService, CardManagerService>();
builder.Services.AddScoped<ICardPackService, CardPackService>();
builder.Services.AddScoped<IIoControllerService, IoControllerService>();
builder.Services.AddScoped<ICardholderService, CardholderService>();
builder.Services.AddScoped<ISpaceZoneService, SpaceZoneService>();
builder.Services.AddScoped<ITimeSheetService, TimeSheetService>();
builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();
builder.Services.AddScoped<ITriggersService, TriggersService>();

// Database backup / restore
builder.Services.AddScoped<IBackupService, BackupService>();

// Legacy DoorsClient backup restore (password-protected ZIP of .sql/.rs table dumps)
builder.Services.AddScoped<ILegacyBackupService, LegacyBackupService>();

// System Settings dialog: sites (legacy T_Sites).
builder.Services.AddScoped<ISiteService, SiteService>();

// System Settings dialog: global settings (controller communication) persisted as a JSON
// file on the settings volume. Singleton: the file is the single source of truth.
var settingsDirectory = SystemSettingsService.ResolveDirectory(builder.Configuration);
builder.Services.AddSingleton<ISystemSettingsService>(new SystemSettingsService(settingsDirectory));

// User photo storage. Resolved once so DI and the static-file provider agree on the path.
var userPhotoDirectory = PhotoStorageService.ResolveDirectory(builder.Configuration);
builder.Services.AddSingleton<IPhotoStorageService>(new PhotoStorageService(userPhotoDirectory));

// Cardholder photo storage (keyed by card number; no DB column). Resolved once so DI and the
// static-file provider below point at the same directory.
var cardPhotoDirectory = CardPhotoService.ResolveDirectory(builder.Configuration);
builder.Services.AddSingleton<ICardPhotoService>(new CardPhotoService(cardPhotoDirectory));

// Controller UDP protocol: one singleton is both the background listener and the send handle.
builder.Services.AddSingleton<UdpProtocolService>();
builder.Services.AddSingleton<IUdpProtocolService>(sp => sp.GetRequiredService<UdpProtocolService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<UdpProtocolService>());

// Live door state for the floorplan: a singleton cache fed by the UDP listener and broadcast over
// the EventHub, and a hosted service (offline sweep + periodic door-map refresh).
builder.Services.AddSingleton<DoorStateService>();
builder.Services.AddSingleton<IDoorStateService>(sp => sp.GetRequiredService<DoorStateService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<DoorStateService>());

// Active polling: pings every controller (B,1) on the configured interval so the live state above
// stays current even when no events are happening. Replies (B,2) flow back through the UDP listener.
builder.Services.AddHostedService<ControllerPollingService>();

// Event-log drain: when a ping reply reports unread entries, pull them from the controller (D,1/
// D,3/D,2 handshake), record them to T_Events and push each to clients as "NewEvent". Singleton so
// it owns its UDP subscription and per-door drain de-duplication for the app's lifetime.
builder.Services.AddSingleton<IEventLogService, EventLogService>();

// Door control (unlock / lock / momentary / lockdown) — builds and sends protocol commands.
builder.Services.AddScoped<IDoorCommandService, DoorCommandService>();

// Floorplan layout (optional uploaded plan image + door placements), persisted per-site as JSON
// on the settings volume. Singleton: the files are the single source of truth. Constructed up
// front so the static-file provider below can serve its image directory.
var floorPlanService = new FloorPlanService(SystemSettingsService.ResolveDirectory(builder.Configuration));
builder.Services.AddSingleton<IFloorPlanService>(floorPlanService);

// Licensing: validates the signed key (in system settings) against the configured public key and
// exposes the door/card/expiry limits. Singleton so verification is cached and not re-run per
// request. The matching read-only middleware (below) blocks writes once a valid license expires.
builder.Services.AddSingleton<LicenseService>();
builder.Services.AddSingleton<ILicenseService>(sp => sp.GetRequiredService<LicenseService>());

// Reports engine: each IReport is one configuration of the single engine; the registry indexes them
// by key for the controller (list/run/export) and the client hub. Scoped because reports read through
// the (scoped) DbContext. Register new reports here and they appear in the hub automatically.
builder.Services.AddScoped<IReport, AccessHistoryReport>();
builder.Services.AddScoped<IReportRegistry, ReportRegistry>();

var app = builder.Build();

// Apply EF migrations on startup so a fresh PostgreSQL volume gets the full schema
// automatically (the legacy restore loads into these tables, so they must exist first).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DoorsEnterpriseContext>();
    db.Database.Migrate();
}

app.MapHub<EventHub>("/eventHub");
app.MapHub<DoorsWeb.API.Services.BackupHub>("/backupHub");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Serve uploaded user photos as static files at /media/user-photo (public, GUID file names).
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(userPhotoDirectory),
    RequestPath = "/media/user-photo"
});

// Serve uploaded cardholder photos as static files at /media/card-photo (named by card number).
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(cardPhotoDirectory),
    RequestPath = CardPhotoService.WebPath
});

// Serve uploaded floorplan background images at /media/floorplan (public, server-named files).
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(floorPlanService.ImageDirectory),
    RequestPath = FloorPlanService.ImageWebPath
});

// Stamp a correlation ID on every request (response header + log scope + ProblemDetails requestId),
// then emit one structured completion line per request. Placed after static files so media requests
// don't add log noise, but before auth so even rejected requests are traced.
app.UseMiddleware<DoorsWeb.API.Middleware.CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseCors("BlazorCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<DoorsWeb.API.Middleware.MustChangePasswordMiddleware>();
// Blocks all writes once a valid license expires (exempts /auth and /api/SystemSettings so the key
// can be renewed). No-op while unlicensed or licensed-and-active.
app.UseMiddleware<DoorsWeb.API.Middleware.LicenseReadOnlyMiddleware>();
app.MapControllers().RequireAuthorization();
app.Run();
