using Archean.Application.Services;
using Archean.Application.Services.Events;
using Archean.Application.Services.Networking;
using Archean.Application.Services.Worlds;
using Archean.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Archean.Application;

public static class Extensions
{
    /// <summary>
    /// Registers all default Archean services.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureArcheanDefaultServices(this IHostBuilder host)
    {
        return host.ConfigureServices((context, services) => services.RegisterArcheanDefaultServices(context.Configuration));
    }

    /// <summary>
    /// Registers all default Archean services.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterArcheanDefaultServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection

            // General
            .AddSingleton<ServerStartup>()

            // Settings
            .Configure<ServerSettings>(configuration.GetSection("Server"))
            .Configure<ChatSettings>(configuration.GetSection("Chat"))
            .Configure<AliasSettings>(configuration.GetSection("Aliases"))

            // Server and connection handling.
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<IConnectionHandler, ConnectionHandler>()
            .AddSingleton<IClientPacketReader, ClientPacketReader>()
            .AddSingleton<IServerPacketWriter, ServerPacketWriter>()
            .AddSingleton<IPacketDataReader, PacketDataReader>()
            .AddSingleton<IPacketDataWriter, PacketDataWriter>()
            .AddSingleton<IPlayerRegistry, PlayerRegistry>()

            // Events
            .AddScoped<IClientPacketHandler, ClientPacketHandler>()
            .AddScoped<IClientEventHandler, ClientEventHandler>()
            .AddScoped<IScopedEventBus, EventBus>()
            .AddSingleton<IGlobalEventBus, EventBus>()
            .AddScoped<IScopedEventListener, ScopedEventListener>()
            .AddSingleton<IGlobalEventListener, GlobalEventListener>()

            // Gameplay
            .AddScoped<IPlayerService, PlayerService>()
            .AddSingleton<IBlockDictionary, BlockDictionary>()

            // Worlds
            .AddSingleton<IWorldRegistry, WorldRegistry>()

            // Hosted service
            .AddHostedService<ArcheanHostedService>();
    }
}
