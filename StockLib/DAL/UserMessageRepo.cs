using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IUserMessageRepo : IBaseRepo<UserMessage>
    {
    }
    public class UserMessageRepo : BaseRepo<UserMessage>, IUserMessageRepo
    {
        public UserMessageRepo() { }
    }
}
