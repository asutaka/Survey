using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface IUserMessageCoinRepo : IBaseRepo<UserMessage>
    {
    }
    public class UserMessageCoinRepo : BaseRepo<UserMessage>, IUserMessageCoinRepo
    {
        public UserMessageCoinRepo() { }
    }
}
