using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IStockTypeRepo : IBaseRepo<StockType>
    {
    }

    public class StockTypeRepo : BaseRepo<StockType>, IStockTypeRepo
    {
        public StockTypeRepo()
        {
        }
    }
}
