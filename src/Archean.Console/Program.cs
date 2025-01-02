using Archean.Application;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder()
    .ConfigureArcheanDefaultServices()
    .Build()
    .Run();
