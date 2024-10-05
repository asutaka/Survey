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
            services.AddSingleton<IFinancialCKRepo, FinancialCKRepo>();
            services.AddSingleton<IFinancialThepRepo, FinancialThepRepo>();
            services.AddSingleton<IFinancialBanLeRepo, FinancialBanLeRepo>();
            services.AddSingleton<IFinancialDienRepo, FinancialDienRepo>();
            services.AddSingleton<IFinancialOtoRepo, FinancialOtoRepo>();
            services.AddSingleton<IFinancialCangBienRepo, FinancialCangBienRepo>();
            services.AddSingleton<IFinancialPhanBonRepo, FinancialPhanBonRepo>();
            services.AddSingleton<IFinancialThanRepo, FinancialThanRepo>();
            services.AddSingleton<IFinancialThuySanRepo, FinancialThuySanRepo>();
            services.AddSingleton<IFinancialNhuaRepo, FinancialNhuaRepo>();
            services.AddSingleton<IFinancialXimangRepo, FinancialXimangRepo>();
            services.AddSingleton<IFinancialCaoSuRepo, FinancialCaoSuRepo>();
            services.AddSingleton<IFinancialDauTuCongRepo, FinancialDauTuCongRepo>();
            services.AddSingleton<IFinancialDetMayRepo, FinancialDetMayRepo>();
            services.AddSingleton<IFinancialGoRepo, FinancialGoRepo>();
            services.AddSingleton<IFinancialHangKhongRepo, FinancialHangKhongRepo>();
            services.AddSingleton<IFinancialLogisticRepo, FinancialLogisticRepo>();
            services.AddSingleton<IFinancialDauKhiRepo, FinancialDauKhiRepo>();
            services.AddSingleton<IConfigDataRepo, ConfigDataRepo>();
            services.AddSingleton<ICategoryRepo, CategoryRepo>();
            services.AddSingleton<IThongKeRepo, ThongKeRepo>();
            services.AddSingleton<IThongKeQuyRepo, ThongKeQuyRepo>();
            services.AddSingleton<IThongKeHaiQuanRepo, ThongKeHaiQuanRepo>();
            services.AddSingleton<IChiSoPERepo, ChiSoPERepo>();
            services.AddSingleton<IKeHoachRepo, KeHoachRepo>();
            services.AddSingleton<ISpecialInfoRepo, SpecialInfoRepo>();
            services.AddSingleton<IConfigBaoCaoPhanTichRepo, ConfigBaoCaoPhanTichRepo>();
        }
    }
}
