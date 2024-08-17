using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialBanLeRepo : IBaseRepo<Financial_BanLe>
    {
    }

    public class FinancialBanLeRepo : BaseRepo<Financial_BanLe>, IFinancialBanLeRepo
    {
        public FinancialBanLeRepo()
        {
        }
    }
}
