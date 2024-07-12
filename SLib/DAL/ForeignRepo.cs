using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SLib.Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SLib.DAL
{ 
    public interface IForeignRepo : IMongoRepositoryBase<Foreign>
    {
        List<Foreign> GetWithFilter(int offset, int limit, string code, long date);
    }
    public class ForeignRepo : MongoBaseRepo<Foreign>, IForeignRepo
    {
        public ForeignRepo() { }
        public List<Foreign> GetWithFilter(int offset, int limit, string code, long date)
        {
            try
            {
                FilterDefinition<Foreign> filter = null;
                var builder = Builders<Foreign>.Filter;
                var lFilter = new List<FilterDefinition<Foreign>>();
                if (string.IsNullOrWhiteSpace(code))
                    return null;

                lFilter.Add(builder.Eq(x => x.s, code));
                lFilter.Add(builder.Eq(x => x.d, date));
                foreach (var item in lFilter)
                {
                    if (filter is null)
                    {
                        filter = item;
                        continue;
                    }
                    filter &= item;
                }

                if (filter is null)
                    return null;

                return _collection.Find(filter)
                        .Skip((offset - 1) * limit)
                        .Limit(limit).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ForeignRepo.GetWithFilterAsync|EXCEPTION| {ex.Message}");
            }

            return null;
        }
    }
}
