using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IStockRepo : IBaseRepo<Stock>
    {
    }

    public class StockRepo : BaseRepo<Stock>, IStockRepo
    {
        public StockRepo()
        {
        }
    }
}
