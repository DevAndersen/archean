using Archean.Application.Services.Networking;
using Archean.Application.Services;
using Archean.Core.Services.Networking;
using Archean.Core.Services;
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
            .AddScoped<IConnectionService, ConnectionService>()
            .AddScoped<IClientPacketHandler, ClientPacketHandler>()
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<IConnectionRepository, ConnectionRepository>()
            .AddSingleton<IBlockDictionary, BlockDictionary>();
    }
}
