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
        public async Task SyncBCTC_BatDongSan()
        {
            await SyncBCTC_BatDongSan_KQKD("DPG");
            return;
            try
            {
                var lStock = _stockRepo.GetAll();
                var lBDS = lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name == "Xây dựng" || y.name == "Bất động sản")).Select(x => x.s);

                foreach (var item in lBDS)
                {
                    await SyncBCTC_BatDongSan_KQKD(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_BatDongSan|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_BatDongSan_KQKD(string code)
        {
            try
            {
                var batchCount = 1;
                var lReportID = await _apiService.VietStock_KQKD_GetListReportData(code);
                Thread.Sleep(1000);
                var totalCount = lReportID.data.Count();
                lReportID.data = lReportID.data.Where(x => x.Isunited == 0 && x.BasePeriodBegin >= 202001).ToList();
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
                if(lSub.Any())
                {
                    lBatch.Add(lSub.ToList());
                }
                
               
                foreach (var item in lBatch)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"StockCode={code}&");
                    strBuilder.Append($"Unit=1000000000&");
                    strBuilder.Append($"__RequestVerificationToken=KfZYybRs9qYVyNmoczUJUaVItBQB3M64pTTwOaz0XLN7DziURs1EoHjiUPLcHiAY4OlvMaIMTGzRUiWbjVTqUm3vw0vwAHMoEJbgeqa8NpFi-NBrUMIYHOx4ApBOrenS0&");

                    var count = item.Count();
                    for (int i = 0; i < count; i++)
                    {

                        if (i > 0)
                        {
                            strBuilder.Append("&");
                        }
                        var element = item[i];
                        strBuilder.Append($"listReportDataIds[{i}][ReportDataId]={element.ReportDataID}&");
                        strBuilder.Append($"listReportDataIds[{i}][Index]={i}&");
                        strBuilder.Append($"listReportDataIds[{i}][IsShowData]={true}&");
                        strBuilder.Append($"listReportDataIds[{i}][RowNumber]={element.RowNumber}&");
                        strBuilder.Append($"listReportDataIds[{i}][SortTimeType]=Time_ASC&");
                        strBuilder.Append($"listReportDataIds[{i}][TotalCount]={totalCount}&");
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
                        if(month >= 10)
                        {
                            quarter = 4;
                        }
                        else if(month >= 7)
                        {
                            quarter = 3;
                        }
                        else if(month >= 4)
                        {
                            quarter = 2;
                        }

                        FilterDefinition<Financial_BDS> filter = null;
                        var builder = Builders<Financial_BDS>.Filter;
                        var lFilter = new List<FilterDefinition<Financial_BDS>>
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

                        var lUpdate = _bdsRepo.GetByFilter(filter);
                        Financial_BDS entityUpdate = lUpdate.FirstOrDefault();
                        if (lUpdate is null || !lUpdate.Any())
                        {
                            //insert
                            entityUpdate = new Financial_BDS
                            {
                                d = int.Parse($"{year}{quarter}"),
                                s = code,
                                t = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                            };
                            _bdsRepo.InsertOne(entityUpdate);
                        }

                        //
                        var DoanhThu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.DoanhThu);
                        var LoiNhuan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNST);
                        var GiaVon = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.GiaVon);

                        if(i == 0)
                        {
                            if((DoanhThu?.Value1 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value1 ?? 0;
                            }

                            if ((LoiNhuan?.Value1 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value1 ?? 0;
                            }

                            if ((GiaVon?.Value1 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value1 ?? 0;
                            }
                        }
                        else if (i == 1)
                        {
                            if ((DoanhThu?.Value2 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value2 ?? 0;
                            }

                            if ((LoiNhuan?.Value2 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value2 ?? 0;
                            }

                            if ((GiaVon?.Value2 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value2 ?? 0;
                            }
                        }
                        else if (i == 2)
                        {
                            if ((DoanhThu?.Value3 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value3 ?? 0;
                            }

                            if ((LoiNhuan?.Value3 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value3 ?? 0;
                            }

                            if ((GiaVon?.Value3 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value3 ?? 0;
                            }
                        }
                        else if (i == 3)
                        {
                            if ((DoanhThu?.Value4 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value4 ?? 0;
                            }

                            if ((LoiNhuan?.Value4 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value4 ?? 0;
                            }

                            if ((GiaVon?.Value4 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value4 ?? 0;
                            }
                        }
                        else if (i == 4)
                        {
                            if ((DoanhThu?.Value5 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value5 ?? 0;
                            }

                            if ((LoiNhuan?.Value5 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value5 ?? 0;
                            }

                            if ((GiaVon?.Value5 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value5 ?? 0;
                            }
                        }
                        else if (i == 5)
                        {
                            if ((DoanhThu?.Value6 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value6 ?? 0;
                            }

                            if ((LoiNhuan?.Value6 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value6 ?? 0;
                            }

                            if ((GiaVon?.Value6 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value6 ?? 0;
                            }
                        }
                        else if (i == 6)
                        {
                            if ((DoanhThu?.Value7 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value7 ?? 0;
                            }

                            if ((LoiNhuan?.Value7 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value7 ?? 0;
                            }

                            if ((GiaVon?.Value7 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value7 ?? 0;
                            }
                        }
                        else if (i == 7)
                        {
                            if ((DoanhThu?.Value8 ?? 0) > 0)
                            {
                                entityUpdate.rv = DoanhThu?.Value8 ?? 0;
                            }

                            if ((LoiNhuan?.Value8 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan?.Value8 ?? 0;
                            }

                            if ((GiaVon?.Value8 ?? 0) > 0)
                            {
                                entityUpdate.ce = GiaVon?.Value8 ?? 0;
                            }
                        }

                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _bdsRepo.Update(entityUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_BatDongSan_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        //private async Task SyncBCTC_BatDongSan_BCCT(string code)
        //{
        //    try
        //    {
        //        var batchCount = 8;
        //        var lReportID = await _apiService.VietStock_KQKD_GetListReportData(code);
        //        Thread.Sleep(1000);
        //        var totalCount = lReportID.data.Count();
        //        lReportID.data = lReportID.data.Where(x => x.Isunited == 0 && x.BasePeriodBegin >= 202001).ToList();
        //        var lBatch = new List<List<ReportDataIDDetailResponse>>();
        //        var lSub = new List<ReportDataIDDetailResponse>();
        //        for (int i = 0; i < lReportID.data.Count; i++)
        //        {
        //            if (i > 0 && i % batchCount == 0)
        //            {
        //                lBatch.Add(lSub.ToList());
        //                lSub.Clear();
        //            }
        //            lSub.Add(lReportID.data[i]);
        //        }
        //        if (lSub.Any())
        //        {
        //            lBatch.Add(lSub.ToList());
        //        }


        //        foreach (var item in lBatch)
        //        {
        //            var strBuilder = new StringBuilder();
        //            strBuilder.Append($"StockCode={code}&");
        //            strBuilder.Append($"Unit=1000000000&");
        //            strBuilder.Append($"__RequestVerificationToken=KfZYybRs9qYVyNmoczUJUaVItBQB3M64pTTwOaz0XLN7DziURs1EoHjiUPLcHiAY4OlvMaIMTGzRUiWbjVTqUm3vw0vwAHMoEJbgeqa8NpFi-NBrUMIYHOx4ApBOrenS0&");

        //            var count = item.Count();
        //            for (int i = 0; i < count; i++)
        //            {

        //                if (i > 0)
        //                {
        //                    strBuilder.Append("&");
        //                }
        //                var element = item[i];
        //                strBuilder.Append($"listReportDataIds[{i}][ReportDataId]={element.ReportDataID}&");
        //                strBuilder.Append($"listReportDataIds[{i}][Index]={i}&");
        //                strBuilder.Append($"listReportDataIds[{i}][IsShowData]={true}&");
        //                strBuilder.Append($"listReportDataIds[{i}][RowNumber]={element.RowNumber}&");
        //                strBuilder.Append($"listReportDataIds[{i}][SortTimeType]=Time_ASC&");
        //                strBuilder.Append($"listReportDataIds[{i}][TotalCount]={totalCount}&");
        //                strBuilder.Append($"listReportDataIds[{i}][YearPeriod]={element.BasePeriodBegin / 100}");
        //            }
        //            var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
        //            var lData = await _apiService.VietStock_GetReportDataDetailValue_KQKD_ByReportDataIds(txt);
        //            Thread.Sleep(1000);
        //            if (lData is null || lData.data is null)
        //                continue;

        //            for (int i = 0; i < count; i++)
        //            {
        //                var element = item[i];
        //                var year = element.BasePeriodBegin / 100;
        //                var month = element.BasePeriodBegin - year * 100;
        //                var quarter = 1;
        //                if (month >= 10)
        //                {
        //                    quarter = 4;
        //                }
        //                else if (month >= 7)
        //                {
        //                    quarter = 3;
        //                }
        //                else if (month >= 4)
        //                {
        //                    quarter = 2;
        //                }

        //                FilterDefinition<Financial_BDS> filter = null;
        //                var builder = Builders<Financial_BDS>.Filter;
        //                var lFilter = new List<FilterDefinition<Financial_BDS>>
        //                {
        //                    builder.Eq(x => x.s, code),
        //                    builder.Eq(x => x.d, int.Parse($"{year}{quarter}"))
        //                };

        //                foreach (var itemFilter in lFilter)
        //                {
        //                    if (filter is null)
        //                    {
        //                        filter = itemFilter;
        //                        continue;
        //                    }
        //                    filter &= itemFilter;
        //                }

        //                var lUpdate = _bdsRepo.GetByFilter(filter);
        //                Financial_BDS entityUpdate = lUpdate.FirstOrDefault();
        //                if (lUpdate is null || !lUpdate.Any())
        //                {
        //                    //insert
        //                    entityUpdate = new Financial_BDS
        //                    {
        //                        d = int.Parse($"{year}{quarter}"),
        //                        s = code,
        //                        t = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
        //                    };
        //                    _bdsRepo.InsertOne(entityUpdate);
        //                }

        //                //
        //                var DoanhThu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.DoanhThu);
        //                var LoiNhuan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNST);
        //                var GiaVon = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.GiaVon);


        //                //var ChiPhiTaiChinh = lData?.data.FirstOrDefault(x => x.ReportnormId == 3004);
        //                //var TonKho = lData?.data.FirstOrDefault(x => x.ReportnormId == 3006);
        //                //var TongTaiSan = lData?.data.FirstOrDefault(x => x.ReportnormId == 2996);
        //                //var NoPhaiTra = lData?.data.FirstOrDefault(x => x.ReportnormId == 2997);
        //                //var VonChu = lData?.data.FirstOrDefault(x => x.ReportnormId == 2998);

        //                if (i == 0)
        //                {
        //                    if ((DoanhThu?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value1 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value1 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value1 ?? 0;
        //                    }

        //                    if ((TonKho?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value1 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value1 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value1 ?? 0;
        //                    }

        //                    if ((VonChu?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value1 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value1 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value1 ?? 0;
        //                    }
        //                }
        //                else if (i == 1)
        //                {
        //                    if ((DoanhThu?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value2 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value2 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value2 ?? 0;
        //                    }

        //                    if ((TonKho?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value2 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value2 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value2 ?? 0;
        //                    }

        //                    if ((VonChu?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value2 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value2 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value2 ?? 0;
        //                    }
        //                }
        //                else if (i == 2)
        //                {
        //                    if ((DoanhThu?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value3 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value3 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value3 ?? 0;
        //                    }

        //                    if ((TonKho?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value3 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value3 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value3 ?? 0;
        //                    }

        //                    if ((VonChu?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value3 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value3 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value3 ?? 0;
        //                    }
        //                }
        //                else if (i == 3)
        //                {
        //                    if ((DoanhThu?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value4 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value4 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value4 ?? 0;
        //                    }

        //                    if ((TonKho?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value4 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value4 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value4 ?? 0;
        //                    }

        //                    if ((VonChu?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value4 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value4 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value4 ?? 0;
        //                    }
        //                }
        //                else if (i == 4)
        //                {
        //                    if ((DoanhThu?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value5 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value5 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value5 ?? 0;
        //                    }

        //                    if ((TonKho?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value5 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value5 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value5 ?? 0;
        //                    }

        //                    if ((VonChu?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value5 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value5 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value5 ?? 0;
        //                    }
        //                }
        //                else if (i == 5)
        //                {
        //                    if ((DoanhThu?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value6 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value6 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value6 ?? 0;
        //                    }

        //                    if ((TonKho?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value6 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value6 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value6 ?? 0;
        //                    }

        //                    if ((VonChu?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value6 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value6 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value6 ?? 0;
        //                    }
        //                }
        //                else if (i == 6)
        //                {
        //                    if ((DoanhThu?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value7 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value7 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value7 ?? 0;
        //                    }

        //                    if ((TonKho?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value7 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value7 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value7 ?? 0;
        //                    }

        //                    if ((VonChu?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value7 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value7 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value7 ?? 0;
        //                    }
        //                }
        //                else if (i == 7)
        //                {
        //                    if ((DoanhThu?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.rv.va = DoanhThu?.Value8 ?? 0;
        //                    }

        //                    if ((LoiNhuan?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.pf = LoiNhuan?.Value8 ?? 0;
        //                    }

        //                    if ((ChiPhiTaiChinh?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.fi.va = ChiPhiTaiChinh?.Value8 ?? 0;
        //                    }

        //                    if ((TonKho?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.inv.va = TonKho?.Value8 ?? 0;
        //                    }

        //                    if ((TongTaiSan?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ta = TongTaiSan?.Value8 ?? 0;
        //                    }

        //                    if ((NoPhaiTra?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.tl = NoPhaiTra?.Value8 ?? 0;
        //                    }

        //                    if ((VonChu?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.eq = VonChu?.Value8 ?? 0;
        //                    }

        //                    if ((GiaVon?.Value8 ?? 0) > 0)
        //                    {
        //                        entityUpdate.ce.va = GiaVon?.Value8 ?? 0;
        //                    }
        //                }

        //                entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        //                _financialRepo.Update(entityUpdate);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.SyncBCTC_BatDongSan|EXCEPTION| {ex.Message}");
        //    }
        //}
    }
}