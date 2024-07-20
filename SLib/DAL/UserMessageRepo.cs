using SLib.Model;

namespace SLib.DAL
{ 
    public interface IUserMessageRepo : IMongoRepositoryBase<UserMessage>
    {
    }
    public class UserMessageRepo : MongoBaseRepo<UserMessage>, IUserMessageRepo
    {
        public UserMessageRepo() { }
    }
}
