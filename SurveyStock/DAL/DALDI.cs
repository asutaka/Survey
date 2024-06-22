using Microsoft.Extensions.DependencyInjection;
using SurveyStock.DAL.MongoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.DAL
{
    public static class DALDI
    {
        public static void DALDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAccountMongoRepo, AccountMongoRepo>();
            services.AddSingleton<ICategoryMongoRepo, CategoryMongoRepo>();
            services.AddSingleton<IStockMongoRepo, StockMongoRepo>();
            services.AddSingleton<ITransactionMongoRepo, TransactionMongoRepo>();
        }
    }
}
