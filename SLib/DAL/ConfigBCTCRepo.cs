using SLib.Model;

namespace SLib.DAL
{
    public interface IConfigBCTCRepo : IMongoRepositoryBase<ConfigBCTC>
    {
    }

    public class ConfigBCTCRepo : MongoBaseRepo<ConfigBCTC>, IConfigBCTCRepo
    {
        public ConfigBCTCRepo()
        {
        }
    }
}
