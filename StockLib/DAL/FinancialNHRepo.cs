using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialNHRepo : IBaseRepo<Financial_NH>
    {
    }

    public class FinancialNHRepo : BaseRepo<Financial_NH>, IFinancialNHRepo
    {
        public FinancialNHRepo()
        {
        }
    }
}
