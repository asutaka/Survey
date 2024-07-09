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
        List<Foreign> GetByFilterDESC(FilterDefinition<Foreign> filter, int offset = 0, int limit = 0);
        List<Foreign> GetByFilterASC(FilterDefinition<Foreign> filter, int offset = 0, int limit = 0);
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
                    return _collection.AsQueryable().OrderByDescending(x => x.t).Skip((offset - 1) * limit).Take(limit).ToList();
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

        public List<Foreign> GetByFilterDESC(FilterDefinition<Foreign> filter, int offset = 0, int limit = 0)
        {
            if (filter is null)
            {

                if (offset > 0)
                {
                    return _collection.AsQueryable().OrderByDescending(x => x.fbvat - x.fsvat).Skip((offset - 1) * limit)
                    .Take(limit).ToList();
                }

                return _collection.AsQueryable().OrderByDescending(x => x.fbvat - x.fsvat).ToList();
            }
            else
            {
                if (offset > 0)
                {
                    return _collection.Find(filter).SortByDescending(x => x.fbvat - x.fsvat).Skip((offset - 1) * limit)
                            .Limit(limit).ToList();
                }

                return _collection.Find(filter).SortByDescending(x => x.fbvat - x.fsvat).ToList();
            }
        }

        public List<Foreign> GetByFilterASC(FilterDefinition<Foreign> filter, int offset = 0, int limit = 0)
        {
            if (filter is null)
            {

                if (offset > 0)
                {
                    return _collection.AsQueryable().OrderBy(x => x.fbvat - x.fsvat).Skip((offset - 1) * limit)
                    .Take(limit).ToList();
                }

                return _collection.AsQueryable().OrderBy(x => x.fbvat - x.fsvat).ToList();
            }
            else
            {
                if (offset > 0)
                {
                    return _collection.Find(filter).SortBy(x => x.fbvat - x.fsvat).Skip((offset - 1) * limit)
                            .Limit(limit).ToList();
                }

                return _collection.Find(filter).SortBy(x => x.fbvat - x.fsvat).ToList();
            }
        }

    }
}
