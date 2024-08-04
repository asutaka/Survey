using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IStockFinancialRepo : IBaseRepo<StockFinancial>
    {
    }

    public class StockFinancialRepo : BaseRepo<StockFinancial>, IStockFinancialRepo
    {
        public StockFinancialRepo()
        {
        }
    }
}
