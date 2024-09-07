using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialHangKhongRepo : IBaseRepo<Financial_HangKhong>
    {
    }

    public class FinancialHangKhongRepo : BaseRepo<Financial_HangKhong>, IFinancialHangKhongRepo
    {
        public FinancialHangKhongRepo()
        {
        }
    }
}
