using Archean.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.ConfigureArcheanDefaults();

IHost app = builder.Build();
app.Run();
