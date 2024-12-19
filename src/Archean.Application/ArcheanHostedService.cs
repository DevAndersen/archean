using Archean.Core.Services.Networking;
using Microsoft.Extensions.Hosting;

namespace Archean.Application;

public class ArcheanHostedService : IHostedService
{
    private readonly ISocketServer socketServer;

    public ArcheanHostedService(ISocketServer socketServer)
    {
        this.socketServer = socketServer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await socketServer.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await socketServer.StopAsync();
    }
}
