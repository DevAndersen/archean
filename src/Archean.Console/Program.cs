using Archean.Hosting;
using Microsoft.Extensions.Hosting;

Host.CreateApplicationBuilder()
    .ConfigureArcheanDefaults()
    .Build()
    .Run();
