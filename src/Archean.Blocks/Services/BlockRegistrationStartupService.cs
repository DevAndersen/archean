using Archean.Core.Services;

namespace Archean.Blocks.Services;

public class BlockRegistrationStartupService : IStartupService
{
    private readonly IBlockDictionary _blockDictionary;

    public BlockRegistrationStartupService(IBlockDictionary blockDictionary)
    {
        _blockDictionary = blockDictionary;
    }

    public Task OnStartupAsync()
    {
        _blockDictionary.RegisterStandardBlocks();
        _blockDictionary.RegisterCustomAliases();

        return Task.CompletedTask;
    }
}
