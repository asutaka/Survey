using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface ICategoryRepo : IBaseRepo<Category>
    {
    }

    public class CategoryRepo : BaseRepo<Category>, ICategoryRepo
    {
        public CategoryRepo()
        {
        }
    }
}
