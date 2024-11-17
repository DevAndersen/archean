using Archean.Application.Services;
using Archean.Core.Models;
using Microsoft.Extensions.Logging;

namespace Archean.Tests;

public class BlockDictionaryTests
{
    private readonly BlockDictionary dictionary;

    public BlockDictionaryTests()
    {
        ILogger<BlockDictionary> logger = NSubstitute.Substitute.For<ILogger<BlockDictionary>>();
        dictionary = new BlockDictionary(logger);
    }

    [Fact]
    public void TryGetBlock_ValidIdentity_ExpectedValue()
    {
        bool success = dictionary.TryGetBlock("Stone", out Block block);

        Assert.True(success);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void TryGetBlock_InvalidIdentity_Failure()
    {
        bool success = dictionary.TryGetBlock("Invalid", out _);

        Assert.False(success);
    }

    [Fact]
    public void TryGetBlock_RegisteredBlock_Success()
    {
        bool registerSuccess = dictionary.RegisterBlock("Rock", Block.Stone);
        bool tryGetSuccess = dictionary.TryGetBlock("Rock", out Block block);

        Assert.True(registerSuccess);
        Assert.True(tryGetSuccess);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void RegisterBlock_ValidBlock_Success()
    {
        bool success = dictionary.RegisterBlock("Rock", Block.Stone);

        Assert.True(success);
    }

    [Fact]
    public void RegisterBlock_InvalidBlock_Failure()
    {
        bool success = dictionary.RegisterBlock("Rock", (Block)200);

        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_EmptyIdentity_Failure()
    {
        bool success = dictionary.RegisterBlock(string.Empty, Block.Stone);

        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_WhitespaceIdentity_Failure()
    {
        bool success = dictionary.RegisterBlock(" ", Block.Stone);

        Assert.False(success);
    }
}
