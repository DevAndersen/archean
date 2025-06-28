using Archean.Core;
using Archean.Core.Helpers;
using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Settings;
using Archean.Worlds.Services.Persistence;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Worlds.Models;

public class TestWorld : IWorld
{
    private const int LoadSemaphoreTimeout = 1000 * 10;

    private readonly ILogger<TestWorld> _logger;
    private readonly IGlobalEventListener _eventListener;
    private readonly ServerSettings _serverSettings;
    private readonly WorldPersistenceHandler _worldPersistenceHandler;

    private readonly List<IPlayer> _players = [];
    private readonly SemaphoreSlim _loadSemaphore;

    public BlockMap? Blocks { get; internal set; }

    public string Name { get; private set; }

    [MemberNotNullWhen(true, nameof(Blocks))]
    public bool IsLoaded { get; private set; }

    public IReadOnlyList<IPlayer> Players => _players;

    public TestWorld(
        string name,
        ILogger<TestWorld> logger,
        IGlobalEventListener eventListener,
        ServerSettings serverSettings,
        WorldPersistenceHandler worldPersistenceHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        _logger = logger;
        _eventListener = eventListener;
        _serverSettings = serverSettings;
        _worldPersistenceHandler = worldPersistenceHandler;

        _loadSemaphore = new SemaphoreSlim(1);
    }

    public async Task<bool> LoadAsync()
    {
        if (await _loadSemaphore.WaitAsync(LoadSemaphoreTimeout))
        {
            if (IsLoaded)
            {
                _logger.LogWarning("Attempted to load already loaded world {worldName}",
                    Name);

                return false;
            }

            if (!await _worldPersistenceHandler.LoadWorldAsync(this))
            {
                _logger.LogWarning("Failed to load world {worldName}",
                    Name);

                return false;
            }

            _eventListener.Subscribe<SetBlockEvent>(OnSetBlockAsync);
            _eventListener.Subscribe<PositionAndOrientationEvent>(OnPlayerMoveAsync);
            _eventListener.Subscribe<PlayerDisconnectEvent>(OnPlayerLeaveAsync);

            IsLoaded = true;

            _loadSemaphore.Release();
            return true;
        }

        return false;
    }

    public async Task UnloadAsync()
    {
        if (await _loadSemaphore.WaitAsync(LoadSemaphoreTimeout))
        {
            if (!IsLoaded)
            {
                return;
            }

            IsLoaded = false;

            // Todo: Transfer all players to the default world.

            await _worldPersistenceHandler.SaveWorldAsync(this);

            _eventListener.Unsubscribe<SetBlockEvent>(OnSetBlockAsync);
            _eventListener.Unsubscribe<PositionAndOrientationEvent>(OnPlayerMoveAsync);
            _eventListener.Unsubscribe<PlayerDisconnectEvent>(OnPlayerLeaveAsync);

            _loadSemaphore.Release();
        }
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
