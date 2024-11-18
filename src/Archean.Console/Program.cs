using Archean.Application.Services;
using Archean.Application.Services.Networking;
using Archean.Core.Services;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ServiceProvider serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddScoped<IConnectionHandler, ConnectionHandler>()
    .AddScoped<IClientPacketReader, ClientPacketReader>()
    .AddScoped<IServerPacketWriter, ServerPacketWriter>()
    .AddScoped<IPacketDataReader, PacketDataReader>()
    .AddScoped<IPacketDataWriter, PacketDataWriter>()
    .AddScoped<IConnectionService, ConnectionService>()
    .AddScoped<IClientPacketHandler, ClientPacketHandler>()
    .AddSingleton<ISocketServer, SocketServer>()
    .AddSingleton<IConnectionRepository, ConnectionRepository>()
    .AddSingleton<IBlockDictionary, BlockDictionary>()
    .BuildServiceProvider();

ISocketServer server = serviceProvider.GetRequiredService<ISocketServer>();
await server.StartAsync();
server.ServerRunningCancellationToken.WaitHandle.WaitOne();
