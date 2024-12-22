using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Application.Models.Worlds;

public class TestWorld : IWorld
{
    private readonly ILogger logger;
    private readonly IServerPacketWriter serverPacketWriter;
    private readonly IEventListener eventListener;

    private readonly BlockMap blockMap;
    private readonly List<IPlayer> players = [];

    public string Name { get; } = $"Test world {Guid.NewGuid()}";

    public IReadOnlyList<IPlayer> Players => players;

    public TestWorld(
        ILogger logger,
        IServerPacketWriter serverPacketWriter,
        IEventListener eventListener)
    {
        this.logger = logger;
        this.serverPacketWriter = serverPacketWriter;
        this.eventListener = eventListener;

        short width = 16;
        short height = 16;
        short depth = 16;

        blockMap = new BlockMap(width, height, depth);

        for (int x = 0; x < blockMap.Width; x++)
        {
            for (int z = 0; z < blockMap.Depth; z++)
            {
                for (int y = 0; y < 7; y++)
                {
                    blockMap[x, y, z] = Block.Dirt;
                }

                blockMap[x, 7, z] = Block.Grass;
            }
        }
    }

    public Task LoadAsync()
    {
        eventListener.Subscribe<SetBlockEvent>(OnSetBlockAsync);
        eventListener.Subscribe<SetBlockEvent>(ReceiveSetBlock);

        return Task.CompletedTask;
    }

    public Task UnloadAsync()
    {
        return Task.CompletedTask;
    }

    public async Task JoinAsync(IPlayer player)
    {
        logger.LogInformation("Player {username} joining world {world}",
            player.Username,
            Name);

        await SendJoinServerTestAsync(player);
        players.Add(player);
    }

    private Task OnSetBlockAsync(SetBlockEvent eventArgs)
    {
        if (eventArgs.Player != null && players.Contains(eventArgs.Player))
        {
            blockMap[eventArgs.X, eventArgs.Y, eventArgs.Z] = eventArgs.Block;
        }
        return Task.CompletedTask;
    }

    private async Task SendJoinServerTestAsync(IPlayer player)
    {
        IConnection connection = player.Connection;

        await connection.SendAsync(new ServerLevelInitializePacket());
        await SendLevelTestAsync(blockMap, connection);

        await connection.SendAsync(new ServerLevelFinalizePacket
        {
            XSize = blockMap.Width,
            YSize = blockMap.Height,
            ZSize = blockMap.Depth,
        });

        await connection.SendAsync(new ServerSpawnPlayerPacket
        {
            PlayerId = Constants.Networking.PlayerSelfId,
            PlayerName = "Todo", // Todo
            X = new FShort(4F),
            Y = new FShort(blockMap.Height + 3),
            Z = new FShort(4F),
            Pitch = 0,
            Yaw = 0
        });
    }

    private async Task SendLevelTestAsync(BlockMap blockMap, IConnection connection)
    {
        byte[] blockGZipBuffer = GZipHelper.Compress(blockMap.AsMemory().Span);
        int chunkCount = (blockGZipBuffer.Length / Constants.Networking.ByteArrayLength) + 1;

        for (int i = 0; i < chunkCount; i++)
        {
            int start = i * Constants.Networking.ByteArrayLength;
            int end = Math.Min(blockGZipBuffer.Length - start, Constants.Networking.ByteArrayLength);
            int length = end - start;
            byte percent = (byte)((i + 1) / (float)chunkCount * 100);

            Memory<byte> chunk = blockGZipBuffer
                .AsMemory()
                .Slice(start, end);

            Memory<byte> buffer;
            if (length < Constants.Networking.ByteArrayLength)
            {
                buffer = new byte[Constants.Networking.ByteArrayLength];
                chunk.CopyTo(buffer);
            }
            else
            {
                buffer = chunk;
            }

            await connection.SendAsync(new ServerIdentificationPacket
            {
                PlayerType = PlayerType.Op,
                ProtocolVersion = Constants.Networking.ProtocolVersion,
                ServerMotd = $"You are {percent}% done",
                ServerName = "Server name",
            });

            await connection.SendAsync(new ServerLevelDataChunkPacket
            {
                ChunkData = buffer.ToArray(),
                ChunkLength = (short)length,
                PercentageComplete = percent
            });
        }
    }

    public Task LeaveAsync(IPlayer player)
    {
        logger.LogInformation("Player {username} leaving world {world}",
            player.Username,
            Name);

        IPlayer? matchingPlayer = players.FirstOrDefault(x => x == player);
        if (matchingPlayer == null)
        {
            // Todo: Log unexpected error finding player.
            return Task.CompletedTask;
        }

        players.Remove(matchingPlayer);

        // Todo
        return Task.CompletedTask;
    }

    private async Task ReceiveSetBlock(SetBlockEvent arg)
    {
        if (arg.Player != null && players.Contains(arg.Player))
        {
            blockMap[arg.X, arg.Y, arg.Z] = arg.Block;

            foreach (IPlayer? otherPlayer in players.Except([arg.Player]))
            {
                await otherPlayer.Connection.SendAsync(new ServerSetBlockPacket
                {
                    BlockType = arg.Mode == BlockChangeMode.Break ? Block.Air : arg.Block,
                    X = arg.X,
                    Y = arg.Y,
                    Z = arg.Z
                });
            }
        }
    }
}
