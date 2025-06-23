using Archean.Core.Models;
using Archean.Core.Services;
using Archean.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archean.Blocks.Services;

public class BlockDictionary : IBlockDictionary
{
    private readonly ILogger<BlockDictionary> _logger;
    private readonly AliasSettings _aliasSettings;

    private readonly Dictionary<string, Block> _dictionary;

    public BlockDictionary(ILogger<BlockDictionary> logger, IOptions<AliasSettings> aliasSettingsOptions)
    {
        _logger = logger;
        _aliasSettings = aliasSettingsOptions.Value;

        _dictionary = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);
    }

    public int RegisterStandardBlocks()
    {
        int registeredAliases = 0;

        // Register all defined Block values by their ID and name.
        foreach (Block block in Enum.GetValues<Block>())
        {
            if (_aliasSettings.RegisterDefaultIdAliases && RegisterBlock(((byte)block).ToString(), block))
            {
                registeredAliases++;
            }

            if (_aliasSettings.RegisterDefaultNameAliases && RegisterBlock(block.ToString(), block))
            {
                registeredAliases++;
            }
        }

        _logger.LogDebug("Registered {dictionarySize} standard block aliases", registeredAliases);
        return registeredAliases;
    }

    public int RegisterCustomAliases()
    {
        if (_aliasSettings.CustomAliases == null)
        {
            return 0;
        }

        int registeredAliases = 0;
        Block[] definedBlocks = Enum.GetValues<Block>();

        foreach (KeyValuePair<string, string[]> kvp in _aliasSettings.CustomAliases)
        {
            if (Enum.TryParse(kvp.Key, out Block parsedBlock) && definedBlocks.Contains(parsedBlock))
            {
                foreach (string alias in kvp.Value)
                {
                    if (RegisterBlock(alias, parsedBlock))
                    {
                        registeredAliases++;
                    }
                }
            }
        }

        _logger.LogDebug("Registered {dictionarySize} custom block aliases", registeredAliases);
        return registeredAliases;
    }

    public bool RegisterBlock(string alias, Block block)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            _logger.LogWarning("Failed to register empty or whitespace alias for block {block}",
                block);

            return false;
        }

        if (!Enum.IsDefined(block))
        {
            _logger.LogWarning("Failed to register alias {alias} for undefined block {block}",
                alias,
                block);

            return false;
        }

        if (_dictionary.ContainsKey(alias))
        {
            _logger.LogWarning("Failed to register alias {alias} for block {block}, alias already in use",
                alias,
                block);

            return false;
        }

        _logger.LogTrace("Succesfully registered alias {alias} for block {block}",
            alias,
            block);

        _dictionary[alias] = block;
        return true;
    }

    public bool TryGetBlock(string identity, out Block block)
    {
        return _dictionary.TryGetValue(identity, out block);
    }
}
