using Microsoft.Extensions.DependencyInjection;

namespace StockLib.DAL.Settings
{
    public static class RegisterDAL
    {
        public static void DALDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IStockRepo, StockRepo>();
            services.AddSingleton<IFinancialRepo, FinancialRepo>();
            services.AddSingleton<IFinancialBDSRepo, FinancialBDSRepo>();
            services.AddSingleton<IFinancialNHRepo, FinancialNHRepo>();
            services.AddSingleton<IUserMessageRepo, UserMessageRepo>();
            services.AddSingleton<IFinancialCKRepo, FinancialCKRepo>();
            services.AddSingleton<IFinancialThuySanRepo, FinancialThuySanRepo>();
            services.AddSingleton<IConfigDataRepo, ConfigDataRepo>();
            services.AddSingleton<ICategoryRepo, CategoryRepo>();
            services.AddSingleton<IThongKeRepo, ThongKeRepo>();
            services.AddSingleton<IThongKeQuyRepo, ThongKeQuyRepo>();
            services.AddSingleton<IThongKeHaiQuanRepo, ThongKeHaiQuanRepo>();
            services.AddSingleton<IChiSoPERepo, ChiSoPERepo>();
            services.AddSingleton<IKeHoachRepo, KeHoachRepo>();
            services.AddSingleton<ISpecialInfoRepo, SpecialInfoRepo>();
            services.AddSingleton<IConfigBaoCaoPhanTichRepo, ConfigBaoCaoPhanTichRepo>();
            services.AddSingleton<ICoinRepo, CoinRepo>();
        }
    }
}
