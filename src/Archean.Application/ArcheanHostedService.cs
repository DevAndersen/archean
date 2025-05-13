using Archean.Application.Services;
using Microsoft.Extensions.Hosting;

namespace Archean.Application;

public class ArcheanHostedService : IHostedService
{
    private readonly ISocketServer _socketServer;
    private readonly ServerStartup _serverStartup;

    public ArcheanHostedService(ISocketServer socketServer, ServerStartup serverStartup)
    {
        _socketServer = socketServer;
        _serverStartup = serverStartup;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serverStartup.PerformSetup();
        await _socketServer.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _socketServer.StopAsync();
    }
}
