using Microsoft.Extensions.Logging;
using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface IStockMongoRepo : IMongoRepositoryBase<Stock>
    {
    }

    public class StockMongoRepo : MongoRepositoryBase<Stock>, IStockMongoRepo
    {
        public StockMongoRepo()
        {
        }
    }
}
