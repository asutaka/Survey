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

                var lDSC = await _apiService.DSC_GetPost();
                if(lDSC != null)
                {
                    var lValid = lDSC.Where(x => x.attributes.public_at > time);
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
                                sBuilder.AppendLine($"[DSC - Phân tích cổ phiếu] - {code}:{itemValid.attributes.title}");
                            }
                            else
                            {
                                sBuilder.AppendLine($"[DSC - Báo cáo phân tích]");
                            }
                            sBuilder.AppendLine($"Link: www.dsc.com.vn/bao-cao-phan-tich/{itemValid.attributes.slug}");
                        }
                    }
                }

                var lVNDirect = await _apiService.VNDirect_GetPost();
                if (lVNDirect != null)
                {
                    var t = int.Parse($"{time.Year}{time.Month.To2Digit()}{time.Day.To2Digit()}");
                    var lValid = lVNDirect.Where(x => int.Parse(x.newsDate.Replace("-","")) >= t);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.VNDirect),
                                builder.Eq(x => x.key, itemValid.newsId),
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
                                key = itemValid.newsId,
                                ty = (int)ESource.DSC
                            });

                            sBuilder.AppendLine($"[VNDirect - Phân tích cổ phiếu] - {itemValid.tagsCode}: {itemValid.newsTitle}");
                            sBuilder.AppendLine($"Link: https://dstock.vndirect.com.vn/trung-tam-phan-tich/bao-cao-phan-tich-dn");
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
