using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialThanRepo : IBaseRepo<Financial_Than>
    {
    }

    public class FinancialThanRepo : BaseRepo<Financial_Than>, IFinancialThanRepo
    {
        public FinancialThanRepo()
        {
        }
    }
}
