using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> BaoCaoPhanTich(DateTime dt)
        {
            try
            {
                var sBuilder = new StringBuilder();
                var time = new DateTime(dt.Year, dt.Month, dt.Day);
                var d = int.Parse($"{time.Year}{time.Month.To2Digit()}{time.Day.To2Digit()}");

                var lPost = await _apiService.DSC_GetPost();
                if(lPost != null)
                {
                    var lValid = lPost.Where(x => x.attributes.public_at > time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.DSC),
                                builder.Eq(x => x.key, itemValid.id.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id.ToString(),
                                ty = (int)ESource.DSC
                            });

                            if (itemValid.attributes.category_id.data.attributes.slug.Equals("phan-tich-doanh-nghiep"))
                            {
                                var code = itemValid.attributes.slug.Split('-').First().ToUpper();
                                sBuilder.AppendLine($"[DSC - Phân tích cổ phiếu] - {code}");
                            }
                            else
                            {
                                sBuilder.AppendLine($"[DSC - Báo cáo phân tích]");
                            }
                            sBuilder.AppendLine($"Link: www.dsc.com.vn/bao-cao-phan-tich/{itemValid.attributes.slug}");
                        }
                    }
                }

                if(sBuilder.Length > 0)
                {
                    return (1, sBuilder.ToString());
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError($"AnalyzeService.BaoCaoPhanTich|EXCEPTION| {ex.Message}");
            }
            return (0, null);
        }
    }
}
