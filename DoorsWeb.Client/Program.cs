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
// Same-origin: the app is served by nginx, which reverse-proxies /api, /auth, /eventHub,
// /backupHub and /media to the API container. Calling through our own origin means one
// browser-trusted certificate (nginx's) and no CORS — no hardcoded API host to break when
// the app is reached from another machine.
builder.Services.AddHttpClient("WebAPI", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<JwtAuthorizationHandler>();
// Plain client without the auth handler, used by the handler itself to call
// auth/refresh so a refresh 401 can't recurse back into a refresh attempt.
builder.Services.AddHttpClient("WebAPI-Refresh", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
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

// App-wide live door status (its own hub connection) feeding the nav problem badge and the Door
// Manager's Activity column. Scoped == one instance for the app's lifetime in standalone WASM.
builder.Services.AddScoped<DoorsWeb.Client.Services.DoorStatusStore>();

await builder.Build().RunAsync();
