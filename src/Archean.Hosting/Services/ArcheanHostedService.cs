using Archean.Core.Services;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archean.Hosting.Services;

public class ArcheanHostedService : IHostedService
{
    private readonly ISocketServer _socketServer;
    private readonly IEnumerable<IStartupService> _startupServices;
    private readonly IEnumerable<IShutdownService> _shutdownServices;
    private readonly ILogger<ArcheanHostedService> _logger;

    public ArcheanHostedService(
        ISocketServer socketServer,
        IEnumerable<IStartupService> startupServices,
        IEnumerable<IShutdownService> shutdownServices,
        ILogger<ArcheanHostedService> logger)
    {
        _socketServer = socketServer;
        _startupServices = startupServices;
        _shutdownServices = shutdownServices;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (IStartupService startupService in _startupServices)
        {
            await startupService.OnStartupAsync();
        }

        await _socketServer.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _socketServer.StopAsync();

        foreach (IShutdownService shutdownService in _shutdownServices)
        {
            try
            {
                await shutdownService.OnShutdownAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred during shutdown");
            }
        }
    }
}
