﻿using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Worlds;

public interface IWorldRegistry
{
    IWorld? GetDefaultWorld();

    Task<bool> RegisterWorldAsync(IWorld world);

    Task<IWorld?> CreateWorldAsync(string name);

    Task<bool> DeleteWorldAsync(IWorld world);

    Task<bool> UnregisterWorldAsync(string name);

    IEnumerable<IWorld> GetWorlds();

    bool TryGetWorld(string name, [NotNullWhen(true)] out IWorld? world);

    Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld);
}
