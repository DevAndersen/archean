using Archean.Core.Services;

namespace Archean.Application.Services;

public class BlockDictionary : IBlockDictionary
{
    private readonly ILogger<BlockDictionary> logger;
    private readonly AliasSettings aliasSettings;

    private readonly Dictionary<string, Block> dictionary;

    public BlockDictionary(ILogger<BlockDictionary> logger, IOptions<AliasSettings> aliasSettingsOptions)
    {
        this.logger = logger;
        aliasSettings = aliasSettingsOptions.Value;

        dictionary = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);
    }

    public int RegisterStandardBlocks()
    {
        int registeredAliases = 0;

        // Register all defined Block values by their ID and name.
        foreach (Block block in Enum.GetValues<Block>())
        {
            if (aliasSettings.RegisterDefaultIdAliases)
            {
                if (RegisterBlock(((byte)block).ToString(), block))
                {
                    registeredAliases++;
                }
            }

            if (aliasSettings.RegisterDefaultNameAliases)
            {
                if (RegisterBlock(block.ToString(), block))
                {
                    registeredAliases++;
                }
            }
        }

        logger.LogDebug("Registered {dictionarySize} standard block aliases", registeredAliases);
        return registeredAliases;
    }

    public int RegisterCustomAliases()
    {
        if (aliasSettings.CustomAliases == null)
        {
            return 0;
        }

        int registeredAliases = 0;
        Block[] definedBlocks = Enum.GetValues<Block>();

        foreach (KeyValuePair<string, string[]> kvp in aliasSettings.CustomAliases)
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

        logger.LogDebug("Registered {dictionarySize} custom block aliases", registeredAliases);
        return registeredAliases;
    }

    public bool RegisterBlock(string alias, Block block)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            logger.LogWarning("Failed to register empty or whitespace alias for block {block}",
                block);

            return false;
        }

        if (!Enum.IsDefined(block))
        {
            logger.LogWarning("Failed to register alias {alias} for undefined block {block}",
                alias,
                block);

            return false;
        }

        if (dictionary.ContainsKey(alias))
        {
            logger.LogWarning("Failed to register alias {alias} for block {block}, alias already in use",
                alias,
                block);

            return false;
        }

        logger.LogTrace("Succesfully registered alias {alias} for block {block}",
            alias,
            block);

        dictionary[alias] = block;
        return true;
    }

    public bool TryGetBlock(string identity, out Block block)
    {
        return dictionary.TryGetValue(identity, out block);
    }
}
