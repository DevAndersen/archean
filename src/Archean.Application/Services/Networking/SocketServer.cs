using Archean.Application.Models.Networking;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class SocketServer : ISocketServer
{
    private readonly IConnectionHandler connectionHandler;
    private readonly ILogger<SocketServer> logger;

    private readonly CancellationTokenSource serverRunningCancellationTokenSource;
    private readonly Socket serverSocket;
    private readonly Thread socketThread;

    private readonly int port;
    private readonly int listenBacklogSize;

    public bool Running { get; private set; }

    public CancellationToken ServerRunningCancellationToken => serverRunningCancellationTokenSource.Token;

    public SocketServer(IConnectionHandler connectionHandler, ILogger<SocketServer> logger)
    {
        this.connectionHandler = connectionHandler;
        this.logger = logger;

        port = 25565; // Todo
        listenBacklogSize = 3; // Todo

        serverRunningCancellationTokenSource = new CancellationTokenSource();

        serverSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        socketThread = new Thread(async () => await AcceptClientAsync());
    }

    public Task StartAsync()
    {
        if (Running)
        {
            throw new InvalidOperationException("The socket server is already running");
        }

        serverSocket.Listen(listenBacklogSize);
        socketThread.Start();
        Running = true;

        logger.LogInformation("Socket server started on port {port} with backlog size {listenBacklogSize}",
            port,
            listenBacklogSize);

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!Running)
        {
            return;
        }

        logger.LogInformation("Socket server running on port {port} has been stopped", port);

        Running = false;
        serverRunningCancellationTokenSource.Cancel();

        serverSocket.Close();
    }

    private async Task AcceptClientAsync()
    {
        try
        {
            while (Running && !serverRunningCancellationTokenSource.IsCancellationRequested)
            {
                Socket clientSocket = await serverSocket.AcceptAsync(serverRunningCancellationTokenSource.Token);
                Connection connection = new Connection(clientSocket);
                await connectionHandler.HandleNewConnectionAsync(connection);
            }
        }
        catch (OperationCanceledException) // Catch and suppress the OperationCanceledException that may be thrown when the server is stopped.
        {
        }
    }
}
