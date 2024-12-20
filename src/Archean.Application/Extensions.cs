using Archean.Application.Services;
using Archean.Application.Services.Events;
using Archean.Application.Services.Networking;
using Archean.Core.Services;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace Archean.Application;

public static class Extensions
{
    /// <summary>
    /// Adds all default Archean services.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddArcheanDefaultServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection

            // Server and connection handling.
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<IConnectionHandler, ConnectionHandler>()
            .AddSingleton<IClientPacketReader, ClientPacketReader>()
            .AddSingleton<IServerPacketWriter, ServerPacketWriter>()
            .AddSingleton<IPacketDataReader, PacketDataReader>()
            .AddSingleton<IPacketDataWriter, PacketDataWriter>()
            .AddSingleton<IPlayerRegistry, PlayerRegistry>()
            .AddScoped<IClientPacketHandler, ClientPacketHandler>()

            // Events
            .AddScoped<IEventBus, EventBus>()
            .AddScoped<IEventListener, EventListener>()
            .AddSingleton<IGlobalEventBus, EventBus>()

            // Gameplay
            .AddScoped<IPlayerService, PlayerService>()
            .AddSingleton<IBlockDictionary, BlockDictionary>()

            // Hosted service
            .AddHostedService<ArcheanHostedService>();
    }
}
