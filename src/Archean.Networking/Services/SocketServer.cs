using System.Net;

namespace Archean.Networking.Services;

public class SocketServer : ISocketServer
{
    private readonly IConnectionHandler _connectionHandler;
    private readonly ILogger<SocketServer> _logger;
    private readonly ServerSettings _serverSettings;

    private readonly CancellationTokenSource _serverRunningCancellationTokenSource;
    private readonly Socket _serverSocket;
    private readonly Thread _socketThread;

    private readonly int _port;
    private readonly int _listenBacklogSize;

    public bool Running { get; private set; }

    public CancellationToken ServerRunningCancellationToken => _serverRunningCancellationTokenSource.Token;

    public SocketServer(
        IConnectionHandler connectionHandler,
        ILogger<SocketServer> logger,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        _connectionHandler = connectionHandler;
        _logger = logger;
        _serverSettings = serverSettingsOptions.Value;

        _port = serverSettingsOptions.Value.Port;
        _listenBacklogSize = serverSettingsOptions.Value.Backlog;

        _serverRunningCancellationTokenSource = new CancellationTokenSource();

        _serverSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
        _socketThread = new Thread(async () => await AcceptClientAsync());
    }

    public Task StartAsync()
    {
        if (Running)
        {
            throw new InvalidOperationException("The socket server is already running");
        }

        Running = true;
        _serverSocket.Listen(_listenBacklogSize);
        _socketThread.Start();

        _logger.LogInformation("Starting socket server on port {port}, with {maxPlayers} player slots",
            _port,
            _serverSettings.MaxPlayers);

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!Running)
        {
            return;
        }

        _logger.LogInformation("Stopping socket server on port {port}", _port);

        Running = false;
        await _serverRunningCancellationTokenSource.CancelAsync();

        _serverSocket.Close();
    }

    private async Task AcceptClientAsync()
    {
        try
        {
            while (Running && !_serverRunningCancellationTokenSource.IsCancellationRequested)
            {
                Socket clientSocket = await _serverSocket.AcceptAsync(ServerRunningCancellationToken);

                Connection connection = new Connection(
                    Guid.NewGuid(),
                    clientSocket);

                await _connectionHandler.HandleNewConnectionAsync(connection, ServerRunningCancellationToken);
            }
        }
        catch (OperationCanceledException) // Catch and suppress the OperationCanceledException that may be thrown when the server is stopped.
        {
        }
    }
}
