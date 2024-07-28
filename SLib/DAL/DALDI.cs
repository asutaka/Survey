using Microsoft.Extensions.DependencyInjection;

namespace SLib.DAL
{
    public static class DALDI
    {
        public static void DALDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IStockRepo, StockRepo>();
            services.AddSingleton<ITuDoanhRepo, TuDoanhRepo>();
            services.AddSingleton<IConfigDataRepo, ConfigDataRepo>();
            services.AddSingleton<IUserMessageRepo, UserMessageRepo>();
            services.AddSingleton<ICategoryRepo, CategoryRepo>();
            services.AddSingleton<IFinancialRepo, FinancialRepo>();
            services.AddSingleton<IGoogleSheetRepo, GoogleSheetRepo>();
            services.AddSingleton<IConfigBCTCRepo, ConfigBCTCRepo>();
            services.AddSingleton<IGoogleDataRepo, GoogleDataRepo>();
        }
    }
}
