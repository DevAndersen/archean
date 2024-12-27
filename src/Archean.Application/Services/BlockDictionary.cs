using Archean.Core.Services;

namespace Archean.Application.Services;

public class BlockDictionary : IBlockDictionary
{
    private readonly ILogger<BlockDictionary> logger;
    private readonly Dictionary<string, Block> dictionary;

    public BlockDictionary(ILogger<BlockDictionary> logger)
    {
        this.logger = logger;
        dictionary = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);
    }

    public void RegisterStandardBlocks()
    {
        // Register all defined Block values by their ID and name.
        foreach (Block block in Enum.GetValues<Block>())
        {
            RegisterBlock(((byte)block).ToString(), block);
            RegisterBlock(block.ToString(), block);
        }

        logger.LogTrace("Registered {dictionarySize} standard block aliases", dictionary.Count);
    }

    public bool RegisterBlock(string identity, Block block)
    {
        if (string.IsNullOrWhiteSpace(identity))
        {
            logger.LogWarning("Failed to register empty or whitespace block alias for block ID {blockId}",
                (int)block);

            return false;
        }

        if (Enum.IsDefined(block))
        {
            logger.LogTrace("Registered block alias {blockIdentity} for block ID {blockId}",
                identity,
                (int)block);

            dictionary[identity] = block;
            return true;
        }
        else
        {
            logger.LogWarning("Failed to register block alias {blockIdentity} for undefined block ID {blockId}",
                identity,
                (int)block);

            return false;
        }
    }

    public bool TryGetBlock(string identity, out Block block)
    {
        return dictionary.TryGetValue(identity, out block);
    }
}
