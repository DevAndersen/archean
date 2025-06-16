using Archean.Core.Services;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.Hosting;

namespace Archean.Hosting.Services;

public class ArcheanHostedService : IHostedService
{
    private readonly ISocketServer _socketServer;
    private readonly IEnumerable<IStartupService> _startupServices;

    public ArcheanHostedService(ISocketServer socketServer, IEnumerable<IStartupService> startupServices)
    {
        _socketServer = socketServer;
        _startupServices = startupServices;
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
    }
}
