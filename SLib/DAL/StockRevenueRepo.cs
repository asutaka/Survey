using SLib.Model;

namespace SLib.DAL
{
    public interface IStockRevenueRepo : IMongoRepositoryBase<StockRevenue>
    {
    }

    public class StockRevenueRepo : MongoBaseRepo<StockRevenue>, IStockRevenueRepo
    {
        public StockRevenueRepo()
        {
        }
    }
}
