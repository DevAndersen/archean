﻿using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Archean.Networking.Services;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IPlayerService _playerService;
    private readonly ILogger<ConnectionHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IWorldRegistry _worldRegistry;
    private readonly IGlobalEventBus _globalEventBus;
    private readonly IOptions<ServerSettings> _serverSettings;

    public ConnectionHandler(
        IPlayerService playerService,
        ILogger<ConnectionHandler> logger,
        IServiceScopeFactory serviceScopeFactory,
        IWorldRegistry worldRegistry,
        IGlobalEventBus globalEventBus,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        _playerService = playerService;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _worldRegistry = worldRegistry;
        _globalEventBus = globalEventBus;
        _serverSettings = serverSettingsOptions;
    }

    public async Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken)
    {
        IEnumerable<IClientPacket> initialConnectionPackets = connection.ReadAsync(cancellationToken)
            .ToBlockingEnumerable(cancellationToken);

        if (initialConnectionPackets.ToArray() is not [ClientIdentificationPacket clientIdentificationPacket])
        {
            _logger.LogError("Unexpected initial packets received from connection {connectionId}",
                connection.Id);

            await connection.DisconnectAsync();
            return;
        }

        // Validate client protocol ID.
        if (clientIdentificationPacket.ProtocolVersion != Constants.Networking.ProtocolVersion)
        {
            _logger.LogError("Connection {connectionId} specified unexpected protocol version {protocolVersion}",
                connection.Id,
                clientIdentificationPacket.ProtocolVersion);

            await connection.SendAsync(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client protocol version {clientIdentificationPacket.ProtocolVersion}"
            });

            await connection.DisconnectAsync();
            return;
        }

        // Validate username.
        if (string.IsNullOrWhiteSpace(clientIdentificationPacket.Username))
        {
            await connection.DisconnectAsync();
            return;
        }

        Player player = new Player(
            connection,
            clientIdentificationPacket.Username,
            _globalEventBus);

        if (_playerService.TryAdd(player))
        {
            _logger.LogInformation("Player {username} assigned ID {playerId}",
                player.Username,
                player.Id);

            await SendJoinServerTestAsync(player);

            // Todo: Better handling of tasks than fire-and-forget
            _ = Task.Run(async () => await ReceiveFromClientAsync(player, cancellationToken), cancellationToken);
        }
        else
        {
            // Todo: Differentiate between failure to add due to full server, or already being logged in.
            _logger.LogInformation("Unable to register player {username}, server is full",
                player.Username);

            await connection.DisconnectAsync(_serverSettings.Value.ServerFullMessage);
        }
    }

    private async Task ReceiveFromClientAsync(IPlayer player, CancellationToken cancellationToken)
    {
        IConnection connection = player.Connection;

        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        // Set the player of the current scope.
        ISessionService sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
        sessionService.SetPlayer(player);

        // Register event subscriptions.
        IClientEventHandler eventHandler = scope.ServiceProvider.GetRequiredService<IClientEventHandler>();
        eventHandler.RegisterEventSubscriptions();

        IClientPacketHandler packetHandler = scope.ServiceProvider.GetRequiredService<IClientPacketHandler>();

        try
        {
            while (!cancellationToken.IsCancellationRequested && connection.IsConnected)
            {
                IAsyncEnumerable<IClientPacket> packets = connection.ReadAsync(cancellationToken);

                await foreach (IClientPacket packet in packets)
                {
                    switch (packet)
                    {
                        // Client identification packet.
                        case ClientIdentificationPacket clientIdentificationPacket:
                            // Todo: Handle unexpected client identification packet.
                            break;

                        // Block update packet.
                        case ClientSetBlockPacket setBlockPacket:
                            await packetHandler.HandleSetBlockPacketAsync(setBlockPacket);
                            break;

                        // Pose packet.
                        case ClientPositionAndOrientationPacket positionAndOrientationPacket:
                            await packetHandler.HandlePositionAndOrientationPacketAsync(positionAndOrientationPacket);
                            break;

                        // Message packet.
                        case ClientMessagePacket messagePacket:
                            await packetHandler.HandleMessagePacketAsync(messagePacket);
                            break;

                        // Unknown packet type.
                        default:
                            // Todo: Handle unknown packet.
                            _logger.LogWarning("Unexpected packet {packetType} receives from {connectionId}, ", // Todo: Finish error message
                                packet.GetType().FullName,
                                connection.Id);
                            break;
                    }
                }
            }
        }
        catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionReset)
        {
            _logger.LogInformation("Connection {connectionId} disconnected",
                connection.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected {exceptionType} thrown while receiving data from connection {connectionId}",
                e.GetType().FullName,
                connection.Id);
        }
        finally
        {
            _playerService.Remove(connection);

            await _globalEventBus.InvokeEventAsync(new PlayerDisconnectEvent
            {
                Player = player
            });

            await connection.DisconnectAsync();
        }
    }

    private async Task SendJoinServerTestAsync(IPlayer player)
    {
        await player.Connection.SendAsync(new ServerIdentificationPacket
        {
            PlayerType = PlayerType.Normal,
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            ServerMotd = _serverSettings.Value.Motd,
            ServerName = _serverSettings.Value.Name,
        });

        IWorld? world = _worldRegistry.GetDefaultWorld();
        if (world == null)
        {
            await player.Connection.DisconnectAsync(); // Todo

            _logger.LogError("Failed to join player {playerName}, default world was not found",
                player.DisplayName);

            return;
        }
        await world.JoinAsync(player);
    }
}
