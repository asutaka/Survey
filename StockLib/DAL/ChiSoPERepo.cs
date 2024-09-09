using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IChiSoPERepo : IBaseRepo<ChiSoPE>
    {
    }

    public class ChiSoPERepo : BaseRepo<ChiSoPE>, IChiSoPERepo
    {
        public ChiSoPERepo()
        {
        }
    }
}
