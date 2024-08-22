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

    public async Task<ReadOnlyMemory<byte>> ReadAsync()
    {
        Memory<byte> data = new byte[socket.Available];
        await socket.ReceiveAsync(data);
        return data;
    }

    public async Task SendAsync(ReadOnlyMemory<byte> data)
    {
        if (IsConnected)
        {
            await socket.SendAsync(data);
        }
    }

    public void Dispose()
    {
        socket.Dispose();
    }
}
