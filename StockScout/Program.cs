using StockScout;
using SLib.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddSLib();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
