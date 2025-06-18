using System.Buffers;
using System.Runtime.CompilerServices;

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

    public async IAsyncEnumerable<IClientPacket> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int readAttempts = 0;
        while (_socket.Available == 0 && readAttempts++ < 5)
        {
            await Task.Delay(10, cancellationToken);
        }

        int bufferSize = _socket.Available;
        byte[] array = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            Memory<byte> buffer = array.AsMemory()[..bufferSize];
            await _socket.ReceiveAsync(buffer, cancellationToken);

            foreach (IClientPacket packet in ClientPacketDeserializer.ReadPackets(buffer, cancellationToken))
            {
                yield return packet;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }

    public async Task SendAsync(params IEnumerable<IServerPacket> packets)
    {
        int bufferSize = ServerPacketSizer.CalculateSize(packets);
        byte[] array = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            Memory<byte> buffer = array.AsMemory()[..bufferSize];
            ServerPacketSerializer.WritePackets(packets, buffer.Span);
            await _socket.SendAsync(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
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
