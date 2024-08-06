using Microsoft.Extensions.DependencyInjection;

namespace StockLib.PublicService.Settings
{
    public static class RegisterPublicService
    {
        public static void PublicServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IStockService, StockService>();
            services.AddSingleton<IFinancialDataService, FinancialDataService>();
        }
    }
}
