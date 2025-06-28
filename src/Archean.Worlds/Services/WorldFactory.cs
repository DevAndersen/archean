using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Settings;
using Archean.Worlds.Models;
using Archean.Worlds.Services.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archean.Worlds.Services;

public class WorldFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WorldFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IWorld CreateNewWorld(string name)
    {
        return new TestWorld(
            name,
            _serviceProvider.GetRequiredService<ILogger<TestWorld>>(),
            _serviceProvider.GetRequiredService<IGlobalEventListener>(),
            _serviceProvider.GetRequiredService<IOptions<ServerSettings>>().Value,
            _serviceProvider.GetRequiredService<WorldPersistenceHandler>());
    }
}
