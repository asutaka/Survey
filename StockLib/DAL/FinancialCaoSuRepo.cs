using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialCaoSuRepo : IBaseRepo<Financial_CaoSu>
    {
    }

    public class FinancialCaoSuRepo : BaseRepo<Financial_CaoSu>, IFinancialCaoSuRepo
    {
        public FinancialCaoSuRepo()
        {
        }
    }
}
