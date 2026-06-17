using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DoorsEnterpriseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    )
);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));
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
    });
builder.Services.AddAuthorization();

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

// Alarms (read-only list for the Alarms page)
builder.Services.AddScoped<IAlarmService, AlarmService>();

// Header CRUD services (legacy DoorsEnterprise entities)
builder.Services.AddScoped<IAccessLevelHeaderService, AccessLevelHeaderService>();
builder.Services.AddScoped<IApbzoneHeaderService, ApbzoneHeaderService>();
builder.Services.AddScoped<ICalendarHeaderService, CalendarHeaderService>();
builder.Services.AddScoped<ICardDesignHeaderService, CardDesignHeaderService>();
builder.Services.AddScoped<ICardManagerHeaderService, CardManagerHeaderService>();
builder.Services.AddScoped<ICardPackHeaderService, CardPackHeaderService>();
builder.Services.AddScoped<IIocontrollerHeaderService, IocontrollerHeaderService>();
builder.Services.AddScoped<INameHeaderService, NameHeaderService>();
builder.Services.AddScoped<ISpaceZoneHeaderService, SpaceZoneHeaderService>();
builder.Services.AddScoped<ITimeSheetHeaderService, TimeSheetHeaderService>();
builder.Services.AddScoped<ITimeZoneHeaderService, TimeZoneHeaderService>();
builder.Services.AddScoped<ITriggersHeaderService, TriggersHeaderService>();

// Database backup / restore
builder.Services.AddScoped<IBackupService, BackupService>();

// User photo storage. Resolved once so DI and the static-file provider agree on the path.
var userPhotoDirectory = PhotoStorageService.ResolveDirectory(builder.Configuration);
builder.Services.AddSingleton<IPhotoStorageService>(new PhotoStorageService(userPhotoDirectory));

// Controller UDP protocol: one singleton is both the background listener and the send handle.
builder.Services.AddSingleton<UdpProtocolService>();
builder.Services.AddSingleton<IUdpProtocolService>(sp => sp.GetRequiredService<UdpProtocolService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<UdpProtocolService>());

var app = builder.Build();
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

app.UseCors("BlazorCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<DoorsWeb.API.Middleware.MustChangePasswordMiddleware>();
app.MapControllers().RequireAuthorization();
app.Run();
