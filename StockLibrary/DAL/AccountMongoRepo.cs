using Microsoft.Extensions.Logging;
using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface IAccountMongoRepo : IMongoRepositoryBase<Account>
    {
    }

    public class AccountMongoRepo : MongoRepositoryBase<Account>, IAccountMongoRepo
    {
        public AccountMongoRepo()
        {}
    }
}
