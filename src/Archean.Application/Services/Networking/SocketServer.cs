﻿using Archean.Application.Models.Networking;
using System.Net;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class SocketServer : ISocketServer
{
    private readonly IConnectionHandler connectionHandler;
    private readonly ILogger<SocketServer> logger;
    private readonly IServerPacketWriter serverPacketWriter;
    private readonly ServerSettings serverSettings;

    private readonly CancellationTokenSource serverRunningCancellationTokenSource;
    private readonly Socket serverSocket;
    private readonly Thread socketThread;

    private readonly int port;
    private readonly int listenBacklogSize;

    public bool Running { get; private set; }

    public CancellationToken ServerRunningCancellationToken => serverRunningCancellationTokenSource.Token;

    public SocketServer(
        IConnectionHandler connectionHandler,
        ILogger<SocketServer> logger,
        IServerPacketWriter serverPacketWriter,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        this.connectionHandler = connectionHandler;
        this.logger = logger;
        this.serverPacketWriter = serverPacketWriter;
        serverSettings = serverSettingsOptions.Value;

        port = serverSettingsOptions.Value.Port;
        listenBacklogSize = serverSettingsOptions.Value.Backlog;

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

        Running = true;
        serverSocket.Listen(listenBacklogSize);
        socketThread.Start();

        logger.LogInformation("Starting socket server on port {port}, with {maxPlayers} player slots",
            port,
            serverSettings.MaxPlayers);

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!Running)
        {
            return;
        }

        logger.LogInformation("Stopping socket server on port {port}", port);

        Running = false;
        await serverRunningCancellationTokenSource.CancelAsync();

        serverSocket.Close();
    }

    private async Task AcceptClientAsync()
    {
        try
        {
            while (Running && !serverRunningCancellationTokenSource.IsCancellationRequested)
            {
                Socket clientSocket = await serverSocket.AcceptAsync(ServerRunningCancellationToken);

                Connection connection = new Connection(
                    Guid.NewGuid(),
                    clientSocket,
                    serverPacketWriter);

                await connectionHandler.HandleNewConnectionAsync(connection, ServerRunningCancellationToken);
            }
        }
        catch (OperationCanceledException) // Catch and suppress the OperationCanceledException that may be thrown when the server is stopped.
        {
        }
    }
}
