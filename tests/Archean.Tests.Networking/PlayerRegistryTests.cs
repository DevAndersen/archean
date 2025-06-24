using Archean.Core.Models;
using Archean.Core.Services.Networking;
using Archean.Core.Settings;
using Archean.Networking.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Archean.Tests.Networking;

public class PlayerRegistryTests
{
    private readonly ServerSettings _defaultServerSettings;

    public PlayerRegistryTests()
    {
        _defaultServerSettings = Substitute.For<ServerSettings>();
        _defaultServerSettings.MaxPlayers.Returns((byte)Constants.Players.HighestPlayerId);
    }

    [Fact]
    public void TryAdd_SinglePlayer_ExpectedGetAllResult()
    {
        // Setup
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);

        IPlayerRegistry playerRegistry = new PlayerRegistry(settings);
        IPlayer player = Substitute.For<IPlayer>();

        // Action
        bool wasPlayerAdded = playerRegistry.TryAdd(player);

        // Assert
        Assert.True(wasPlayerAdded);
        Assert.Equal([player], playerRegistry.GetAll());
    }

    [Fact]
    public void TryAdd_MultiplePlayers_ExpectedPlayerIds()
    {
        // Setup
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);

        IPlayerRegistry playerRegistry = new PlayerRegistry(settings);
        IPlayer playerA = Substitute.For<IPlayer>();
        IPlayer playerB = Substitute.For<IPlayer>();
        IPlayer playerC = Substitute.For<IPlayer>();

        // Action
        bool wasPlayerAAdded = playerRegistry.TryAdd(playerA);
        bool wasPlayerBAdded = playerRegistry.TryAdd(playerB);
        bool wasPlayerCAdded = playerRegistry.TryAdd(playerC);

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
        // Setup
        IOptions<ServerSettings> settings = Substitute.For<IOptions<ServerSettings>>();
        settings.Value.Returns(_defaultServerSettings);
        _defaultServerSettings.MaxPlayers.Returns(maxPlayerCount);

        IPlayerRegistry playerRegistry = new PlayerRegistry(settings);
        IPlayer exceedingPlayer = Substitute.For<IPlayer>();

        // Action
        for (int i = 1; i < maxPlayerCount; i++)
        {
            IPlayer player = Substitute.For<IPlayer>();
            bool wasPlayerAdded = playerRegistry.TryAdd(player);

            Assert.True(wasPlayerAdded);
            Assert.Equal(i, player.Id);
        }

        bool wasExceedingPlayerAdded = playerRegistry.TryAdd(exceedingPlayer);

        // Assert
        Assert.False(wasExceedingPlayerAdded);
        Assert.Equal(default, exceedingPlayer.Id);
    }
}
