using Microsoft.Extensions.DependencyInjection;

namespace StockLib.DAL.Settings
{
    public static class RegisterDAL
    {
        public static void DALDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IStockRepo, StockRepo>();
            services.AddSingleton<IStockFinancialRepo, StockFinancialRepo>();
            services.AddSingleton<IFinancialBDSRepo, FinancialBDSRepo>();
            services.AddSingleton<IFinancialNHRepo, FinancialNHRepo>();
            services.AddSingleton<IUserMessageRepo, UserMessageRepo>();
        }
    }
}
