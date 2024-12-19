using Archean.Core.Models;
using Archean.Core.Models.Networking;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Networking;

public interface IPlayerRegistry
{
    public IEnumerable<IPlayer> GetAll();

    public bool TryAdd(IPlayer player, [NotNullWhen(false)] out string? errorMessage);

    public void Remove(IPlayer player);

    public void Remove(IConnection connection);
}
