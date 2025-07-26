using Archean.App.WebApp.Components.Pages;
using Archean.App.WebApp.Services;

namespace Archean.App.WebApp.Settings;

public class WebAppSettings
{
    /// <summary>
    /// The password for the server admin site.
    /// </summary>
    /// <remarks>
    /// If this is set to null, or an empty- or whitespace string, the server admin site will not be accessible.
    /// </remarks>
    public required string SitePassword { get; init; } = string.Empty;

    /// <summary>
    /// The number of log entries kept by <see cref="MemoryLoggerOutput"/>, visible on the <see cref="LogsPage"/>.
    /// </summary>
    public required int MemoryLoggerCapacity { get; init; } = 1000;
}
