using SLib.Model;

namespace SLib.DAL
{
    public interface IFinancialRepo : IMongoRepositoryBase<Financial>
    {

    }
    public class FinancialRepo : MongoBaseRepo<Financial>, IFinancialRepo
    {

    }
}
