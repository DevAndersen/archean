using Archean.Application;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ServiceProvider serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddArcheanDefaultServices()
    .BuildServiceProvider();

ISocketServer server = serviceProvider.GetRequiredService<ISocketServer>();
await server.StartAsync();
server.ServerRunningCancellationToken.WaitHandle.WaitOne();
