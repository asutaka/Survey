using Microsoft.Extensions.Logging;
using SurveyStock.Model.MongoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.DAL.MongoDAL
{
    public interface ITransactionMongoRepo : IMongoRepositoryBase<Transaction>
    {
    }

    public class TransactionMongoRepo : MongoRepositoryBase<Transaction>, ITransactionMongoRepo
    {

        private readonly ILogger<TransactionMongoRepo> logger;

        public TransactionMongoRepo(ILogger<TransactionMongoRepo> logger)
        {
            this.logger = logger;
        }
    }
}
