using Microsoft.Extensions.Logging;
using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface ICategoryMongoRepo : IMongoRepositoryBase<Category>
    {
    }

    public class CategoryMongoRepo : MongoRepositoryBase<Category>, ICategoryMongoRepo
    {
        public CategoryMongoRepo()
        {
        }
    }
}
