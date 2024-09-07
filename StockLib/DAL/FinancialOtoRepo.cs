using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialOtoRepo : IBaseRepo<Financial_Oto>
    {
    }

    public class FinancialOtoRepo : BaseRepo<Financial_Oto>, IFinancialOtoRepo
    {
        public FinancialOtoRepo()
        {
        }
    }
}
