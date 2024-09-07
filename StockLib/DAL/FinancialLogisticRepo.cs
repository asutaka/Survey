using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialLogisticRepo : IBaseRepo<Financial_Logistic>
    {
    }

    public class FinancialLogisticRepo : BaseRepo<Financial_Logistic>, IFinancialLogisticRepo
    {
        public FinancialLogisticRepo()
        {
        }
    }
}
