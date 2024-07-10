using SLib.Model;

namespace SLib.DAL
{
    public interface IConfigDataRepo : IMongoRepositoryBase<ConfigData>
    {
    }

    public class ConfigDataRepo : MongoBaseRepo<ConfigData>, IConfigDataRepo
    {
        public ConfigDataRepo()
        {
        }
    }
}
