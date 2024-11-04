using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task SyncBCTC_CaoSu(bool onlyLast = false)
        {
            try
            {
                var lStockFilter = StaticVal._lCaoSu;
                foreach (var item in lStockFilter)
                {
                    await SyncBCTC_CaoSu_KQKD(item, onlyLast);
                    await SyncBCTC_CaoSu_CDKT(item, onlyLast);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_CaoSu|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_CaoSu_KQKD(string code, bool onlyLast)
        {
            try
            {
                var time = GetCurrentTime();
                var batchCount = 8;
                var lReportID = await _apiService.VietStock_KQKD_GetListReportData(code);
                if (onlyLast)
                {
                    lReportID.data = lReportID.data.TakeLast(4).ToList();
                }
                Thread.Sleep(1000);
                if (!lReportID.data.Any())
                    return;
                var totalCount = lReportID.data.Count();
                var last = lReportID.data.Last();
                if (last.BasePeriodBegin / 100 > time.Item2)
                {
                    var div = last.ReportTermID - time.Item3;
                    foreach (var item in lReportID.data)
                    {
                        var term = item.ReportTermID - div;
                        if (term == 0)
                        {
                            term = 4;
                            item.YearPeriod -= 1;
                        }
                        var month = 0;
                        if (term == 1)
                        {
                            month = 1;
                        }
                        else if (term == 2)
                        {
                            month = 4;
                        }
                        else if (term == 3)
                        {
                            month = 7;
                        }
                        else if (term == 4)
                        {
                            month = 10;
                        }

                        item.BasePeriodBegin = int.Parse($"{item.YearPeriod}{month.To2Digit()}");
                    }
                }
                lReportID.data = lReportID.data.Where(x => (x.Isunited == 0 || x.Isunited == 1) && x.BasePeriodBegin >= 202001).ToList();
                var lBatch = new List<List<ReportDataIDDetailResponse>>();
                var lSub = new List<ReportDataIDDetailResponse>();
                for (int i = 0; i < lReportID.data.Count; i++)
                {
                    if (i > 0 && i % batchCount == 0)
                    {
                        lBatch.Add(lSub.ToList());
                        lSub.Clear();
                    }
                    lSub.Add(lReportID.data[i]);
                }
                if (lSub.Any())
                {
                    lBatch.Add(lSub.ToList());
                }


                foreach (var item in lBatch)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"StockCode={code}&");
                    strBuilder.Append($"Unit=1000000000&");
                    strBuilder.Append($"__RequestVerificationToken={StaticVal._VietStock_Token}");

                    var count = item.Count();
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0)
                        {
                            strBuilder.Append("&");
                        }
                        var element = item[i];
                        strBuilder.Append($"listReportDataIds[{i}][ReportDataId]={element.ReportDataID}&");
                        strBuilder.Append($"listReportDataIds[{i}][YearPeriod]={element.BasePeriodBegin / 100}");
                    }
                    var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
                    var lData = await _apiService.VietStock_GetReportDataDetailValue_KQKD_ByReportDataIds(txt);
                    Thread.Sleep(1000);
                    if (lData is null || lData.data is null)
                        continue;

                    for (int i = 0; i < count; i++)
                    {
                        var element = item[i];
                        var year = element.BasePeriodBegin / 100;
                        var month = element.BasePeriodBegin - year * 100;
                        var quarter = 1;
                        if (month >= 10)
                        {
                            quarter = 4;
                        }
                        else if (month >= 7)
                        {
                            quarter = 3;
                        }
                        else if (month >= 4)
                        {
                            quarter = 2;
                        }

                        FilterDefinition<Financial> filter = null;
                        var builder = Builders<Financial>.Filter;
                        var lFilter = new List<FilterDefinition<Financial>>
                            {
                                builder.Eq(x => x.s, code),
                                builder.Eq(x => x.d, int.Parse($"{year}{quarter}"))
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

                        var lUpdate = _financialRepo.GetByFilter(filter);
                        Financial entityUpdate = lUpdate.FirstOrDefault();
                        if (lUpdate is null || !lUpdate.Any())
                        {
                            //insert
                            entityUpdate = new Financial
                            {
                                d = int.Parse($"{year}{quarter}"),
                                s = code,
                                t = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                            };
                            _financialRepo.InsertOne(entityUpdate);
                        }

                        //
                        var DoanhThu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.DoanhThu);
                        var LoiNhuan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNST);
                        var LoiNhuanGop = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNGop);
                        var LoiNhuanRong = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNRong);

                        switch (i)
                        {
                            case 0: AssignData(DoanhThu?.Value1, LoiNhuan?.Value1, LoiNhuanGop?.Value1, LoiNhuanRong?.Value1); break;
                            case 1: AssignData(DoanhThu?.Value2, LoiNhuan?.Value2, LoiNhuanGop?.Value2, LoiNhuanRong?.Value2); break;
                            case 2: AssignData(DoanhThu?.Value3, LoiNhuan?.Value3, LoiNhuanGop?.Value3, LoiNhuanRong?.Value3); break;
                            case 3: AssignData(DoanhThu?.Value4, LoiNhuan?.Value4, LoiNhuanGop?.Value4, LoiNhuanRong?.Value4); break;
                            case 4: AssignData(DoanhThu?.Value5, LoiNhuan?.Value5, LoiNhuanGop?.Value5, LoiNhuanRong?.Value5); break;
                            case 5: AssignData(DoanhThu?.Value6, LoiNhuan?.Value6, LoiNhuanGop?.Value6, LoiNhuanRong?.Value6); break;
                            case 6: AssignData(DoanhThu?.Value7, LoiNhuan?.Value7, LoiNhuanGop?.Value7, LoiNhuanRong?.Value7); break;
                            case 7: AssignData(DoanhThu?.Value8, LoiNhuan?.Value8, LoiNhuanGop?.Value8, LoiNhuanRong?.Value8); break;
                            default: break;
                        };

                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _financialRepo.Update(entityUpdate);

                        void AssignData(double? DoanhThu, double? LoiNhuan, double? LoiNhuanGop, double? LoiNhuanRong)
                        {
                            entityUpdate.rv = DoanhThu ?? 0;
                            entityUpdate.pf = LoiNhuan ?? 0;
                            entityUpdate.pfg = LoiNhuanGop ?? 0;
                            entityUpdate.pfn = LoiNhuanRong ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_CaoSu_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_CaoSu_CDKT(string code, bool onlyLast)
        {
            try
            {
                var time = GetCurrentTime();
                var batchCount = 8;
                var lReportID = await _apiService.VietStock_CDKT_GetListReportData(code);
                if (onlyLast)
                {
                    lReportID.data = lReportID.data.TakeLast(4).ToList();
                }
                Thread.Sleep(1000);
                if (!lReportID.data.Any())
                    return;
                var totalCount = lReportID.data.Count();
                var last = lReportID.data.Last();
                if (last.BasePeriodBegin / 100 > time.Item2)
                {
                    var div = last.ReportTermID - time.Item3;
                    foreach (var item in lReportID.data)
                    {
                        var term = item.ReportTermID - div;
                        if (term == 0)
                        {
                            term = 4;
                            item.YearPeriod -= 1;
                        }
                        var month = 0;
                        if (term == 1)
                        {
                            month = 1;
                        }
                        else if (term == 2)
                        {
                            month = 4;
                        }
                        else if (term == 3)
                        {
                            month = 7;
                        }
                        else if (term == 4)
                        {
                            month = 10;
                        }

                        item.BasePeriodBegin = int.Parse($"{item.YearPeriod}{month.To2Digit()}");
                    }
                }
                lReportID.data = lReportID.data.Where(x => (x.Isunited == 0 || x.Isunited == 1) && x.BasePeriodBegin >= 202001).ToList();
                var lBatch = new List<List<ReportDataIDDetailResponse>>();
                var lSub = new List<ReportDataIDDetailResponse>();
                for (int i = 0; i < lReportID.data.Count; i++)
                {
                    if (i > 0 && i % batchCount == 0)
                    {
                        lBatch.Add(lSub.ToList());
                        lSub.Clear();
                    }
                    lSub.Add(lReportID.data[i]);
                }
                if (lSub.Any())
                {
                    lBatch.Add(lSub.ToList());
                }


                foreach (var item in lBatch)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"StockCode={code}&");
                    strBuilder.Append($"Unit=1000000000&");
                    strBuilder.Append($"__RequestVerificationToken={StaticVal._VietStock_Token}");

                    var count = item.Count();
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0)
                        {
                            strBuilder.Append("&");
                        }
                        var element = item[i];
                        strBuilder.Append($"listReportDataIds[{i}][ReportDataId]={element.ReportDataID}&");
                        strBuilder.Append($"listReportDataIds[{i}][YearPeriod]={element.BasePeriodBegin / 100}");
                    }
                    var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
                    var lData = await _apiService.VietStock_GetReportDataDetailValue_CDKT_ByReportDataIds(txt);
                    Thread.Sleep(1000);
                    if (lData is null || lData.data is null)
                        continue;

                    for (int i = 0; i < count; i++)
                    {
                        var element = item[i];
                        var year = element.BasePeriodBegin / 100;
                        var month = element.BasePeriodBegin - year * 100;
                        var quarter = 1;
                        if (month >= 10)
                        {
                            quarter = 4;
                        }
                        else if (month >= 7)
                        {
                            quarter = 3;
                        }
                        else if (month >= 4)
                        {
                            quarter = 2;
                        }

                        FilterDefinition<Financial> filter = null;
                        var builder = Builders<Financial>.Filter;
                        var lFilter = new List<FilterDefinition<Financial>>
                        {
                            builder.Eq(x => x.s, code),
                            builder.Eq(x => x.d, int.Parse($"{year}{quarter}"))
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

                        var lUpdate = _financialRepo.GetByFilter(filter);
                        Financial entityUpdate = lUpdate.FirstOrDefault();
                        if (lUpdate is null || !lUpdate.Any())
                        {
                            continue;
                        }

                        //
                        var TonKho = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TonKhoThep);
                        var VayNganHan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.VayNganHan);
                        var VayDaiHan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.VayDaiHan);
                        var VonChuSH = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.VonChuSoHuu);

                        switch (i)
                        {
                            case 0: AssignData(TonKho?.Value1, VayNganHan?.Value1, VayDaiHan?.Value1, VonChuSH?.Value1); break;
                            case 1: AssignData(TonKho?.Value2, VayNganHan?.Value2, VayDaiHan?.Value2, VonChuSH?.Value2); break;
                            case 2: AssignData(TonKho?.Value3, VayNganHan?.Value3, VayDaiHan?.Value3, VonChuSH?.Value3); break;
                            case 3: AssignData(TonKho?.Value4, VayNganHan?.Value4, VayDaiHan?.Value4, VonChuSH?.Value4); break;
                            case 4: AssignData(TonKho?.Value5, VayNganHan?.Value5, VayDaiHan?.Value5, VonChuSH?.Value5); break;
                            case 5: AssignData(TonKho?.Value6, VayNganHan?.Value6, VayDaiHan?.Value6, VonChuSH?.Value6); break;
                            case 6: AssignData(TonKho?.Value7, VayNganHan?.Value7, VayDaiHan?.Value7, VonChuSH?.Value7); break;
                            case 7: AssignData(TonKho?.Value8, VayNganHan?.Value8, VayDaiHan?.Value8, VonChuSH?.Value8); break;
                            default: break;
                        };

                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _financialRepo.Update(entityUpdate);

                        void AssignData(double? TonKho, double? VayNganHan, double? VayDaiHan, double? VonChuSH)
                        {
                            entityUpdate.inv = TonKho ?? 0;
                            entityUpdate.debt = (VayNganHan ?? 0) + (VayDaiHan ?? 0);
                            entityUpdate.eq = VonChuSH ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_CaoSu_CDKT|EXCEPTION| {ex.Message}");
            }
        }
    }
}
