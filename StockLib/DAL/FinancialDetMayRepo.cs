using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialDetMayRepo : IBaseRepo<Financial_DetMay>
    {
    }

    public class FinancialDetMayRepo : BaseRepo<Financial_DetMay>, IFinancialDetMayRepo
    {
        public FinancialDetMayRepo()
        {
        }
    }
}
