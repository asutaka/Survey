using Microsoft.Extensions.Logging;
using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface IAccountMongoRepo : IMongoRepositoryBase<Account>
    {
    }

    public class AccountMongoRepo : MongoRepositoryBase<Account>, IAccountMongoRepo
    {

        private readonly ILogger<AccountMongoRepo> logger;

        public AccountMongoRepo(ILogger<AccountMongoRepo> logger)
        {
            this.logger = logger;
        }
    }
}
