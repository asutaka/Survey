using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IShareRepo : IBaseRepo<Share>
    {
    }

    public class ShareRepo : BaseRepo<Share>, IShareRepo
    {
        public ShareRepo()
        {
        }
    }
}
