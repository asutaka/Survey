using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialCangBienRepo : IBaseRepo<Financial_CangBien>
    {
    }

    public class FinancialCangBienRepo : BaseRepo<Financial_CangBien>, IFinancialCangBienRepo
    {
        public FinancialCangBienRepo()
        {
        }
    }
}
