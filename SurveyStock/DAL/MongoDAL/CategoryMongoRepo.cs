using Microsoft.Extensions.Logging;
using SurveyStock.Model.MongoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.DAL.MongoDAL
{
    public interface ICategoryMongoRepo : IMongoRepositoryBase<Category>
    {
    }

    public class CategoryMongoRepo : MongoRepositoryBase<Category>, ICategoryMongoRepo
    {

        private readonly ILogger<CategoryMongoRepo> logger;

        public CategoryMongoRepo(ILogger<CategoryMongoRepo> logger)
        {
            this.logger = logger;
        }
    }
}
