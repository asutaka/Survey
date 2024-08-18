using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface ITuDoanhRepo : IBaseRepo<TuDoanh>
    {
    }

    public class TuDoanhRepo : BaseRepo<TuDoanh>, ITuDoanhRepo
    {}
}
