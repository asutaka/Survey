using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockLibrary.DAL
{
    public interface ITuDoanhMongoRepo : IMongoRepositoryBase<TuDoanh>
    {
        List<TuDoanh> GetWithFilter(int offset, int limit, string code, DateTime date, string type);
    }

    public class TuDoanhMongoRepo : MongoRepositoryBase<TuDoanh>, ITuDoanhMongoRepo
    {
        public TuDoanhMongoRepo()
        {}

        public List<TuDoanh> GetWithFilter(int offset, int limit, string code, DateTime date, string type)
        {
            try
            {
                FilterDefinition<TuDoanh> filter = null;
                var builder = Builders<TuDoanh>.Filter;
                var lFilter = new List<FilterDefinition<TuDoanh>>();
                if (string.IsNullOrWhiteSpace(code))
                    return null;

                lFilter.Add(builder.Eq(x => x.ma_ck, code));
                lFilter.Add(builder.Eq(x => x.ngay, date));
                lFilter.Add(builder.Eq(x => x.type, type));
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
                _logger.LogError($"TuDoanhMongoRepo.GetWithFilterAsync|EXCEPTION| {ex.Message}");
            }

            return null;
        }

    }
}
