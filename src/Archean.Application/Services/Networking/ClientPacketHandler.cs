namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly IPlayerService playerService;
    private readonly IGlobalEventBus globalEventBus;

    private FShort x;
    private FShort y;
    private FShort z;
    private byte yaw;
    private byte pitch;

    public ClientPacketHandler(
        IPlayerService playerService,
        IGlobalEventBus globalEventBus)
    {
        this.playerService = playerService;
        this.globalEventBus = globalEventBus;
    }

    public async Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await globalEventBus.InvokeEventAsync(new MessageEvent
            {
                PlayerSender = player,
                Message = packet.Message
            });
        }
    }

    public async Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            if (packet.X != x
                || packet.Y != y
                || packet.Z != z
                || packet.Pitch != pitch
                || packet.Yaw != yaw)
            {
                x = packet.X;
                y = packet.Y;
                z = packet.Z;
                pitch = packet.Pitch;
                yaw = packet.Yaw;

                player.UpdatePositionAndRotation(
                    packet.X.ToFloat(),
                    packet.Y.ToFloat(),
                    packet.Z.ToFloat(),
                    packet.Pitch,
                    packet.Yaw);

                await globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
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
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await globalEventBus.InvokeEventAsync(new SetBlockEvent
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
