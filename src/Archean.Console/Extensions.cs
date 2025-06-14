﻿using Archean.Application;
using Archean.Application.Models.Commands;
using Archean.Application.Services;
using Archean.Application.Services.Commands;
using Archean.Application.Services.Events;
using Archean.Application.Services.Worlds;
using Archean.Core.Models.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Networking.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archean.Console;

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

        // Define default log levels, while allowing configurations to override these.
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
        });

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
