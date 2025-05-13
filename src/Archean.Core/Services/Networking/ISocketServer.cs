namespace Archean.Core.Services.Networking;

public interface ISocketServer
{
    bool Running { get; }

    CancellationToken ServerRunningCancellationToken { get; }

    Task StartAsync();

    Task StopAsync();
}
