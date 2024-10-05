using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IConfigBaoCaoPhanTichRepo : IBaseRepo<ConfigBaoCaoPhanTich>
    {
    }

    public class ConfigBaoCaoPhanTichRepo : BaseRepo<ConfigBaoCaoPhanTich>, IConfigBaoCaoPhanTichRepo
    {
        public ConfigBaoCaoPhanTichRepo()
        {
        }
    }
}
