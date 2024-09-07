using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialDauTuCongRepo : IBaseRepo<Financial_DauTuCong>
    {
    }

    public class FinancialDauTuCongRepo : BaseRepo<Financial_DauTuCong>, IFinancialDauTuCongRepo
    {
        public FinancialDauTuCongRepo()
        {
        }
    }
}
