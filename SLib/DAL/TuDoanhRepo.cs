using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLib.DAL
{
    public interface ITuDoanhRepo : IMongoRepositoryBase<TuDoanh>
    {
        List<TuDoanh> GetWithFilter(int offset, int limit, string code, long date);
        List<TuDoanh> GetWithCodeOrderby(int offset, int limit, string code, long date);
        List<TuDoanh> GetWithFilter(int offset, int limit, FilterDefinition<TuDoanh> filter);
    }

    public class TuDoanhRepo : MongoBaseRepo<TuDoanh>, ITuDoanhRepo
    {
        public TuDoanhRepo()
        { }

        public List<TuDoanh> GetWithFilter(int offset, int limit, FilterDefinition<TuDoanh> filter)
        {
            try
            {
                if (filter is null)
                {
                    return _collection.AsQueryable().Skip((offset - 1) * limit)
                        .Take(limit).ToList();
                }

                return _collection.Find(filter)
                        .Skip((offset - 1) * limit)
                        .Limit(limit).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TuDoanhRepo.GetWithFilter|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        public List<TuDoanh> GetWithFilter(int offset, int limit, string code, long date)
        {
            try
            {
                FilterDefinition<TuDoanh> filter = null;
                var builder = Builders<TuDoanh>.Filter;
                var lFilter = new List<FilterDefinition<TuDoanh>>();
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
                _logger.LogError($"TuDoanhRepo.GetWithFilterAsync|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        public List<TuDoanh> GetWithCodeOrderby(int offset, int limit, string code, long date)
        {
            try
            {
                FilterDefinition<TuDoanh> filter = null;
                var builder = Builders<TuDoanh>.Filter;
                var lFilter = new List<FilterDefinition<TuDoanh>>();
                if (!string.IsNullOrWhiteSpace(code))
                {
                    lFilter.Add(builder.Eq(x => x.s, code));
                }
                if (date > 0)
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
                    return null;

                return _collection.Find(filter)
                    .SortByDescending(x => x.d)
                        .Skip((offset - 1) * limit)
                        .Limit(limit).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TuDoanhRepo.GetWithCodeOrderby|EXCEPTION| {ex.Message}");
            }

            return null;
        }

    }
}
