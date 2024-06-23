using Microsoft.Extensions.DependencyInjection;

namespace StockLibrary.DAL
{
    public static class DALDI
    {
        public static void DALDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAccountMongoRepo, AccountMongoRepo>();
            services.AddSingleton<ICategoryMongoRepo, CategoryMongoRepo>();
            services.AddSingleton<IStockMongoRepo, StockMongoRepo>();
            services.AddSingleton<ITuDoanhMongoRepo, TuDoanhMongoRepo>();
            services.AddSingleton<IForeignMongoRepo, ForeignMongoRepo>();
        }
    }
}
