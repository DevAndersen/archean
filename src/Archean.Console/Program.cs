using Archean.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"))
    .ConfigureLogging(l => l.AddConsole())
    .ConfigureServices(s => s.AddArcheanDefaultServices())
    .Build()
    .Run();
