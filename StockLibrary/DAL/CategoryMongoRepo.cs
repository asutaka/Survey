using Microsoft.Extensions.Logging;
using StockLibrary.Model;

namespace StockLibrary.DAL
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
