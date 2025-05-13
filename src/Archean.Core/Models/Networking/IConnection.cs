using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Core.Models.Networking;

public interface IConnection : IDisposable
{
    Guid Id { get; }

    bool IsConnected { get; }

    Task SendAsync(params IEnumerable<IServerPacket> packets);

    Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Closes the connection.
    /// </summary>
    /// <returns></returns>
    Task DisconnectAsync();

    /// <summary>
    /// Sends a <see cref="ServerDisconnectPlayerPacket"/> containing <paramref name="message"/>, then closes the connection.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task DisconnectAsync(string message);
}
