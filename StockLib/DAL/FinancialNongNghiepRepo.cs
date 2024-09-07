using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IFinancialNongNghiepRepo : IBaseRepo<Financial_NongNghiep>
    {
    }

    public class FinancialNongNghiepRepo : BaseRepo<Financial_NongNghiep>, IFinancialNongNghiepRepo
    {
        public FinancialNongNghiepRepo()
        {
        }
    }
}
