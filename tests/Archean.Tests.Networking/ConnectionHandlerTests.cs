using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Networking.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Archean.Tests.Networking;

public class ConnectionHandlerTests
{
    private readonly ILogger<ConnectionHandler> _loggerSub;
    private readonly IConnection _connectionSub;
    private readonly IPlayerRegistry _playerRegistrySub;
    private readonly IOptions<ServerSettings> _serverSettingsSub;
    private readonly IWorldRegistry _worldRegistrySub;
    private readonly ConnectionHandler _connectionHandler;

    public ConnectionHandlerTests()
    {
        _loggerSub = Substitute.For<ILogger<ConnectionHandler>>();
        _connectionSub = Substitute.For<IConnection>();
        _playerRegistrySub = Substitute.For<IPlayerRegistry>();
        _worldRegistrySub = Substitute.For<IWorldRegistry>();
        _serverSettingsSub = Substitute.For<IOptions<ServerSettings>>();
        _serverSettingsSub.Value.Returns(new ServerSettings
        {
            Backlog = default,
            MaxPlayers = default,
            Motd = "server motd",
            Name = "server name",
            Port = 25565,
            ServerFullMessage = string.Empty,
            WorldLoadingMotd = string.Empty
        });

        _connectionHandler = new ConnectionHandler(
            playerRegistry: _playerRegistrySub,
            logger: _loggerSub,
            serviceScopeFactory: null!,
            worldRegistry: _worldRegistrySub,
            globalEventBus: null!,
            serverSettingsOptions: _serverSettingsSub);
    }

    [Fact]
    public async Task HandleNewConnectionAsync_ExpectedInitialPackets_ConnectionSuccesful()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(new ClientIdentificationPacket
        {
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            Username = "username",
            VerificationKey = string.Empty,
        }));

        _playerRegistrySub.TryAdd(Arg.Any<IPlayer>()).Returns(true);

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.DidNotReceive().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_NoInitialPackets_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets());

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_UnexpectedInitialPacket_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(new ClientMessagePacket
        {
            Message = string.Empty
        }));

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_UnexpectedInitialPackets_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(
            new ClientIdentificationPacket
            {
                ProtocolVersion = Constants.Networking.ProtocolVersion,
                Username = "username",
                VerificationKey = string.Empty,
            },
            new ClientMessagePacket
            {
                Message = string.Empty
            }
        ));

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_IncorrectProtocolVersion_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(new ClientIdentificationPacket
        {
            ProtocolVersion = Constants.Networking.ProtocolVersion + 1,
            Username = "username",
            VerificationKey = string.Empty,
        }));

        _playerRegistrySub.TryAdd(Arg.Any<IPlayer>()).Returns(true);

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_EmptyUsername_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(new ClientIdentificationPacket
        {
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            Username = string.Empty,
            VerificationKey = string.Empty,
        }));

        _playerRegistrySub.TryAdd(Arg.Any<IPlayer>()).Returns(true);

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    [Fact]
    public async Task HandleNewConnectionAsync_NoDefaultWorld_ConnectionDisconnected()
    {
        // Arrange
        _connectionSub.ReadAsync(TestContext.Current.CancellationToken).Returns(Packets(new ClientIdentificationPacket
        {
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            Username = "username",
            VerificationKey = string.Empty,
        }));

        _playerRegistrySub.TryAdd(Arg.Any<IPlayer>()).Returns(true);
        _worldRegistrySub.GetDefaultWorld().Returns((IWorld?)null);

        // Act
        await _connectionHandler.HandleNewConnectionAsync(_connectionSub, TestContext.Current.CancellationToken);

        // Assertion
        await _connectionSub.Received().DisconnectAsync();
    }

    /// <summary>
    /// Wraps <paramref name="packets"/> in an <see cref="IAsyncEnumerable{IClientPacket}"/>.
    /// </summary>
    /// <param name="packets"></param>
    /// <returns></returns>
    private static async IAsyncEnumerable<IClientPacket> Packets(params IEnumerable<IClientPacket> packets)
    {
        await Task.Yield();

        foreach (IClientPacket packet in packets)
        {
            yield return packet;
        }
    }
}
