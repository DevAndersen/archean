using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Archean.Core.Models.Worlds;

/// <summary>
/// Contains the blocks of a map.
/// </summary>
public class BlockMap
{
    /// <summary>
    /// The size of the X-dimension of the block map.
    /// </summary>
    public short Width { get; }

    /// <summary>
    /// The size of the Y-dimension of the block map.
    /// </summary>
    public short Height { get; }

    /// <summary>
    /// The size of the Z-dimension of the block map.
    /// </summary>
    public short Depth { get; }

    /// <summary>
    /// Provides positional read-write access to the blocks of the block map.
    /// </summary>
    public Block this[int x, int y, int z]
    {
        get => GetBlock(x, y, z);
        set => SetBlock(value, x, y, z);
    }

    /// <summary>
    /// The byte array containing the raw map data.
    /// </summary>
    private readonly byte[] data;

    /// <summary>
    /// Create a new map with the specified dimensions.
    /// </summary>
    public BlockMap(short width, short height, short depth)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(width, 0, nameof(width));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(height, 0, nameof(height));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(depth, 0, nameof(depth));

        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, Constants.Worlds.MaxWorldDimensionSize, nameof(width));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, Constants.Worlds.MaxWorldDimensionSize, nameof(height));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(depth, Constants.Worlds.MaxWorldDimensionSize, nameof(depth));

        Width = width;
        Height = height;
        Depth = depth;

        int blockCount = width * height * depth;
        data = new byte[sizeof(int) + blockCount];

        // Set the data length bytes.
        BinaryPrimitives.WriteInt32BigEndian(data, blockCount);
    }

    /// <summary>
    /// Create a new map with the specified dimensions.
    /// </summary>
    public BlockMap(short width, short height, short depth, ReadOnlySpan<Block> blocks) : this(width, height, depth)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(blocks.Length, width * height * depth, nameof(blocks));
        blocks.CopyTo(GetBlockBuffer());
    }

    /// <summary>
    /// Get the block at the specified position.
    /// </summary>
    public Block GetBlock(int x, int y, int z)
    {
        if (TryGetBlockIndex(x, y, z, out int index))
        {
            return GetBlockBuffer()[index];
        }

        return default;
    }

    /// <summary>
    /// Set the block at the specified position.
    /// </summary>
    public void SetBlock(Block block, int x, int y, int z)
    {
        if (TryGetBlockIndex(x, y, z, out int index))
        {
            GetBlockBuffer()[index] = block;
        }
    }

    /// <summary>
    /// Attempt to get the one-dimensional index of the specified block coordinate.
    /// </summary>
    public bool TryGetBlockIndex(int x, int y, int z, out int index)
    {
        if (IsValidBlockPosition(x, y, z))
        {
            index = x + (y * Width * Depth) + (z * Width);
            return true;
        }

        index = default;
        return false;
    }

    /// <summary>
    /// Determine if the coordinates are within the block area of the block map.
    /// </summary>
    public bool IsValidBlockPosition(int x, int y, int z)
    {
        return !(x < 0 || x > Width - 1
            || y < 0 || y > Height - 1
            || z < 0 || z > Depth - 1);
    }

    /// <summary>
    /// Get a read-only memory over the data buffer of the block map.
    /// </summary>
    public ReadOnlyMemory<byte> AsMemory()
    {
        return data.AsMemory();
    }

    /// <summary>
    /// Returns a Span over the block data.
    /// </summary>
    private Span<Block> GetBlockBuffer()
    {
        return MemoryMarshal.Cast<byte, Block>(data.AsSpan()[sizeof(int)..]);
    }
}
