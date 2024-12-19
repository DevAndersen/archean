using Archean.Core.Models.Networking;
using System.Net.Sockets;

namespace Archean.Application.Models.Networking;

public class Connection : IConnection
{
    private readonly Socket socket;

    public Guid Id { get; }

    public bool IsConnected => socket.Connected;

    public Connection(Guid id, Socket socket)
    {
        Id = id;
        this.socket = socket;
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

    public async Task SendAsync(ReadOnlyMemory<byte> data)
    {
        if (IsConnected)
        {
            await socket.SendAsync(data);
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
