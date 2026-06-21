using DoorsWeb.Client;
using DoorsWeb.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TabBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<JwtAuthorizationHandler>();
builder.Services.AddHttpClient("WebAPI", client =>
    client.BaseAddress = new Uri("https://localhost:60879"))
    .AddHttpMessageHandler<JwtAuthorizationHandler>();
// Plain client without the auth handler, used by the handler itself to call
// auth/refresh so a refresh 401 can't recurse back into a refresh attempt.
builder.Services.AddHttpClient("WebAPI-Refresh", client =>
    client.BaseAddress = new Uri("https://localhost:60879"));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTabler();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());

// Live feeds over SignalR /eventHub: door-state for the floorplan, new events for the events page.
builder.Services.AddScoped<DoorsWeb.Client.Services.DoorStateHubClient>();
builder.Services.AddScoped<DoorsWeb.Client.Services.EventHubClient>();

await builder.Build().RunAsync();
