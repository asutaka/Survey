using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IConfigDataRepo : IBaseRepo<ConfigData>
    {
    }

    public class ConfigDataRepo : BaseRepo<ConfigData>, IConfigDataRepo
    {
        public ConfigDataRepo()
        {
        }
    }
}
