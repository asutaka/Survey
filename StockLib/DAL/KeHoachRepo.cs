using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IKeHoachRepo : IBaseRepo<KeHoach>
    {
    }

    public class KeHoachRepo : BaseRepo<KeHoach>, IKeHoachRepo
    {
        public KeHoachRepo()
        {
        }
    }
}
