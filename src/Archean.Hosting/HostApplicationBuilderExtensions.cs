using Archean.Application.Models.Commands;
using Archean.Blocks.Services;
using Archean.Commands.Services;
using Archean.Core.Models.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Events.Services;
using Archean.Hosting.Services;
using Archean.Networking.Services;
using Archean.Worlds.Services;
using Archean.Worlds.Services.Persistence;
using Archean.Worlds.Services.TerrainGenerators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Archean.Hosting;

/// <summary>
/// Contains extension methods for setting up Archean services and configurations.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Configures all Archean defaults.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static THostApplicationBuilder ConfigureArcheanDefaults<THostApplicationBuilder>(this THostApplicationBuilder builder)
        where THostApplicationBuilder : IHostApplicationBuilder
    {
        builder.ConfigureDefaultLogging();
        builder.RegisterDefaultServices();
        builder.RegisterDefaultCommands();

        return builder;
    }

    /// <summary>
    /// Configure default log levels, while allowing configurations to override these.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureDefaultLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder =>
        {
            // Logging levels
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));

            // Console logging formatting
            builder.Logging.AddConsole().AddConsoleFormatter<ArcheanConsoleLoggerFormatter, ConsoleFormatterOptions>();
            string formatterName = builder.Configuration.GetSection("Logging:Console:FormatterName").Value
                ?? nameof(ArcheanConsoleLoggerFormatter);
            loggingBuilder.AddConsole(x => x.FormatterName = formatterName);
        });

        return builder;
    }

    /// <summary>
    /// Register all default services.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterDefaultServices(this IHostApplicationBuilder builder)
    {
        builder
            .AddConfiguration<ServerSettings>("Server")
            .AddConfiguration<ChatSettings>("Chat")
            .AddConfiguration<AliasSettings>("Aliases")
            .AddConfiguration<WorldSettings>("Worlds");

        builder.Services

            // Startup
            .AddStartup<BlockRegistrationStartupService>()
            .AddStartup<CommandRegistrationStartupService>()
            .AddStartup<TerrainGeneratorStartupService>()
            .AddStartup<WorldRegistrationLifetimeService>()

            // Shutdown
            .AddShutdown<WorldRegistrationLifetimeService>()

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
            .AddSingleton<ICommandInvoker, CommandInvoker>()

            // Worlds
            .AddSingleton<IWorldRegistry, WorldRegistry>()
            .AddSingleton<WorldFactory>()
            .AddSingleton<BlockMapFactory>()
            .AddSingleton<WorldPersistenceHandler>()
            .AddSingleton<BlockMapPersistenceHandler>()

            // Terrain generation
            .AddSingleton<ITerrainGeneratorRegistry, TerrainGeneratorRegistry>()
            .AddTerrainGenerator<FlatTerrainGenerator>()
            .AddTerrainGenerator<EmptyTerrainGenerator>()

            // Hosted service
            .AddHostedService<ArcheanHostedService>();

        return builder;
    }

    /// <summary>
    /// Register all default commands.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterDefaultCommands(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddCommand<TestCommand>();

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

    /// <summary>
    /// Register a shutdown service.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddShutdown<TService>(this IServiceCollection serviceCollection)
        where TService : class, IShutdownService
    {
        return serviceCollection.AddSingleton<IShutdownService, TService>();
    }

    /// <summary>
    /// Register a command.
    /// </summary>
    /// <remarks>
    /// To new up a command for invocation, use the methods of <see cref="ICommandRegistry"/>.
    /// </remarks>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommand<TCommand>(this IServiceCollection serviceCollection)
        where TCommand : Command
    {
        return serviceCollection
            .AddSingleton<Command, TCommand>()
            .AddTransient<TCommand>();
    }

    /// <summary>
    /// Register a terrain generator.
    /// </summary>
    /// <remarks>
    /// To new up a command for invocation, use the methods of <see cref="ITerrainGeneratorRegistry"/>.
    /// </remarks>
    /// <typeparam name="TTerrainGenerator"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddTerrainGenerator<TTerrainGenerator>(this IServiceCollection serviceCollection)
        where TTerrainGenerator : class, ITerrainGenerator
    {
        return serviceCollection.AddSingleton<ITerrainGenerator, TTerrainGenerator>();
    }

    /// <summary>
    /// Register <typeparamref name="TConfiguration"/>, binding it to the specified configuration <paramref name="section"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Inject an instance of <see cref="IOptions{TOptions}"/> to access the configured values.</item>
    /// <item><paramref name="section"/> uses colon (":") to access nested elements, for example <c>"Category:SubCategory"</c>.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TConfiguration"></typeparam>
    /// <param name="builder"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddConfiguration<TConfiguration>(this IHostApplicationBuilder builder, string section)
        where TConfiguration : class
    {
        builder.Services.Configure<TConfiguration>(builder.Configuration.GetSection(section));

        return builder;
    }
}
