using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

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
    public void Constructor_ValidBlockData_ExpectedSuccess()
    {
        // Setup
        ReadOnlySpan<byte> blockByteData = [1, 2, 3, 4];
        ReadOnlySpan<Block> blockData = MemoryMarshal.Cast<byte, Block>(blockByteData);

        // Action
        BlockMap blockMap = new BlockMap(2, 2, 1, blockData);

        // Assert
        Assert.NotNull(blockMap);
    }

    [Fact]
    public void Constructor_MismatchingDimensions_ThrowsException()
    {
        // Setup
        Block[] blockData = [(Block)1, (Block)2, (Block)3, (Block)4];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(2, 1, 1, blockData.AsSpan()));
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

    [Fact]
    public void AsMemory_ValidData_ExpectedBlockBytes()
    {
        // Setup
        BlockMap blockMap = new BlockMap(2, 2, 1);

        // Action
        blockMap[0, 0, 0] = Block.Stone;
        blockMap[1, 1, 0] = Block.Dirt;

        // Assert
        byte[] expectedBytes = [(byte)Block.Stone, 0, 0, (byte)Block.Dirt];
        Assert.Equal(blockMap.AsMemory().Span[sizeof(int)..], expectedBytes);
    }

    [Theory]
    [InlineData(2, 2, 1, 2 * 2 * 1)]
    [InlineData(100, 44, 54, 100 * 44 * 54)]
    public void AsMemory_ValidData_ExpectedLengthBytes(short width, short height, short depth, int expectedLength)
    {
        // Setup
        BlockMap blockMap = new BlockMap(width, height, depth);

        // Action
        ReadOnlySpan<byte> lengthBytes = blockMap.AsMemory().Span[..sizeof(int)];
        int length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);

        // Assert
        Assert.Equal(expectedLength, length);
    }

    [Fact]
    public void IsValidBlockPosition_ValidInput_ExpectedSuccess()
    {
        // Setup
        BlockMap blockMap = new BlockMap(3, 3, 3);

        // Assert
        Assert.True(blockMap.IsValidBlockPosition(0, 0, 0));
        Assert.True(blockMap.IsValidBlockPosition(1, 1, 1));
        Assert.True(blockMap.IsValidBlockPosition(2, 2, 2));
    }

    [Fact]
    public void IsValidBlockPosition_InvalidInput_ExpectedFailure()
    {
        // Setup
        BlockMap blockMap = new BlockMap(3, 3, 3);

        // Assert
        Assert.False(blockMap.IsValidBlockPosition(-1, -1, -1));
        Assert.False(blockMap.IsValidBlockPosition(3, 3, 3));
    }
}
