using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IThongKeHaiQuanRepo : IBaseRepo<ThongKeHaiQuan>
    {
    }

    public class ThongKeHaiQuanRepo : BaseRepo<ThongKeHaiQuan>, IThongKeHaiQuanRepo
    {
        public ThongKeHaiQuanRepo()
        {
        }
    }
}
