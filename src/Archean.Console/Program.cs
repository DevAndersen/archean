using Archean.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"))
    .ConfigureLogging(l => l.AddConsole())
    .ConfigureServices((context, services) => services.AddArcheanDefaultServices(context.Configuration))
    .Build()
    .Run();
