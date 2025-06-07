using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Networking.Services;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly IPlayerService _playerService;
    private readonly IGlobalEventBus _globalEventBus;

    private FShort _x;
    private FShort _y;
    private FShort _z;
    private byte _yaw;
    private byte _pitch;

    public ClientPacketHandler(
        IPlayerService playerService,
        IGlobalEventBus globalEventBus)
    {
        _playerService = playerService;
        _globalEventBus = globalEventBus;
    }

    public async Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        if (_playerService.TryGetPlayer(out IPlayer? player))
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
        if (_playerService.TryGetPlayer(out IPlayer? player))
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
                    packet.X.ToFloat(),
                    packet.Y.ToFloat(),
                    packet.Z.ToFloat(),
                    packet.Pitch,
                    packet.Yaw);

                await _globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
                {
                    Player = player,
                    X = packet.X.ToFloat(),
                    Y = packet.Y.ToFloat(),
                    Z = packet.Z.ToFloat(),
                    Pitch = packet.Pitch,
                    Yaw = packet.Yaw
                });
            }
        }
    }

    public async Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        if (_playerService.TryGetPlayer(out IPlayer? player))
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
