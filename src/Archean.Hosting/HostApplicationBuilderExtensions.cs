using Archean.Application.Models.Commands;
using Archean.Application.Services;
using Archean.Application.Services.Events;
using Archean.Application.Services.Worlds;
using Archean.Commands.Services;
using Archean.Core.Models.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Hosting.Services;
using Archean.Networking.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archean.Hosting;

public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Configures all Archean defaults.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureArcheanDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureLoggingDefaults();
        builder.ConfigureServiceDefaults();

        return builder;
    }

    /// <summary>
    /// Define default log levels, while allowing configurations to override these.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureLoggingDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
        });

        return builder;
    }

    /// <summary>
    /// Register a startup service.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddStartup<TService>(this IServiceCollection serviceCollection)
        where TService : class, IStartupService
    {
        return serviceCollection.AddSingleton<IStartupService, TService>();
    }

    public static IHostApplicationBuilder ConfigureServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder
            .ConfigureServiceDefaults<ServerSettings>("Server")
            .ConfigureServiceDefaults<ChatSettings>("Chat")
            .ConfigureServiceDefaults<AliasSettings>("Aliases");

        builder.Services

            // Startup
            .AddStartup<BlockRegistrationStartupService>()
            .AddStartup<CommandRegistrationStartupService>()

            // Server and connection handling.
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<IConnectionHandler, ConnectionHandler>()
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
            .AddSingleton<ICommandRegistry, CommandRegistry>()
            .RegisterCommand<TestCommand>()

            // Worlds
            .AddSingleton<IWorldRegistry, WorldRegistry>()

            // Hosted service
            .AddHostedService<ArcheanHostedService>();

        return builder;
    }

    public static IServiceCollection RegisterCommand<TCommand>(this IServiceCollection serviceCollection)
        where TCommand : class, ICommand
    {
        return serviceCollection
            .AddSingleton<ICommand, TCommand>()
            .AddTransient<TCommand>();
    }

    public static IHostApplicationBuilder ConfigureServiceDefaults<TConfiguration>(this IHostApplicationBuilder builder, string section)
        where TConfiguration : class
    {
        builder.Services.Configure<TConfiguration>(builder.Configuration.GetSection(section));

        return builder;
    }
}
