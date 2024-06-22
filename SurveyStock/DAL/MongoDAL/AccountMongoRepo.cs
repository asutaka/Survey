using Microsoft.Extensions.Logging;
using SurveyStock.Model.MongoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.DAL.MongoDAL
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
