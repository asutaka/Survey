using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialThuySanRepo : IBaseRepo<Financial_ThuySan>
    {
    }

    public class FinancialThuySanRepo : BaseRepo<Financial_ThuySan>, IFinancialThuySanRepo
    {
        public FinancialThuySanRepo()
        {
        }
    }
}
