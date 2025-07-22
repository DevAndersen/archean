using Archean.App.WebApp;
using Archean.App.WebApp.Components;
using Archean.Hosting;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureArcheanDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddCookieAuthentication();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/httpstatus/{0}");
app.UseCookieAuthentication();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGroup("api", group =>
{
    group.MapGet("skin/{skin}", ([FromRoute] string skin) =>
    {
        string skinsDirectory = "skins"; // Todo: Get value from configuration.
        string skinPath = Path.Combine(skinsDirectory, skin);

        if (!File.Exists(skinPath))
        {
            return TypedResults.NotFound();
        }

        byte[] bytes = File.ReadAllBytes(skinPath);
        return Results.File(bytes, "image/png");
    });
}).RequireAuthentication();

app.Run();
