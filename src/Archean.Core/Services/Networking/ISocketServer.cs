namespace Archean.Core.Services.Networking;

public interface ISocketServer
{
    public bool Running { get; }

    public CancellationToken ServerRunningCancellationToken { get; }

    public Task StartAsync();

    public Task StopAsync();
}
