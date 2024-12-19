using Archean.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder()
    .ConfigureLogging(l => l.AddConsole())
    .ConfigureServices(s => s.AddArcheanDefaultServices())
    .Build()
    .Run();
