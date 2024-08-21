using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IThongKeRepo : IBaseRepo<ThongKe>
    {
    }

    public class ThongKeRepo : BaseRepo<ThongKe>, IThongKeRepo
    {
        public ThongKeRepo()
        {
        }
    }
}
