using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialHoaChatRepo : IBaseRepo<Financial_HoaChat>
    {
    }

    public class FinancialHoaChatRepo : BaseRepo<Financial_HoaChat>, IFinancialHoaChatRepo
    {
        public FinancialHoaChatRepo()
        {
        }
    }
}
