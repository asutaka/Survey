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
        public async Task SyncBCTC_NganHang()
        {
            try
            {
                var lStock = _stockRepo.GetAll();
                var lNganHang = lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name == "Ngân hàng")).Select(x => x.s);

                foreach (var item in lNganHang)
                {
                    await SyncBCTC_NganHang_KQKD(item);
                    //await SyncBCTC_NganHang_BCCT(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_BatDongSan|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_NganHang_KQKD(string code)
        {
            try
            {
                var batchCount = 8;
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

                        FilterDefinition<Financial_NH> filter = null;
                        var builder = Builders<Financial_NH>.Filter;
                        var lFilter = new List<FilterDefinition<Financial_NH>>
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

                        var lUpdate = _nhRepo.GetByFilter(filter);
                        Financial_NH entityUpdate = lUpdate.FirstOrDefault();
                        if (lUpdate is null || !lUpdate.Any())
                        {
                            //insert
                            entityUpdate = new Financial_NH
                            {
                                d = int.Parse($"{year}{quarter}"),
                                s = code,
                                t = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                            };
                            _nhRepo.InsertOne(entityUpdate);
                        }

                        //
                        var ThuNhapLai = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ThuNhapLai);
                        var ThuNhapTuDichVu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ThuNhapTuDichVu);
                        var ChiPhiHD = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChiPhiHD);
                        var TrichLap = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TrichLap);
                        var LNSTNH = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNSTNH);


                        switch (i)
                        {
                            case 0: AssignData(ThuNhapLai?.Value1, ThuNhapTuDichVu?.Value1, ChiPhiHD?.Value1, TrichLap?.Value1, LNSTNH?.Value1); break;
                            case 1: AssignData(ThuNhapLai?.Value2, ThuNhapTuDichVu?.Value2, ChiPhiHD?.Value2, TrichLap?.Value2, LNSTNH?.Value2); break;
                            case 2: AssignData(ThuNhapLai?.Value3, ThuNhapTuDichVu?.Value3, ChiPhiHD?.Value3, TrichLap?.Value3, LNSTNH?.Value3); break;
                            case 3: AssignData(ThuNhapLai?.Value4, ThuNhapTuDichVu?.Value4, ChiPhiHD?.Value4, TrichLap?.Value4, LNSTNH?.Value4); break;
                            case 4: AssignData(ThuNhapLai?.Value5, ThuNhapTuDichVu?.Value5, ChiPhiHD?.Value5, TrichLap?.Value5, LNSTNH?.Value5); break;
                            case 5: AssignData(ThuNhapLai?.Value6, ThuNhapTuDichVu?.Value6, ChiPhiHD?.Value6, TrichLap?.Value6, LNSTNH?.Value6); break;
                            case 6: AssignData(ThuNhapLai?.Value7, ThuNhapTuDichVu?.Value7, ChiPhiHD?.Value7, TrichLap?.Value7, LNSTNH?.Value7); break;
                            case 7: AssignData(ThuNhapLai?.Value8, ThuNhapTuDichVu?.Value8, ChiPhiHD?.Value8, TrichLap?.Value8, LNSTNH?.Value8); break;
                            default: break;
                        };
                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _nhRepo.Update(entityUpdate);

                        void AssignData(double? thunhaplai, double? thunhaptudichvu, double? chiphihd, double? trichlap, double? loinhuan)
                        {
                            entityUpdate.rv = (thunhaplai ?? 0) + (thunhaptudichvu ?? 0);
                            entityUpdate.pf = loinhuan ?? 0;
                            entityUpdate.cost_o = chiphihd ?? 0;
                            entityUpdate.cost_r = trichlap ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_NganHang_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        //private async Task SyncBCTC_NganHang_BCCT(string code)
        //{
        //    try
        //    {
        //        var batchCount = 8;
        //        var lReportID = await _apiService.VietStock_CDKT_GetListReportData(code);
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
        //                strBuilder.Append($"listReportDataIds[{i}][YearPeriod]={element.BasePeriodBegin / 100}");
        //            }
        //            var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
        //            var lData = await _apiService.VietStock_GetReportDataDetailValue_CDKT_ByReportDataIds(txt);
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
        //                    continue;
        //                }

        //                //
        //                var TonKho = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TonKho);
        //                var NguoiMua = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.NguoiMuaTraTienTruoc);
        //                var NoPhaiTra = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.NoPhaiTra);
        //                var VonChu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.VonChuSoHuu);

        //                switch(i)
        //                {
        //                    case 0: AssignData(TonKho?.Value1, NguoiMua?.Value1, NoPhaiTra?.Value1, VonChu?.Value1); break;
        //                    case 1: AssignData(TonKho?.Value2, NguoiMua?.Value2, NoPhaiTra?.Value2, VonChu?.Value2); break;
        //                    case 2: AssignData(TonKho?.Value3, NguoiMua?.Value3, NoPhaiTra?.Value3, VonChu?.Value3); break;
        //                    case 3: AssignData(TonKho?.Value4, NguoiMua?.Value4, NoPhaiTra?.Value4, VonChu?.Value4); break;
        //                    case 4: AssignData(TonKho?.Value5, NguoiMua?.Value5, NoPhaiTra?.Value5, VonChu?.Value5); break;
        //                    case 5: AssignData(TonKho?.Value6, NguoiMua?.Value6, NoPhaiTra?.Value6, VonChu?.Value6); break;
        //                    case 6: AssignData(TonKho?.Value7, NguoiMua?.Value7, NoPhaiTra?.Value7, VonChu?.Value7); break;
        //                    case 7: AssignData(TonKho?.Value8, NguoiMua?.Value8, NoPhaiTra?.Value8, VonChu?.Value8); break;
        //                    default: break;
        //                };

        //                entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        //                _bdsRepo.Update(entityUpdate);

        //                void AssignData(double? tonkho, double? nguoimua, double? nophaitra, double? vonchu)
        //                {
        //                    entityUpdate.inv = tonkho ?? 0;
        //                    entityUpdate.bp = nguoimua ?? 0;
        //                    entityUpdate.tl = nophaitra ?? 0;
        //                    entityUpdate.eq = vonchu ?? 0;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.SyncBCTC_BatDongSan_KQKD|EXCEPTION| {ex.Message}");
        //    }
        //}
    }
}