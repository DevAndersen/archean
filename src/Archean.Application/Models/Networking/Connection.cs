using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Networking;
using System.Net.Sockets;

namespace Archean.Application.Models.Networking;

public class Connection : IConnection
{
    private readonly Socket socket;
    private readonly IServerPacketWriter serverPacketWriter;

    public Guid Id { get; }

    public bool IsConnected => socket.Connected;

    public Connection(Guid id, Socket socket, IServerPacketWriter serverPacketWriter)
    {
        Id = id;
        this.socket = socket;
        this.serverPacketWriter = serverPacketWriter;
    }

    public async Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken)
    {
        int readAttempts = 0;
        while (socket.Available == 0 && readAttempts++ < 5)
        {
            await Task.Delay(10, cancellationToken);
        }

        Memory<byte> data = new byte[socket.Available];
        await socket.ReceiveAsync(data, cancellationToken);
        return data;
    }

    public async Task SendAsync(params IEnumerable<IServerPacket> packets)
    {
        foreach (IServerPacket packet in packets)
        {
            if (!IsConnected)
            {
                return;
            }

            ReadOnlyMemory<byte> bytes = serverPacketWriter.WritePacket(packet);
            await socket.SendAsync(bytes);
        }
    }

    public Task DisconnectAsync()
    {
        socket.Close();
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(string message)
    {
        // Todo: Send a ServerDisconnectPlayerPacket before closing the connection.
        socket.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        socket.Dispose();
    }
}
