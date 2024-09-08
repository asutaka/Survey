using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialDauKhiRepo : IBaseRepo<Financial_DauKhi>
    {
    }

    public class FinancialDauKhiRepo : BaseRepo<Financial_DauKhi>, IFinancialDauKhiRepo
    {
        public FinancialDauKhiRepo()
        {
        }
    }
}
