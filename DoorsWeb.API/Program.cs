using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Mapster;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DoorsEnterpriseContext>(options =>
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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

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

var app = builder.Build();
app.MapHub<EventHub>("/eventHub");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("BlazorCorsPolicy");
app.MapControllers();
app.Run();
