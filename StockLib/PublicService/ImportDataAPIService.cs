using MongoDB.Driver;
using StockLib.DAL;
using StockLib.DAL.Entity;

namespace StockLib.PublicService
{
    public interface IImportDataAPIService
    {
        Financial GetFinancial(int d, string s);
        void UpdateFinancial(Financial entity);
    }
    public class ImportDataAPIService : IImportDataAPIService
    {
        private readonly IFinancialRepo _repo;
        public ImportDataAPIService(IFinancialRepo repo)
        {
            _repo = repo;
        }
        public Financial GetFinancial(int d, string s)
        {
            FilterDefinition<Financial> filter = null;
            var builder = Builders<Financial>.Filter;
            var lFilter = new List<FilterDefinition<Financial>>
                        {
                            builder.Eq(x => x.s, s),
                            builder.Eq(x => x.d, d)
                        };

            foreach (var itemFilter in lFilter)
            {
                if (filter is null)
                {
                    filter = itemFilter;
                    continue;
                }
                filter &= itemFilter;
            }

            var lRes = _repo.GetByFilter(filter);
            return lRes.FirstOrDefault();
        }

        public void UpdateFinancial(Financial entity)
        {
            _repo.Update(entity);
        }
    }
}
