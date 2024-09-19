using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface ISpecialInfoRepo : IBaseRepo<SpecialInfo>
    {
    }

    public class SpecialInfoRepo : BaseRepo<SpecialInfo>, ISpecialInfoRepo
    {
        public SpecialInfoRepo()
        {
        }
    }
}
