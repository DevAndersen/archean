using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly ILogger<ClientPacketHandler> logger;
    private readonly IConnectionService connectionService;
    private readonly IGlobalEventBus globalEventBus;
    private readonly IEventListener eventListener;
    private readonly IServerPacketWriter serverPacketWriter;

    public ClientPacketHandler(ILogger<ClientPacketHandler> logger, IConnectionService connectionService, IGlobalEventBus globalEventBus, IEventListener eventListener, IServerPacketWriter serverPacketWriter)
    {
        this.logger = logger;
        this.connectionService = connectionService;
        this.globalEventBus = globalEventBus;
        this.eventListener = eventListener;

        eventListener.Subscribe<MessageEvent>(ReceiveMessage);
        eventListener.Subscribe<SetBlockEvent>(ReceiveSetBlock);
        this.serverPacketWriter = serverPacketWriter;
    }

    private async Task ReceiveMessage(MessageEvent arg)
    {
        if (connectionService.TryGetConnection(out IConnection? connection))
        {
            await connection.SendAsync(serverPacketWriter.WriteMessagePacket(new ServerMessagePacket
            {
                Message = arg.Message,
                PlayerId = 0
            }));
        }
    }

    private async Task ReceiveSetBlock(SetBlockEvent arg)
    {
        if (connectionService.TryGetConnection(out IConnection? connection))
        {
            await connection.SendAsync(serverPacketWriter.WriteSetBlockPacket(new ServerSetBlockPacket
            {
                BlockType = arg.Mode == BlockChangeMode.Break ? Block.Air : arg.BlockType,
                X = arg.X,
                Y = arg.Y,
                Z = arg.Z
            }));
        }
    }

    public async Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        await globalEventBus.InvokeEventAsync(new MessageEvent
        {
            Message = packet.Message
        });
    }

    public Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        //logger.LogInformation("{x}:{y}:{z}", packet.X, packet.Y, packet.Z);
        return Task.CompletedTask;
    }

    public async Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        await globalEventBus.InvokeEventAsync(new SetBlockEvent
        {
            X = packet.X,
            Y = packet.Y,
            Z = packet.Z,
            Mode = packet.Mode,
            BlockType = packet.BlockType,
        });
    }
}
