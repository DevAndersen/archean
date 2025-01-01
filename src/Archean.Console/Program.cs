using Archean.Application;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => services.AddArcheanDefaultServices(context.Configuration))
    .Build()
    .Run();
