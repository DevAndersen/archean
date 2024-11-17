using Archean.Core.Models.Networking;
using System.Net.Sockets;

namespace Archean.Application.Models.Networking;

public class Connection : IConnection
{
    private readonly Socket socket;

    public bool IsConnected => socket.Connected;

    public Connection(Socket socket)
    {
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

    public void Disconnect()
    {
        socket.Close();
    }

    public void Dispose()
    {
        socket.Dispose();
    }
}
