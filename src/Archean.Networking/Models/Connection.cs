using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Networking.Helpers;
using System.Buffers;
using System.Net.Sockets;

namespace Archean.Networking.Models;

public class Connection : IConnection
{
    private readonly Socket _socket;

    public Guid Id { get; }

    public bool IsConnected => _socket.Connected;

    public Connection(Guid id, Socket socket)
    {
        Id = id;
        _socket = socket;
    }

    public async Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken)
    {
        int readAttempts = 0;
        while (_socket.Available == 0 && readAttempts++ < 5)
        {
            await Task.Delay(10, cancellationToken);
        }

        Memory<byte> data = new byte[_socket.Available];
        await _socket.ReceiveAsync(data, cancellationToken);
        return data;
    }

    public async Task SendAsync(params IEnumerable<IServerPacket> packets)
    {
        int bufferSize = ServerPacketSizer.CalculateSize(packets);
        byte[] array = ArrayPool<byte>.Shared.Rent(bufferSize);

        Memory<byte> memory = array.AsMemory()[..bufferSize];
        ServerPacketSerializer.WritePackets(packets, memory.Span);
        await _socket.SendAsync(memory);

        ArrayPool<byte>.Shared.Return(array);
    }

    public Task DisconnectAsync()
    {
        _socket.Close();
        return Task.CompletedTask;
    }

    public async Task DisconnectAsync(string message)
    {
        await SendAsync(new ServerDisconnectPlayerPacket
        {
            Message = message
        });

        _socket.Close();
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}
