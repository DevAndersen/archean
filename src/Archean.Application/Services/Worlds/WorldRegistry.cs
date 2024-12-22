using Archean.Application.Models.Worlds;
using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;

namespace Archean.Application.Services.Worlds;

public class WorldRegistry : IWorldRegistry
{
    private readonly IWorld defaultWorld;
    private readonly ILogger<WorldRegistry> logger;

    private readonly List<IWorld> worlds = [];

    public WorldRegistry(
        ILogger<WorldRegistry> logger,
        IServerPacketWriter serverPacketWriter,
        IEventListener eventListener)
    {
        this.logger = logger;
        defaultWorld = new TestWorld(logger, serverPacketWriter, eventListener);
        defaultWorld.LoadAsync().GetAwaiter().GetResult(); // Todo
        worlds.Add(defaultWorld);
    }

    public IWorld GetDefaultWorld()
    {
        return defaultWorld;
    }

    public async Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld)
    {
        logger.LogInformation("Transferring player {username} to world {world}",
            player.Username,
            destinationWorld.Name);

        IWorld currentWorld = defaultWorld; // Todo: Get the current world for the connection.

        await currentWorld.LeaveAsync(player);
        await destinationWorld.JoinAsync(player);
    }
}
