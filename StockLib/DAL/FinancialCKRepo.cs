using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialCKRepo : IBaseRepo<Financial_CK>
    {
    }

    public class FinancialCKRepo : BaseRepo<Financial_CK>, IFinancialCKRepo
    {
        public FinancialCKRepo()
        {
        }
    }
}
