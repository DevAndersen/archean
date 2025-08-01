﻿using Archean.Core.Models;
using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPlayerService
{
    IEnumerable<IPlayer> GetAll();

    bool TryAdd(IPlayer player);

    void Remove(IPlayer player);

    void Remove(IConnection connection);
}
