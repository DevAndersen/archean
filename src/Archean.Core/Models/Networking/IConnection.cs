using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Core.Models.Networking;

public interface IConnection : IDisposable
{
    public Guid Id { get; }

    public bool IsConnected { get; }

    public Task SendAsync(ReadOnlyMemory<byte> data);

    public Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Closes the connection.
    /// </summary>
    /// <returns></returns>
    public Task DisconnectAsync();

    /// <summary>
    /// Sends a <see cref="ServerDisconnectPlayerPacket"/> containing <paramref name="message"/>, then closes the connection.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task DisconnectAsync(string message);
}
