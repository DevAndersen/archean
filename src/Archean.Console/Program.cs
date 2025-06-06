using Archean.Application;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.ConfigureArcheanDefaultServices();

IHost app = builder.Build();
app.Run();
