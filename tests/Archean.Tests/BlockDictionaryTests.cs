using Archean.Blocks.Services;
using Archean.Core.Models;
using Archean.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Archean.Tests;

public class BlockDictionaryTests
{
    private readonly BlockDictionary _blockDictionary;
    private readonly ILogger<BlockDictionary> _logger;

    public BlockDictionaryTests()
    {
        _logger = Substitute.For<ILogger<BlockDictionary>>();

        IOptions<AliasSettings> aliasSettingsOptions = Substitute.For<IOptions<AliasSettings>>();
        aliasSettingsOptions.Value.Returns(new AliasSettings
        {
            RegisterDefaultIdAliases = true,
            RegisterDefaultNameAliases = true,
            CustomAliases = []
        });

        _blockDictionary = new BlockDictionary(_logger, aliasSettingsOptions);
    }

    [Fact]
    public void TryGetBlock_ValidStandardIdentity_ExpectedValue()
    {
        // Setup
        _blockDictionary.RegisterStandardBlocks();

        // Action
        bool success = _blockDictionary.TryGetBlock(nameof(Block.Stone), out Block block);

        // Assert
        Assert.True(success);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void TryGetBlock_ValidStandardIdentityWithoutStandardRegistration_Failure()
    {
        // Action
        bool success = _blockDictionary.TryGetBlock(nameof(Block.Stone), out Block _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void TryGetBlock_InvalidStandardIdentity_Failure()
    {
        // Setup
        _blockDictionary.RegisterStandardBlocks();

        // Action
        bool success = _blockDictionary.TryGetBlock("Invalid", out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void TryGetBlock_RegisteredBlock_Success()
    {
        // Action
        bool registerSuccess = _blockDictionary.RegisterBlock("CustomBlockName", Block.Stone);
        bool tryGetSuccess = _blockDictionary.TryGetBlock("CustomBlockName", out Block block);

        // Assert
        Assert.True(registerSuccess);
        Assert.True(tryGetSuccess);
        Assert.Equal(Block.Stone, block);
    }

    [Fact]
    public void RegisterBlock_ValidBlock_Success()
    {
        // Action
        bool success = _blockDictionary.RegisterBlock("CustomBlockName", Block.Stone);

        // Assert
        Assert.True(success);
    }

    [Fact]
    public void RegisterBlock_InvalidBlock_Failure()
    {
        // Action
        bool success = _blockDictionary.RegisterBlock("CustomBlockName", (Block)200);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_EmptyIdentity_Failure()
    {
        // Action
        bool success = _blockDictionary.RegisterBlock(string.Empty, Block.Stone);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void RegisterBlock_WhitespaceIdentity_Failure()
    {
        // Action
        bool success = _blockDictionary.RegisterBlock(" ", Block.Stone);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void RegisterCustomAliases_ValidCustomAliases_Success()
    {
        // Setup
        IOptions<AliasSettings> aliasSettingsOptions = Substitute.For<IOptions<AliasSettings>>();
        aliasSettingsOptions.Value.Returns(new AliasSettings
        {
            RegisterDefaultIdAliases = true,
            RegisterDefaultNameAliases = true,
            CustomAliases = new Dictionary<string, string[]>
            {
                [nameof(Block.Air)] = ["SomeName"],
                [nameof(Block.Stone)] = ["SomeOtherName"]
            }
        });

        BlockDictionary blockDictionary = new BlockDictionary(_logger, aliasSettingsOptions);

        // Action
        int registeredAliases = blockDictionary.RegisterCustomAliases();

        // Assert
        Assert.Equal(2, registeredAliases);
    }

    [Fact]
    public void RegisterCustomAliases_DuplicateCustomAliases_OnlyOneRegistered()
    {
        // Setup
        IOptions<AliasSettings> aliasSettingsOptions = Substitute.For<IOptions<AliasSettings>>();
        aliasSettingsOptions.Value.Returns(new AliasSettings
        {
            RegisterDefaultIdAliases = true,
            RegisterDefaultNameAliases = true,
            CustomAliases = new Dictionary<string, string[]>
            {
                [nameof(Block.Air)] = ["SomeName"],
                [nameof(Block.Stone)] = ["SomeName"]
            }
        });

        BlockDictionary blockDictionary = new BlockDictionary(_logger, aliasSettingsOptions);

        // Action
        int registeredAliases = blockDictionary.RegisterCustomAliases();

        // Assert
        Assert.Equal(1, registeredAliases);
    }
}
