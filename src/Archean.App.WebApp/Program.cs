using Archean.App.WebApp;
using Archean.App.WebApp.Components;
using Archean.App.WebApp.Services;
using Archean.App.WebApp.Settings;
using Archean.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureArcheanDefaults();

// Register memory logger.
int memoryLoggerCapacity = builder.Configuration.GetValue<int?>($"{WebAppConstants.WebAppSettingsSection}:{nameof(WebAppSettings.MemoryLoggerCapacity)}") ?? 1000;
MemoryLoggerOutput memoryLoggerOutput = new MemoryLoggerOutput(memoryLoggerCapacity);
builder.AddLoggerOutput(memoryLoggerOutput);
builder.Services.AddSingleton(memoryLoggerOutput);

builder.Services.AddSingleton<ChatLogService>();
builder.Services.AddScoped<JSInterop>();

builder.AddConfiguration<WebAppSettings>(WebAppConstants.WebAppSettingsSection);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddCookieAuthentication();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Changes current working directory to the directory containing the executable (same behavior as for console projects).
    // This is in order to avoid files created during debugging from showing up in source control,
    // as the output directory is expected to be defined in the .gitignore file.
    Directory.SetCurrentDirectory(AppContext.BaseDirectory);
}
else
{
    // Error handling.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/httpstatus/{0}");
app.UseCookieAuthentication();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Services.GetRequiredService<ChatLogService>().StartListening(); // Todo: Move to IStartupService.

app.MapGroup("api", group =>
{
    group.MapSkinApi();
}).RequireAuthentication();

app.Run();
