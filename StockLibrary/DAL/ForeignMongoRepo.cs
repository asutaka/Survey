using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockLibrary.DAL
{
    public interface IForeignMongoRepo : IMongoRepositoryBase<Foreign>
    {
        List<Foreign> GetWithFilter(int offset, int limit, string code, long date);
    }

    public class ForeignMongoRepo : MongoRepositoryBaseForeign<Foreign>, IForeignMongoRepo
    {
        public ForeignMongoRepo(){}

        public List<Foreign> GetWithFilter(int offset, int limit, string code, long date)
        {
            try
            {
                FilterDefinition<Foreign> filter = null;
                var builder = Builders<Foreign>.Filter;
                var lFilter = new List<FilterDefinition<Foreign>>();
                if (!string.IsNullOrWhiteSpace(code))
                {
                    lFilter.Add(builder.Eq(x => x.s, code));
                }
                if(date > 0)
                {
                    lFilter.Add(builder.Eq(x => x.d, date));
                }
                
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
                {
                    return _collection.AsQueryable().Skip((offset - 1) * limit).Take(limit).ToList();
                }
                else
                {
                    return _collection.Find(filter)
                        .SortByDescending(x => x.t)
                        .Skip((offset - 1) * limit)
                        .Limit(limit).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ForeignMongoRepo.GetWithFilterAsync|EXCEPTION| {ex.Message}");
            }

            return null;
        }

    }
}
