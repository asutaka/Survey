using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialThepRepo : IBaseRepo<Financial_Thep>
    {
    }

    public class FinancialThepRepo : BaseRepo<Financial_Thep>, IFinancialThepRepo
    {
        public FinancialThepRepo()
        {
        }
    }
}
