namespace Archean.Networking.Services;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly ISessionService _sessionService;
    private readonly IGlobalEventBus _globalEventBus;

    private float _x;
    private float _y;
    private float _z;
    private byte _yaw;
    private byte _pitch;

    public ClientPacketHandler(
        ISessionService sessionService,
        IGlobalEventBus globalEventBus)
    {
        _sessionService = sessionService;
        _globalEventBus = globalEventBus;
    }

    public async Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        if (_sessionService.TryGetPlayer(out IPlayer? player))
        {
            await _globalEventBus.InvokeEventAsync(new MessageEvent
            {
                PlayerSender = player,
                Message = packet.Message
            });
        }
    }

    public async Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        if (_sessionService.TryGetPlayer(out IPlayer? player))
        {
            if (packet.X != _x
                || packet.Y != _y
                || packet.Z != _z
                || packet.Pitch != _pitch
                || packet.Yaw != _yaw)
            {
                _x = packet.X;
                _y = packet.Y;
                _z = packet.Z;
                _pitch = packet.Pitch;
                _yaw = packet.Yaw;

                player.UpdatePositionAndRotation(
                    packet.X,
                    packet.Y,
                    packet.Z,
                    packet.Pitch,
                    packet.Yaw);

                await _globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
                {
                    Player = player,
                    X = packet.X,
                    Y = packet.Y,
                    Z = packet.Z,
                    Pitch = packet.Pitch,
                    Yaw = packet.Yaw
                });
            }
        }
    }

    public async Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        if (_sessionService.TryGetPlayer(out IPlayer? player))
        {
            await _globalEventBus.InvokeEventAsync(new SetBlockEvent
            {
                Player = player,
                X = packet.X,
                Y = packet.Y,
                Z = packet.Z,
                Mode = packet.Mode,
                Block = packet.Mode == BlockChangeMode.Place ? packet.BlockType : Block.Air,
                HeldBlock = packet.BlockType,
            });
        }
    }
}
