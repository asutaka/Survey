using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IConfigMainRepo : IBaseRepo<ConfigMain>
    {
    }

    public class ConfigMainRepo : BaseRepo<ConfigMain>, IConfigMainRepo
    {
        public ConfigMainRepo()
        {
        }
    }
}
