using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialGoRepo : IBaseRepo<Financial_Go>
    {
    }

    public class FinancialGoRepo : BaseRepo<Financial_Go>, IFinancialGoRepo
    {
        public FinancialGoRepo()
        {
        }
    }
}
