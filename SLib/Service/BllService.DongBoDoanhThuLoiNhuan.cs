using MongoDB.Driver;
using SLib.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task DongBoDoanhThuLoiNhuan()
        {
            var lStock = GetStock();
            foreach (var item in lStock)
            {
                try
                {
                    var filter = Builders<Financial>.Filter.Eq(x => x.s, item.s);
                    var lFinancial = _financialRepo.GetByFilter(filter);
                    var last = lFinancial?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.t).FirstOrDefault();

                    var lApi = await _apiService.GetDoanhThuLoiNhuan(item.s);
                    if (lApi is null || !lApi.Any())
                        continue;

                    foreach (var itemApi in lApi.Where(x => x.revenue != null && x.profit != null))
                    {
                        if (last != null && last.yearReport >= itemApi.yearReport && last.lengthReport >= itemApi.lengthReport)
                            break;

                        itemApi.s = item.s;
                        itemApi.d = long.Parse($"{itemApi.yearReport}{itemApi.lengthReport}");
                        itemApi.t = DateTimeOffset.Now.ToUnixTimeSeconds();
                        _financialRepo.InsertOne(itemApi);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
