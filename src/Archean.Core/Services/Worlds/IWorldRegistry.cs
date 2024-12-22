using Archean.Core.Models;
using Archean.Core.Models.Worlds;

namespace Archean.Core.Services.Worlds;

public interface IWorldRegistry
{
    public IWorld GetDefaultWorld();

    public Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld);
}
