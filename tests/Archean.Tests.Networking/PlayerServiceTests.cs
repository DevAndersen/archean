using Archean.Core.Models;
using Archean.Core.Services.Networking;
using Archean.Core.Settings;
using Archean.Networking.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Archean.Tests.Networking;

public class PlayerServiceTests
{
    private readonly ServerSettings _defaultServerSettings;

    public PlayerServiceTests()
    {
        _defaultServerSettings = Substitute.For<ServerSettings>();
        _defaultServerSettings.MaxPlayers.Returns((byte)Constants.Players.HighestPlayerId);
    }

    [Fact]
    public void TryAdd_SinglePlayer_ExpectedGetAllResult()
    {
        // Arrange
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);

        IPlayerService playerService = new PlayerService(settings);
        IPlayer player = Substitute.For<IPlayer>();

        // Act
        bool wasPlayerAdded = playerService.TryAdd(player);

        // Assert
        Assert.True(wasPlayerAdded);
        Assert.Equal([player], playerService.GetAll());
    }

    [Fact]
    public void TryAdd_MultiplePlayers_ExpectedPlayerIds()
    {
        // Arrange
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);

        IPlayerService playerService = new PlayerService(settings);
        IPlayer playerA = Substitute.For<IPlayer>();
        IPlayer playerB = Substitute.For<IPlayer>();
        IPlayer playerC = Substitute.For<IPlayer>();
        playerA.Username.Returns("playerA");
        playerB.Username.Returns("playerB");
        playerC.Username.Returns("playerC");

        // Act
        bool wasPlayerAAdded = playerService.TryAdd(playerA);
        bool wasPlayerBAdded = playerService.TryAdd(playerB);
        bool wasPlayerCAdded = playerService.TryAdd(playerC);

        // Assert
        Assert.True(wasPlayerAAdded);
        Assert.True(wasPlayerBAdded);
        Assert.True(wasPlayerCAdded);

        Assert.Equal(1, playerA.Id);
        Assert.Equal(2, playerB.Id);
        Assert.Equal(3, playerC.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(Constants.Players.HighestPlayerId)]
    public void TryAdd_ExceedingMaxPlayerCount_ExpectedFailure(byte maxPlayerCount)
    {
        // Arrange
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);
        _defaultServerSettings.MaxPlayers.Returns(maxPlayerCount);

        IPlayerService playerService = new PlayerService(settings);
        IPlayer exceedingPlayer = Substitute.For<IPlayer>();
        exceedingPlayer.Username.Returns("ExceedingPlayer");

        // Act
        for (int i = 1; i < maxPlayerCount; i++)
        {
            IPlayer player = Substitute.For<IPlayer>();
            player.Username.Returns($"Player{i}");
            bool wasPlayerAdded = playerService.TryAdd(player);

            Assert.True(wasPlayerAdded);
            Assert.Equal(i, player.Id);
        }

        bool wasExceedingPlayerAdded = playerService.TryAdd(exceedingPlayer);

        // Assert
        Assert.False(wasExceedingPlayerAdded);
        Assert.Equal(default, exceedingPlayer.Id);
    }

    [Fact]
    public void TryAdd_ReusedUsername_ReuseNotAdded()
    {
        // Arrange
        string sharedUsername = "username";
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);

        IPlayerService playerService = new PlayerService(settings);
        IPlayer playerA = Substitute.For<IPlayer>();
        IPlayer playerB = Substitute.For<IPlayer>();
        playerA.Username.Returns(sharedUsername);
        playerB.Username.Returns(sharedUsername);

        // Act
        bool wasPlayerAAdded = playerService.TryAdd(playerA);
        bool wasPlayerBAdded = playerService.TryAdd(playerB);

        // Assert
        Assert.True(wasPlayerAAdded);
        Assert.False(wasPlayerBAdded);
        Assert.Equal([playerA], playerService.GetAll());
    }
}
