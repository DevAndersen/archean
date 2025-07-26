using System.Diagnostics.CodeAnalysis;

namespace Archean.Networking.Services;

public class SessionService : ISessionService
{
    private IPlayer? _player;

    public IPlayer? GetPlayer()
    {
        return _player;
    }

    public bool TryGetPlayer([NotNullWhen(true)] out IPlayer? player)
    {
        player = _player;
        return player != null;
    }

    public void SetPlayer(IPlayer player)
    {
        _player = player;
    }
}
