using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialXimangRepo : IBaseRepo<Financial_Ximang>
    {
    }

    public class FinancialXimangRepo : BaseRepo<Financial_Ximang>, IFinancialXimangRepo
    {
        public FinancialXimangRepo()
        {
        }
    }
}
