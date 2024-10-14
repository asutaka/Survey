using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialRepo : IBaseRepo<Financial>
    {
    }

    public class FinancialRepo : BaseRepo<Financial>, IFinancialRepo
    {
        public FinancialRepo()
        {
        }
    }
}
