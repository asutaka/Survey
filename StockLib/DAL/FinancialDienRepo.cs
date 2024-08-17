using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialDienRepo : IBaseRepo<Financial_Dien>
    {
    }

    public class FinancialDienRepo : BaseRepo<Financial_Dien>, IFinancialDienRepo
    {
        public FinancialDienRepo()
        {
        }
    }
}
