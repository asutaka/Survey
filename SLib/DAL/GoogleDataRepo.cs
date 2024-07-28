using SLib.Model;

namespace SLib.DAL
{
    public interface IGoogleDataRepo : IMongoRepositoryBase<GoogleData>
    {

    }
    public class GoogleDataRepo : MongoBaseRepo<GoogleData>, IGoogleDataRepo
    {

    }
}
