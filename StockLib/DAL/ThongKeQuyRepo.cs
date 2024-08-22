using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IThongKeQuyRepo : IBaseRepo<ThongKeQuy>
    {
    }

    public class ThongKeQuyRepo : BaseRepo<ThongKeQuy>, IThongKeQuyRepo
    {
        public ThongKeQuyRepo()
        {
        }
    }
}
