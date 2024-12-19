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
            .AddScoped<IConnectionHandler, ConnectionHandler>()
            .AddScoped<IClientPacketReader, ClientPacketReader>()
            .AddScoped<IServerPacketWriter, ServerPacketWriter>()
            .AddScoped<IPacketDataReader, PacketDataReader>()
            .AddScoped<IPacketDataWriter, PacketDataWriter>()
            .AddScoped<IPlayerService, PlayerService>()
            .AddScoped<IClientPacketHandler, ClientPacketHandler>()
            .AddScoped<IEventListener, EventListener>()
            .AddScoped<IEventBus, EventBus>()
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<IPlayerRegistry, PlayerRegistry>()
            .AddSingleton<IGlobalEventBus, EventBus>()
            .AddSingleton<IBlockDictionary, BlockDictionary>()
            .AddHostedService<ArcheanHostedService>();
    }
}
