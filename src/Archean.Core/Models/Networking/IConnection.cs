namespace Archean.Core.Models.Networking;

public interface IConnection : IDisposable
{
    public Guid Id { get; }

    public bool IsConnected { get; }

    public Task SendAsync(ReadOnlyMemory<byte> data);

    public Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken);

    public Task DisconnectAsync();
}
