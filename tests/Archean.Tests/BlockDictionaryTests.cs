using Archean.Application.Services;
using Archean.Core.Models;
using Microsoft.Extensions.Logging;

namespace Archean.Tests;

public class BlockDictionaryTests
{
    private readonly BlockDictionary blockDictionary;

    public BlockDictionaryTests()
    {
        ILogger<BlockDictionary> logger = NSubstitute.Substitute.For<ILogger<BlockDictionary>>();
        blockDictionary = new BlockDictionary(logger);
    }

    [Fact]
    public void TryGetBlock_ValidStandardIdentity_ExpectedValue()
    {
        // Setup
        blockDictionary.RegisterStandardBlocks();

        // Action
        bool success = blockDictionary.TryGetBlock(nameof(Block.Stone), out Block block);

        // Assert
        Assert.True(success);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void TryGetBlock_ValidStandardIdentityWithoutStandardRegistration_Failure()
    {
        // Action
        bool success = blockDictionary.TryGetBlock(nameof(Block.Stone), out Block _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void TryGetBlock_InvalidStandardIdentity_Failure()
    {
        // Setup
        blockDictionary.RegisterStandardBlocks();

        // Action
        bool success = blockDictionary.TryGetBlock("Invalid", out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void TryGetBlock_RegisteredBlock_Success()
    {
        // Action
        bool registerSuccess = blockDictionary.RegisterBlock("CustomBlockName", Block.Stone);
        bool tryGetSuccess = blockDictionary.TryGetBlock("CustomBlockName", out Block block);

        // Assert
        Assert.True(registerSuccess);
        Assert.True(tryGetSuccess);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void RegisterBlock_ValidBlock_Success()
    {
        // Action
        bool success = blockDictionary.RegisterBlock("CustomBlockName", Block.Stone);

        // Assert
        Assert.True(success);
    }

    [Fact]
    public void RegisterBlock_InvalidBlock_Failure()
    {
        // Action
        bool success = blockDictionary.RegisterBlock("CustomBlockName", (Block)200);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_EmptyIdentity_Failure()
    {
        // Action
        bool success = blockDictionary.RegisterBlock(string.Empty, Block.Stone);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_WhitespaceIdentity_Failure()
    {
        // Action
        bool success = blockDictionary.RegisterBlock(" ", Block.Stone);

        // Assert
        Assert.False(success);
    }
}
