using StockScout;
using SLib.Service;
using SLib.Google;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddSLib();
        services.AddSingleton(typeof(GoogleSheetsHelper));
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
