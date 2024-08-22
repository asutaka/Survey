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
            services.AddSingleton<IConfigMainRepo, ConfigMainRepo>();
            services.AddSingleton<IFinancialCKRepo, FinancialCKRepo>();
            services.AddSingleton<IFinancialThepRepo, FinancialThepRepo>();
            services.AddSingleton<IFinancialBanLeRepo, FinancialBanLeRepo>();
            services.AddSingleton<IFinancialDienRepo, FinancialDienRepo>();
            services.AddSingleton<IConfigDataRepo, ConfigDataRepo>();
            services.AddSingleton<ICategoryRepo, CategoryRepo>();
            services.AddSingleton<ITuDoanhRepo, TuDoanhRepo>();
            services.AddSingleton<IThongKeRepo, ThongKeRepo>();
            services.AddSingleton<IThongKeQuyRepo, ThongKeQuyRepo>();
        }
    }
}
