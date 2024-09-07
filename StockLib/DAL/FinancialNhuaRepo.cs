using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialNhuaRepo : IBaseRepo<Financial_Nhua>
    {
    }

    public class FinancialNhuaRepo : BaseRepo<Financial_Nhua>, IFinancialNhuaRepo
    {
        public FinancialNhuaRepo()
        {
        }
    }
}
