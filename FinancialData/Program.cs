using FinancialData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockLib.PublicService;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddHttpClient();
            services.AddSLib();
            services.AddSingleton<App>();
        })
        .ConfigureAppConfiguration(app =>
        { });
}