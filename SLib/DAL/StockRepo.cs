using SLib.Model;

namespace SLib.DAL
{
    public interface IStockRepo : IMongoRepositoryBase<Stock>
    {
    }

    public class StockRepo : MongoBaseRepo<Stock>, IStockRepo
    {
        public StockRepo()
        {
        }
    }
}
