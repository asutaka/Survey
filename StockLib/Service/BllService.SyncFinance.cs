using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task SyncPE()
        {
            try
            {
                foreach (var item in StaticVal._lStock) 
                {
                    var lData = await _apiService.SSI_GetFinance(item.s);
                    if (lData is null || !lData.Any())
                    {
                        continue;
                    }

                    foreach (var itemData in lData)
                    {
                        if (itemData.lengthReport == 4
                            || itemData.yearReport < 2020)
                            continue;
                        if(itemData.lengthReport == 5)
                        {
                            itemData.lengthReport--;
                        }

                        var dTime = int.Parse($"{itemData.yearReport}{itemData.lengthReport}");
                        FilterDefinition<ChiSoPE> filter = null;
                        var builder = Builders<ChiSoPE>.Filter;
                        var lFilter = new List<FilterDefinition<ChiSoPE>>()
                        {
                            builder.Eq(x => x.d, dTime),
                            builder.Eq(x => x.s, item.s),
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

                        var lCheck = _peRepo.GetByFilter(filter);
                        if (lCheck?.Any() ?? false) 
                            continue;

                        _peRepo.InsertOne(new ChiSoPE
                        {
                            s = item.s,
                            d = dTime,
                            eps = Math.Round(itemData.dilutedEPS, 1),
                            pe = Math.Round(itemData.dilutedPe, 1)
                        });
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError($"BllService.SyncPE|EXCEPTION| {ex.Message}");
            }
        }

        public async Task SyncKeHoach()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncKeHoach|EXCEPTION| {ex.Message}");
            }
        }
    }
}
