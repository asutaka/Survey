using MongoDB.Driver;
using StockLib.DAL;
using StockLib.DAL.Entity;

namespace StockLib.PublicService
{
    public interface IImportDataAPIService
    {
        Financial_NH GetFinancial_NH(int d, string s);
        void UpdateFinancial_NH(Financial_NH entity);
    }
    public class ImportDataAPIService : IImportDataAPIService
    {
        private readonly IFinancialNHRepo _repo;
        public ImportDataAPIService(IFinancialNHRepo repo)
        {
            _repo = repo;
        }
        public Financial_NH GetFinancial_NH(int d, string s)
        {
            FilterDefinition<Financial_NH> filter = null;
            var builder = Builders<Financial_NH>.Filter;
            var lFilter = new List<FilterDefinition<Financial_NH>>
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

        public void UpdateFinancial_NH(Financial_NH entity)
        {
            _repo.Update(entity);
        }
    }
}
