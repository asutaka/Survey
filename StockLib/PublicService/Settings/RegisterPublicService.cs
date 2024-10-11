using Microsoft.Extensions.DependencyInjection;

namespace StockLib.PublicService.Settings
{
    public static class RegisterPublicService
    {
        public static void PublicServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IFinancialDataService, FinancialDataService>();
            services.AddSingleton<IImportDataAPIService, ImportDataAPIService>();
            services.AddSingleton<ITeleStockService, TeleStockService>();
            services.AddSingleton<IAnalyzeStockService, AnalyzeStockService>();
            services.AddSingleton<IStockTestCaseService, StockTestCaseService>();
            services.AddSingleton<IAnalyzeCoinService, AnalyzeCoinService>();
        }
    }
}
