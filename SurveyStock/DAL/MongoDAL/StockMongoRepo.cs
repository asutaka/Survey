using Microsoft.Extensions.Logging;
using SurveyStock.Model.MongoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.DAL.MongoDAL
{
    public interface IStockMongoRepo : IMongoRepositoryBase<Stock>
    {
    }

    public class StockMongoRepo : MongoRepositoryBase<Stock>, IStockMongoRepo
    {

        private readonly ILogger<StockMongoRepo> logger;

        public StockMongoRepo(ILogger<StockMongoRepo> logger)
        {
            this.logger = logger;
        }
    }
}
