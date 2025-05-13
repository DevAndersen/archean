using System.Net.Sockets;

namespace Archean.Application.Models.Networking;

public class Connection : IConnection
{
    private readonly Socket _socket;
    private readonly IServerPacketWriter _serverPacketWriter;

    public Guid Id { get; }

    public bool IsConnected => _socket.Connected;

    public Connection(Guid id, Socket socket, IServerPacketWriter serverPacketWriter)
    {
        Id = id;
        _socket = socket;
        _serverPacketWriter = serverPacketWriter;
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
        foreach (IServerPacket packet in packets)
        {
            if (!IsConnected)
            {
                return;
            }

            ReadOnlyMemory<byte> bytes = _serverPacketWriter.WritePacket(packet);
            await _socket.SendAsync(bytes);
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
