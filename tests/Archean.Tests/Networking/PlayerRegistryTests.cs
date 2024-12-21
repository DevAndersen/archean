using Archean.Application.Services.Networking;
using Archean.Core.Models;
using Archean.Core.Services.Networking;
using NSubstitute;

namespace Archean.Tests.Networking;

public class PlayerRegistryTests
{
    [Fact]
    public void TryAdd_SinglePlayer_ExpectedGetAllResult()
    {
        // Setup
        IPlayerRegistry playerRegistry = new PlayerRegistry();
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
        IPlayerRegistry playerRegistry = new PlayerRegistry();
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

        Assert.Equal(1, playerB.Id);
        Assert.Equal(2, playerC.Id);
    }

    [Fact]
    public void TryAdd_ExceedingMaxPlayerCount_ExpectedFailure()
    {
        // Setup
        IPlayerRegistry playerRegistry = new PlayerRegistry();
        IPlayer exceedingPlayer = Substitute.For<IPlayer>();

        // Action
        for (int i = 0; i <= Constants.Players.HighestPlayerId; i++)
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
