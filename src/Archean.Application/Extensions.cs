using Archean.Application.Models.Commands;
using Archean.Application.Services;
using Archean.Application.Services.Commands;
using Archean.Application.Services.Events;
using Archean.Application.Services.Networking;
using Archean.Application.Services.Worlds;
using Archean.Core.Models.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Archean.Application;

public static class Extensions
{
    /// <summary>
    /// Registers all default Archean services.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureArcheanDefaultServices(this IHostApplicationBuilder builder)
    {
        CommandRegistrations commandRegistrations = new CommandRegistrations();

        builder.Services

            // General
            .AddSingleton<ServerStartup>()
            .AddSingleton(commandRegistrations)

            // Settings
            .Configure<ServerSettings>(builder.Configuration.GetSection("Server"))
            .Configure<ChatSettings>(builder.Configuration.GetSection("Chat"))
            .Configure<AliasSettings>(builder.Configuration.GetSection("Aliases"))

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

            // Commands
            .AddSingleton<ICommandDictionary, CommandDictionary>()
            .RegisterCommand<TestCommand>(commandRegistrations)

            // Worlds
            .AddSingleton<IWorldRegistry, WorldRegistry>()

            // Hosted service
            .AddHostedService<ArcheanHostedService>();

        return builder;
    }

    public static IServiceCollection RegisterCommand<TCommand>(this IServiceCollection serviceCollection, CommandRegistrations commandRegistrations)
        where TCommand : class, ICommand
    {
        commandRegistrations.RegisterCommand<TCommand>();
        return serviceCollection
            .AddTransient<TCommand>();
    }
}
