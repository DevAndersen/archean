using System.Buffers.Binary;
using Archean.Core.Models;
using Archean.Core.Models.World;

namespace Archean.Tests;

public class BlockMapTests
{
    [Fact]
    public void Constructor_ValidData_ExpectedDimensions()
    {
        // Setup
        BlockMap blockMap = new BlockMap(4, 5, 6);

        // Assert
        Assert.Equal(4, blockMap.Width);
        Assert.Equal(5, blockMap.Height);
        Assert.Equal(6, blockMap.Depth);
    }

    [Fact]
    public void Constructor_ValidData_ExpectedBufferLength()
    {
        // Setup
        BlockMap blockMap = new BlockMap(4, 5, 6);

        // Action
        ReadOnlyMemory<byte> buffer = blockMap.AsMemory();
        int blockBufferLength = BinaryPrimitives.ReadInt32BigEndian(buffer.Span);

        // Assert
        Assert.Equal(4 * 5 * 6 + sizeof(int), buffer.Length);
        Assert.Equal(4 * 5 * 6, blockBufferLength);
    }

    [Fact]
    public void Constructor_ZeroSize_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(0, 1, 1));
    }

    [Fact]
    public void Constructor_NegativeSize_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(-1, 1, 1));
    }

    [Fact]
    public void Constructor_DimensionTooBig_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(Constants.Worlds.MaxWorldDimensionSize + 4, 1, 1));
    }

    [Fact]
    public void GetBlock_ValidCoordinates_ReturnsBlock()
    {
        // Setup
        BlockMap blockMap = new BlockMap(4, 4, 4);

        // Action
        blockMap[1, 2, 3] = Block.Stone;

        // Assert
        Assert.Equal(Block.Stone, blockMap[1, 2, 3]);
    }
}
