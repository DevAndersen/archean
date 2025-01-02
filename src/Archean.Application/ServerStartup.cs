using Archean.Core.Services;

namespace Archean.Application;

public class ServerStartup
{
    private readonly IBlockDictionary blockDictionary;

    public ServerStartup(IBlockDictionary blockDictionary)
    {
        this.blockDictionary = blockDictionary;
    }

    /// <summary>
    /// Invokes setup operations to perpare services.
    /// </summary>
    public void PerformSetup()
    {
        blockDictionary.RegisterStandardBlocks();
        blockDictionary.RegisterCustomAliases();
    }
}
