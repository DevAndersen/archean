using System.Diagnostics.CodeAnalysis;

namespace Archean.Application.Services.Networking;

public class PlayerService : IPlayerService
{
    private IPlayer? player;

    public IPlayer? GetPlayer()
    {
        return player;
    }

    public bool TryGetPlayer([NotNullWhen(true)] out IPlayer? player)
    {
        player = this.player;
        return player != null;
    }

    public void SetPlayer(IPlayer player)
    {
        this.player = player;
    }
}
