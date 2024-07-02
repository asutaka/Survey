using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface IConfigDataMongoRepo : IMongoRepositoryBase<ConfigData>
    {
    }

    public class ConfigDataMongoRepo : MongoRepositoryBase<ConfigData>, IConfigDataMongoRepo
    {
        public ConfigDataMongoRepo()
        {
        }
    }
}
