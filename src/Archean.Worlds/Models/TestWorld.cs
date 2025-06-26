using Archean.Core;
using Archean.Core.Helpers;
using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Settings;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Worlds.Models;

public class TestWorld : IWorld
{
    private readonly ILogger<TestWorld> _logger;
    private readonly IGlobalEventListener _eventListener;
    private readonly ServerSettings _serverSettings;

    private readonly List<IPlayer> _players = [];

    public BlockMap? Blocks { get; private set; }

    public string Name { get; private set; }

    [MemberNotNullWhen(true, nameof(Blocks))]
    public bool IsLoaded { get; private set; }

    public IReadOnlyList<IPlayer> Players => _players;

    private readonly Lock _loadLock = new Lock();

    public TestWorld(
        string name,
        ILogger<TestWorld> logger,
        IGlobalEventListener eventListener,
        ServerSettings serverSettings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        _logger = logger;
        _eventListener = eventListener;
        _serverSettings = serverSettings;

        short width = 16;
        short height = 16;
        short depth = 16;

        Blocks = new BlockMap(width, height, depth);

        for (int x = 0; x < Blocks.Width; x++)
        {
            for (int z = 0; z < Blocks.Depth; z++)
            {
                for (int y = 0; y < 7; y++)
                {
                    Blocks[x, y, z] = Block.Dirt;
                }

                Blocks[x, 7, z] = Block.Grass;
            }
        }
    }

    public Task LoadAsync()
    {
        lock (_loadLock)
        {
            if (IsLoaded)
            {
                return Task.CompletedTask;
            }

            _eventListener.Subscribe<SetBlockEvent>(OnSetBlockAsync);
            _eventListener.Subscribe<PositionAndOrientationEvent>(OnPlayerMoveAsync);
            _eventListener.Subscribe<PlayerDisconnectEvent>(OnPlayerLeaveAsync);

            IsLoaded = true;
        }

        return Task.CompletedTask;
    }

    public Task UnloadAsync()
    {
        lock (_loadLock)
        {
            if (!IsLoaded)
            {
                return Task.CompletedTask;
            }

            IsLoaded = false;

            // Todo: Transfer all players to the default world.

            _eventListener.Unsubscribe<SetBlockEvent>(OnSetBlockAsync);
            _eventListener.Unsubscribe<PositionAndOrientationEvent>(OnPlayerMoveAsync);
            _eventListener.Unsubscribe<PlayerDisconnectEvent>(OnPlayerLeaveAsync);
        }

        return Task.CompletedTask;
    }

    public async Task JoinAsync(IPlayer player)
    {
        _logger.LogInformation("Player {username} joining world {world}",
            player.Username,
            Name);

        await SendJoinServerTestAsync(player);

        foreach (IPlayer otherPlayer in _players)
        {
            await player.Connection.SendAsync(new ServerSpawnPlayerPacket
            {
                PlayerId = otherPlayer.Id,
                PlayerName = otherPlayer.DisplayName,
                X = otherPlayer.PosX,
                Y = otherPlayer.PosY,
                Z = otherPlayer.PosZ,
                Pitch = otherPlayer.Pitch,
                Yaw = otherPlayer.Yaw
            });

            await otherPlayer.Connection.SendAsync(new ServerSpawnPlayerPacket
            {
                PlayerId = player.Id,
                PlayerName = player.DisplayName,
                X = player.PosX,
                Y = player.PosY,
                Z = player.PosZ,
                Pitch = player.Pitch,
                Yaw = player.Yaw
            });
        }

        _players.Add(player);
    }

    private async Task SendJoinServerTestAsync(IPlayer player)
    {
        if (!IsLoaded)
        {
            // Todo
            return;
        }

        IConnection connection = player.Connection;

        await connection.SendAsync(new ServerLevelInitializePacket());
        await SendLevelTestAsync(Blocks, connection);

        await connection.SendAsync(new ServerLevelFinalizePacket
        {
            XSize = Blocks.Width,
            YSize = Blocks.Height,
            ZSize = Blocks.Depth,
        });

        await connection.SendAsync(new ServerSpawnPlayerPacket
        {
            PlayerId = Constants.Networking.PlayerSelfId,
            PlayerName = player.Username,
            X = 4F,
            Y = Blocks.Height + 3,
            Z = 4F,
            Pitch = 0,
            Yaw = 0
        });
    }

    private async Task SendLevelTestAsync(BlockMap blockMap, IConnection connection)
    {
        byte[] blockGZipBuffer = GZipHelper.Compress(blockMap.AsMemory().Span);
        int chunkCount = blockGZipBuffer.Length / Constants.Networking.ByteArrayLength + 1;

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
                ServerMotd = string.Format(_serverSettings.WorldLoadingMotd, percent),
                ServerName = _serverSettings.Name,
            });

            await connection.SendAsync(new ServerLevelDataChunkPacket
            {
                ChunkData = buffer.ToArray(),
                ChunkLength = (short)length,
                PercentageComplete = percent
            });
        }
    }

    public async Task LeaveAsync(IPlayer player)
    {
        _logger.LogInformation("Player {username} leaving world {world}",
            player.Username,
            Name);

        IPlayer? matchingPlayer = _players.FirstOrDefault(x => x == player);
        if (matchingPlayer == null)
        {
            return;
        }

        _players.Remove(matchingPlayer);

        foreach (IPlayer otherPlayer in _players)
        {
            await otherPlayer.Connection.SendAsync(new ServerDespawnPlayerPacket
            {
                PlayerId = player.Id
            });
        }
    }

    private async Task OnSetBlockAsync(SetBlockEvent arg)
    {
        if (!IsLoaded)
        {
            // Todo
            return;
        }

        if (arg.Player != null && _players.Contains(arg.Player))
        {
            Blocks[arg.X, arg.Y, arg.Z] = arg.Block;

            foreach (IPlayer? otherPlayer in _players.Except([arg.Player]))
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

    private async Task OnPlayerMoveAsync(PositionAndOrientationEvent arg)
    {
        foreach (IPlayer? otherPlayer in _players.Except([arg.Player]))
        {
            await otherPlayer.Connection.SendAsync(new ServerAbsolutePositionAndOrientationPacket
            {
                PlayerId = arg.Player.Id,
                X = arg.X,
                Y = arg.Y,
                Z = arg.Z,
                Pitch = arg.Pitch,
                Yaw = arg.Yaw
            });
        }
    }

    private async Task OnPlayerLeaveAsync(PlayerDisconnectEvent arg)
    {
        await LeaveAsync(arg.Player);
    }
}
