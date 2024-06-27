using MongoDB.Driver;
using StockLibrary.Model;

namespace StockLibrary.DAL
{
    public interface IReportMongoRepo : IMongoRepositoryBase<Report>
    {
        void DeleteMany(FilterDefinition<Report> filter);
    }

    public class ReportMongoRepo : MongoRepositoryBase<Report>, IReportMongoRepo 
    {
        public void DeleteMany(FilterDefinition<Report> filter)
        {
            _collection.DeleteMany(filter);
        }
    }
}
