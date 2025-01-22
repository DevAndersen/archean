using Archean.Application.Services;
using Microsoft.Extensions.Hosting;

namespace Archean.Application;

public class ArcheanHostedService : IHostedService
{
    private readonly ISocketServer socketServer;
    private readonly ServerStartup serverStartup;

    public ArcheanHostedService(ISocketServer socketServer, ServerStartup serverStartup)
    {
        this.socketServer = socketServer;
        this.serverStartup = serverStartup;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        serverStartup.PerformSetup();
        await socketServer.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await socketServer.StopAsync();
    }
}
