using Stock;
using StockLib.PublicService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddSLib();
    })
    .Build();

await host.RunAsync();
