using Archean.Core.Models;
using Archean.Core.Models.Worlds;

namespace Archean.Core.Services.Worlds;

public interface IWorldRegistry
{
    IWorld GetDefaultWorld();

    Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld);
}
