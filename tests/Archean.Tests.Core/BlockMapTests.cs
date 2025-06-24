using Archean.Core;
using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using System.Buffers.Binary;

namespace Archean.Tests.Core;

public class BlockMapTests
{
    [Fact]
    public void Constructor_ValidData_ExpectedDimensions()
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 32, 48);

        // Assert
        Assert.Equal(16, blockMap.Width);
        Assert.Equal(32, blockMap.Height);
        Assert.Equal(48, blockMap.Depth);
    }

    [Fact]
    public void Constructor_ValidData_ExpectedBufferLength()
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 32, 48);

        // Act
        ReadOnlyMemory<byte> buffer = blockMap.AsMemory();
        int blockBufferLength = BinaryPrimitives.ReadInt32BigEndian(buffer.Span);
        int expectedBlockCount = 16 * 32 * 48;

        // Assert
        Assert.Equal(expectedBlockCount + sizeof(int), buffer.Length);
        Assert.Equal(expectedBlockCount, blockBufferLength);
    }

    [Fact]
    public void Constructor_ZeroSize_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(0, 32, 48));
    }

    [Fact]
    public void Constructor_NegativeSize_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(-1, 32, 48));
    }

    [Theory]
    [InlineData(15, 16, 16)]
    [InlineData(16, 15, 16)]
    [InlineData(16, 16, 15)]
    public void Constructor_DimensionsTooSmall_ThrowsException(int width, int height, int depth)
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(width, height, depth));
    }

    [Fact]
    public void Constructor_DimensionTooBig_ThrowsException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(Constants.Worlds.MaxWorldDimensionSize + 4, 32, 48));
    }

    [Fact]
    public void Constructor_ValidBlockData_ExpectedSuccess()
    {
        // Arrange
        int width = 16;
        int height = 32;
        int depth = 48;
        int blockDataLength = width * height * depth;
        Block[] blockData = new Block[blockDataLength];

        // Act
        BlockMap blockMap = new BlockMap(width, depth, height, blockData);

        // Assert
        Assert.NotNull(blockMap);
    }

    [Fact]
    public void Constructor_MismatchingDimensions_ThrowsException()
    {
        // Arrange
        int width = 16;
        int height = 32;
        int depth = 48;
        int blockDataLength = width * height * depth;
        Block[] blockData = new Block[blockDataLength];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlockMap(width + 16, depth, height, blockData.AsSpan()));
    }

    [Theory]
    [InlineData(31, 32, 32)]
    [InlineData(32, 31, 32)]
    [InlineData(32, 32, 31)]
    public void Constructor_DimensionsNotEvenlyDivisible_ThrowsException(int width, int height, int depth)
    {
        // Assert
        Assert.Throws<ArgumentException>(() => new BlockMap(width, depth, height));
    }

    [Fact]
    public void GetBlock_ValidCoordinates_ReturnsBlock()
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 32, 48);

        // Act
        blockMap[1, 2, 3] = Block.Stone;

        // Assert
        Assert.Equal(Block.Stone, blockMap[1, 2, 3]);
    }

    [Fact]
    public void AsMemory_ValidData_ExpectedBlockBytes()
    {
        // Arrange
        int width = 16;
        int height = 32;
        int depth = 48;
        int blockDataLength = width * height * depth;
        BlockMap blockMap = new BlockMap(width, depth, height);

        // Act
        blockMap[0, 0, 0] = Block.Stone;
        blockMap[1, 0, 0] = Block.Dirt;
        blockMap[0, 0, 1] = Block.Grass;

        // Assert
        byte[] expectedBytes = new byte[blockDataLength];
        expectedBytes[0] = (byte)Block.Stone;
        expectedBytes[1] = (byte)Block.Dirt;
        expectedBytes[16] = (byte)Block.Grass;
        Assert.Equal(blockMap.AsMemory().Span[sizeof(int)..], expectedBytes);
    }

    [Theory]
    [InlineData(16, 32, 48, 16 * 32 * 48)]
    public void AsMemory_ValidData_ExpectedLengthBytes(int width, int height, int depth, int expectedLength)
    {
        // Arrange
        BlockMap blockMap = new BlockMap(width, height, depth);

        // Act
        ReadOnlySpan<byte> lengthBytes = blockMap.AsMemory().Span[..sizeof(int)];
        int length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);

        // Assert
        Assert.Equal(expectedLength, length);
    }

    [Fact]
    public void IsValidBlockPosition_ValidInput_ExpectedSuccess()
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 32, 48);

        // Assert
        Assert.True(blockMap.IsValidBlockPosition(0, 0, 0));
        Assert.True(blockMap.IsValidBlockPosition(1, 1, 1));
        Assert.True(blockMap.IsValidBlockPosition(2, 2, 2));
        Assert.True(blockMap.IsValidBlockPosition(15, 31, 47));
    }

    [Fact]
    public void IsValidBlockPosition_InvalidInput_ExpectedFailure()
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 32, 48);

        // Assert
        Assert.False(blockMap.IsValidBlockPosition(-1, -1, -1));
        Assert.False(blockMap.IsValidBlockPosition(16, 32, 48));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, 1)]
    [InlineData(15, 0, 0, 15)]
    [InlineData(0, 1, 0, 256)]
    [InlineData(0, 15, 0, 3840)]
    [InlineData(0, 0, 1, 16)]
    [InlineData(0, 0, 15, 240)]
    [InlineData(15, 15, 15, 4095)]
    public void TryGetBlockIndex_ValidInput_ExpectedOutput(int x, int y, int z, int expectedIndex)
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 16, 16);

        // Act
        bool isValidIndex = blockMap.TryGetBlockIndex(x, y, z, out int actualIndex);

        // Assert
        Assert.True(isValidIndex);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(-1, -1, -1)]
    [InlineData(16, 15, 15)]
    [InlineData(15, 16, 15)]
    [InlineData(15, 15, 16)]
    [InlineData(16, 16, 16)]
    public void TryGetBlockIndex_InputOutOfRange_ExpectedFailure(int x, int y, int z)
    {
        // Arrange
        BlockMap blockMap = new BlockMap(16, 16, 16);

        // Act
        bool isValidIndex = blockMap.TryGetBlockIndex(x, y, z, out _);

        // Assert
        Assert.False(isValidIndex);
    }
}
