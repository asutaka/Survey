using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialBDSRepo : IBaseRepo<Financial_BDS>
    {
    }

    public class FinancialBDSRepo : BaseRepo<Financial_BDS>, IFinancialBDSRepo
    {
        public FinancialBDSRepo()
        {
        }
    }
}
