using DoorsWeb.API.Data;
using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    )
);

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
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDoorService, DoorService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccessLevelService, AccessLevelService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<PwHashService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditUsers", policy =>
        policy.RequireClaim("CanEditUsers", "yes"));
    options.AddPolicy("CanEditDoors", policy =>
        policy.RequireClaim("CanEditDoors", "yes"));
    options.AddPolicy("CanEditAdmins", policy =>
        policy.RequireClaim("CanEditAdmins", "yes"));
});
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();
app.MapHub<EventHub>("/eventHub");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<DataContext>() ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors("BlazorCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
