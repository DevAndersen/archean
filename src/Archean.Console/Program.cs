using Archean.Application.Services.Networking;
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
    .AddSingleton<ISocketServer, SocketServer>()
    .AddSingleton<IConnectionRepository, ConnectionRepository>()
    .BuildServiceProvider();

ISocketServer server = serviceProvider.GetRequiredService<ISocketServer>();
await server.StartAsync();
server.ServerRunningCancellationToken.WaitHandle.WaitOne();
