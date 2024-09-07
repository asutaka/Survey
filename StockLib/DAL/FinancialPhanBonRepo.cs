using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialPhanBonRepo : IBaseRepo<Financial_PhanBon>
    {
    }

    public class FinancialPhanBonRepo : BaseRepo<Financial_PhanBon>, IFinancialPhanBonRepo
    {
        public FinancialPhanBonRepo()
        {
        }
    }
}
