using SLib.Model;

namespace SLib.DAL
{
    public interface ICategoryRepo : IMongoRepositoryBase<Category>
    {
    }

    public class CategoryRepo : MongoBaseRepo<Category>, ICategoryRepo
    {
        public CategoryRepo()
        {
        }
    }
}
