using SLib.Model;

namespace SLib.DAL
{
    public interface IGoogleSheetRepo : IMongoRepositoryBase<GoogleSheet>
    {

    }
    public class GoogleSheetRepo : MongoBaseRepo<GoogleSheet>, IGoogleSheetRepo
    {

    }
}
