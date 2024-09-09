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
                foreach (var item in StaticVal._lStock)
                {
                    var lKeHoach = await _apiService.Money24h_GetKeHoach(item.s);
                    if (lKeHoach is null || !lKeHoach.Any())
                        continue;

                    foreach (var itemKeHoach in lKeHoach)
                    {
                        if (itemKeHoach.year < 2020)
                            continue;

                        var model = new KeHoach
                        {
                            s = item.s,
                            d = itemKeHoach.year,
                            pf_plan = Math.Round(itemKeHoach.isa22, 1)
                        };

                        var real = itemKeHoach.quarter.FirstOrDefault(x => x.quarter == 0);
                        if (real != null)
                        {
                            model.pf_real = real.isa22_report;
                            model.pf_real_r = real.isa22_percent;
                        }

                        var cum = itemKeHoach.quarter.FirstOrDefault(x => x.quarter == 5);
                        if (cum != null)
                        {
                            model.pf_cum = cum.isa22_report;
                            model.pf_cum_r = cum.isa22_percent;
                        }

                        FilterDefinition<KeHoach> filter = null;
                        var builder = Builders<KeHoach>.Filter;
                        var lFilter = new List<FilterDefinition<KeHoach>>()
                        {
                            builder.Eq(x => x.d, itemKeHoach.year),
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

                        var lCheck = _kehoachRepo.GetByFilter(filter);
                        if (lCheck?.Any() ?? false)
                        {
                            //update
                            var entity = lCheck.First();
                            entity.pf_real = model.pf_real;
                            entity.pf_real_r = model.pf_real_r;
                            entity.pf_cum = model.pf_cum;
                            entity.pf_cum_r = model.pf_cum_r;
                            _kehoachRepo.Update(entity);

                            continue;
                        }

                        _kehoachRepo.InsertOne(model);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncKeHoach|EXCEPTION| {ex.Message}");
            }
        }
    }
}
