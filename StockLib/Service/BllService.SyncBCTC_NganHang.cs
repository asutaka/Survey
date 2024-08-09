﻿using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace StockLib.Service
{
    public partial class BllService 
    {
        /// <summary>
        /// Doanh thu,LNST, Chi phí vận hành, trích lập dự phòng lấy từ Kết quả kinh doanh
        /// CIR lấy từ chỉ số tài chính
        /// Nim + Tăng trưởng tín dụng lấy từ bảng cân đối kế toán và kết quả kinh doanh 
        /// 
        /// Casa tự tính trên BCTC


        /// Nợ xấu các mức tự nhập trên BCTC
        /// Tỉ lệ bao trùm nợ xấu tự tính trên BCTC
        /// </summary>
        /// <returns></returns>
        public async Task SyncBCTC_NganHang()
        {
            try
            {
                var lStock = _stockRepo.GetAll();
                var lNganHang = lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name == "Ngân hàng")).Select(x => x.s);

                foreach (var item in lNganHang)
                {
                    //await SyncBCTC_NganHang_KQKD(item);
                    //await SyncBCTC_NganHang_CIR(item);
                    await SyncBCTC_NganHang_NIM_TinDung(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_NganHang|EXCEPTION| {ex.Message}");
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
                        var TrichLap = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TrichLap);
                        var LNSTNH = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.LNSTNH);


                        switch (i)
                        {
                            case 0: AssignData(ThuNhapLai?.Value1, ThuNhapTuDichVu?.Value1, TrichLap?.Value1, LNSTNH?.Value1); break;
                            case 1: AssignData(ThuNhapLai?.Value2, ThuNhapTuDichVu?.Value2, TrichLap?.Value2, LNSTNH?.Value2); break;
                            case 2: AssignData(ThuNhapLai?.Value3, ThuNhapTuDichVu?.Value3, TrichLap?.Value3, LNSTNH?.Value3); break;
                            case 3: AssignData(ThuNhapLai?.Value4, ThuNhapTuDichVu?.Value4, TrichLap?.Value4, LNSTNH?.Value4); break;
                            case 4: AssignData(ThuNhapLai?.Value5, ThuNhapTuDichVu?.Value5, TrichLap?.Value5, LNSTNH?.Value5); break;
                            case 5: AssignData(ThuNhapLai?.Value6, ThuNhapTuDichVu?.Value6, TrichLap?.Value6, LNSTNH?.Value6); break;
                            case 6: AssignData(ThuNhapLai?.Value7, ThuNhapTuDichVu?.Value7, TrichLap?.Value7, LNSTNH?.Value7); break;
                            case 7: AssignData(ThuNhapLai?.Value8, ThuNhapTuDichVu?.Value8, TrichLap?.Value8, LNSTNH?.Value8); break;
                            default: break;
                        };
                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _nhRepo.Update(entityUpdate);

                        void AssignData(double? thunhaplai, double? thunhaptudichvu, double? trichlap, double? loinhuan)
                        {
                            entityUpdate.rv = (thunhaplai ?? 0) + (thunhaptudichvu ?? 0);
                            entityUpdate.pf = loinhuan ?? 0;
                            entityUpdate.risk = trichlap ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_NganHang_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_NganHang_CIR(string code)
        {
            try
            {
                var batchCount = 8;
                var lReportID = await _apiService.VietStock_CSTC_GetListTempID(code);
                Thread.Sleep(1000);
                var totalCount = lReportID.data.Count();
                lReportID.data = lReportID.data.Where(x => x.YearPeriod >= 2020).ToList();
                var lBatch = new List<List<ReportTempIDDetailResponse>>();
                var lSub = new List<ReportTempIDDetailResponse>();
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
                    strBuilder.Append($"__RequestVerificationToken=KfZYybRs9qYVyNmoczUJUaVItBQB3M64pTTwOaz0XLN7DziURs1EoHjiUPLcHiAY4OlvMaIMTGzRUiWbjVTqUm3vw0vwAHMoEJbgeqa8NpFi-NBrUMIYHOx4ApBOrenS0&");

                    var count = item.Count();
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0)
                        {
                            strBuilder.Append("&");
                        }
                        var element = item[i];
                        strBuilder.Append($"ListTerms[{i}][ItemId]={element.IdTemp}&");
                        strBuilder.Append($"ListTerms[{i}][YearPeriod]={element.YearPeriod}");
                    }
                    var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
                    var lData = await _apiService.VietStock_GetFinanceIndexDataValue_CSTC_ByListTerms(txt);
                    Thread.Sleep(1000);
                    if (lData is null || lData.data is null)
                        continue;

                    for (int i = 0; i < count; i++)
                    {
                        var element = item[i];

                        var year = element.YearPeriod;
                        var quarter = element.ReportTermID - 1;

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
                            continue;
                        }

                        //
                        var cir = lData?.data.FirstOrDefault(x => x.FinanceIndexID == (int)EFinanceIndex.CIR);

                        switch (i)
                        {
                            case 0: AssignData(cir?.Value1); break;
                            case 1: AssignData(cir?.Value2); break;
                            case 2: AssignData(cir?.Value3); break;
                            case 3: AssignData(cir?.Value4); break;
                            case 4: AssignData(cir?.Value5); break;
                            case 5: AssignData(cir?.Value6); break;
                            case 6: AssignData(cir?.Value7); break;
                            case 7: AssignData(cir?.Value8); break;
                            default: break;
                        };

                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _nhRepo.Update(entityUpdate);

                        void AssignData(double? cir)
                        {
                            entityUpdate.cir_r = cir ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_BatDongSan_KQKD|EXCEPTION| {ex.Message}");
            }
        }


        private List<SubKQKD> _lSubKQKD = new List<SubKQKD>();
        private async Task SubBCTC_KQKD(string code)
        {
            try
            {
                var batchCount = 8;
                var lReportID = await _apiService.VietStock_KQKD_GetListReportData(code);
                Thread.Sleep(1000);
                var totalCount = lReportID.data.Count();
                lReportID.data = lReportID.data.Where(x => x.Isunited == 0 && x.BasePeriodBegin >= 201901).ToList();
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

                        var year = element.YearPeriod;
                        var quarter = element.ReportTermID - 1;
                        var LaiThuan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ThuNhapLaiThuan);

                        switch (i)
                        {
                            case 0: AssignData(LaiThuan?.Value1); break;
                            case 1: AssignData(LaiThuan?.Value2); break;
                            case 2: AssignData(LaiThuan?.Value3); break;
                            case 3: AssignData(LaiThuan?.Value4); break;
                            case 4: AssignData(LaiThuan?.Value5); break;
                            case 5: AssignData(LaiThuan?.Value6); break;
                            case 6: AssignData(LaiThuan?.Value7); break;
                            case 7: AssignData(LaiThuan?.Value8); break;
                            default: break;
                        };

                        void AssignData(double? LaiThuan)
                        {
                            _lSubKQKD.Add(new SubKQKD
                            {
                                d = int.Parse($"{year}{quarter}"),
                                s = code,
                                ThuNhapLaiThuan = LaiThuan ?? 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SubBCTC_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        private List<SubCDKT> _lSubCDKT = new List<SubCDKT>();
        private async Task SubBCTC_CDKT(string code)
        {
            try
            {
                var batchCount = 8;
                var lReportID = await _apiService.VietStock_CDKT_GetListReportData(code);
                Thread.Sleep(1000);
                var totalCount = lReportID.data.Count();
                lReportID.data = lReportID.data.Where(x => x.Isunited == 0 && x.BasePeriodBegin >= 201901).ToList();
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
                    var lData = await _apiService.VietStock_GetReportDataDetailValue_CDKT_ByReportDataIds(txt);
                    Thread.Sleep(1000);
                    if (lData is null || lData.data is null)
                        continue;

                    for (int i = 0; i < count; i++)
                    {
                        var element = item[i];

                        var year = element.YearPeriod;
                        var quarter = element.ReportTermID - 1;
                        var TienGuiNHNN = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TienGuiNHNN);
                        var TienGuiTCTD = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.TienGuiTCTD);
                        var ChoVayTCTD = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChoVayTCTD);
                        var ChungKhoanKD = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChungKhoanKD);
                        var ChoVayKH = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChoVayKH);
                        var ChungKhoanDauTu = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChungKhoanDauTu);
                        var ChungKhoanDaoHan = lData?.data.FirstOrDefault(x => x.ReportnormId == (int)EReportNormId.ChungKhoanDaoHan);

                        switch (i)
                        {
                            case 0: AssignData(TienGuiNHNN?.Value1, TienGuiTCTD?.Value1, ChoVayTCTD?.Value1, ChungKhoanKD?.Value1, ChoVayKH?.Value1, ChungKhoanDauTu?.Value1, ChungKhoanDaoHan?.Value1); break;
                            case 1: AssignData(TienGuiNHNN?.Value2, TienGuiTCTD?.Value2, ChoVayTCTD?.Value2, ChungKhoanKD?.Value2, ChoVayKH?.Value2, ChungKhoanDauTu?.Value2, ChungKhoanDaoHan?.Value2); break;
                            case 2: AssignData(TienGuiNHNN?.Value3, TienGuiTCTD?.Value3, ChoVayTCTD?.Value3, ChungKhoanKD?.Value3, ChoVayKH?.Value3, ChungKhoanDauTu?.Value3, ChungKhoanDaoHan?.Value3); break;
                            case 3: AssignData(TienGuiNHNN?.Value4, TienGuiTCTD?.Value4, ChoVayTCTD?.Value4, ChungKhoanKD?.Value4, ChoVayKH?.Value4, ChungKhoanDauTu?.Value4, ChungKhoanDaoHan?.Value4); break;
                            case 4: AssignData(TienGuiNHNN?.Value5, TienGuiTCTD?.Value5, ChoVayTCTD?.Value5, ChungKhoanKD?.Value5, ChoVayKH?.Value5, ChungKhoanDauTu?.Value5, ChungKhoanDaoHan?.Value5); break;
                            case 5: AssignData(TienGuiNHNN?.Value6, TienGuiTCTD?.Value6, ChoVayTCTD?.Value6, ChungKhoanKD?.Value6, ChoVayKH?.Value6, ChungKhoanDauTu?.Value6, ChungKhoanDaoHan?.Value6); break;
                            case 6: AssignData(TienGuiNHNN?.Value7, TienGuiTCTD?.Value7, ChoVayTCTD?.Value7, ChungKhoanKD?.Value7, ChoVayKH?.Value7, ChungKhoanDauTu?.Value7, ChungKhoanDaoHan?.Value7); break;
                            case 7: AssignData(TienGuiNHNN?.Value8, TienGuiTCTD?.Value8, ChoVayTCTD?.Value8, ChungKhoanKD?.Value8, ChoVayKH?.Value8, ChungKhoanDauTu?.Value8, ChungKhoanDaoHan?.Value8); break;
                            default: break;
                        };

                        void AssignData(double? guiNHNN, double? guiTCTD, double? chovayTCTD, double? ckKD, double? chovayKH, double? ckDauTu, double? ckDaoHan)
                        {
                            _lSubCDKT.Add(new SubCDKT
                            {
                                d = int.Parse($"{year}{quarter}"),
                                s = code,
                                TienGuiNHNN = guiNHNN ?? 0,
                                TienGuiTCTD = guiTCTD ?? 0,
                                ChoVayTCTD = chovayTCTD ?? 0,
                                ChungKhoanKD = ckKD ?? 0,
                                ChoVayKH = chovayKH ?? 0,
                                ChungKhoanDauTu = ckDauTu ?? 0,
                                ChungKhoanDaoHan = ckDaoHan ?? 0,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SubBCTC_KQKD|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SyncBCTC_NganHang_NIM_TinDung(string code)
        {
            try
            {
                await SubBCTC_KQKD(code);
                await SubBCTC_CDKT(code);

                var count = _lSubKQKD.Count();

                for (int i = 0; i < count; i++)
                {
                    var elementKQKD = _lSubKQKD[i];

                    FilterDefinition<Financial_NH> filter = null;
                    var builder = Builders<Financial_NH>.Filter;
                    var lFilter = new List<FilterDefinition<Financial_NH>>
                        {
                            builder.Eq(x => x.s, code),
                            builder.Eq(x => x.d, elementKQKD.d)
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
                        continue;
                    }
                    //
                    var elementKQKD_1 = _lSubKQKD[i - 1];
                    var elementKQKD_2 = _lSubKQKD[i - 2];
                    var elementKQKD_3 = _lSubKQKD[i - 3];


                    //
                    var elementCDKT = _lSubCDKT[i];
                    var elementCDKT_1 = _lSubCDKT[i - 1];
                    var elementCDKT_2 = _lSubCDKT[i - 2];
                    var elementCDKT_3 = _lSubCDKT[i - 3];
                    var TuSo = 4 * (elementKQKD.ThuNhapLaiThuan +
                                    elementKQKD_1.ThuNhapLaiThuan +
                                    elementKQKD_2.ThuNhapLaiThuan +
                                    elementKQKD_3.ThuNhapLaiThuan);

                    var MauSo1 = elementCDKT.TienGuiNHNN + elementCDKT.TienGuiTCTD + elementCDKT.ChoVayTCTD + elementCDKT.ChungKhoanKD + elementCDKT.ChoVayKH + elementCDKT.ChungKhoanDauTu + elementCDKT.ChungKhoanDaoHan;
                    var MauSo2 = elementCDKT_1.TienGuiNHNN + elementCDKT_1.TienGuiTCTD + elementCDKT_1.ChoVayTCTD + elementCDKT_1.ChungKhoanKD + elementCDKT_1.ChoVayKH + elementCDKT_1.ChungKhoanDauTu + elementCDKT_1.ChungKhoanDaoHan;
                    var MauSo3 = elementCDKT_2.TienGuiNHNN + elementCDKT_2.TienGuiTCTD + elementCDKT_2.ChoVayTCTD + elementCDKT_2.ChungKhoanKD + elementCDKT_2.ChoVayKH + elementCDKT_2.ChungKhoanDauTu + elementCDKT_2.ChungKhoanDaoHan;
                    var MauSo4 = elementCDKT_3.TienGuiNHNN + elementCDKT_3.TienGuiTCTD + elementCDKT_3.ChoVayTCTD + elementCDKT_3.ChungKhoanKD + elementCDKT_3.ChoVayKH + elementCDKT_3.ChungKhoanDauTu + elementCDKT_3.ChungKhoanDaoHan;
                    var nim = Math.Round(100 * TuSo / (MauSo1 + MauSo2 + MauSo3 + MauSo4), 1);
                    var tindung = Math.Round(100 * (-1 + elementCDKT.ChoVayKH / elementCDKT_1.ChoVayKH), 1);
                    entityUpdate.nim_r = nim;
                    entityUpdate.credit_r = tindung;

                    entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                    _nhRepo.Update(entityUpdate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncBCTC_BatDongSan_KQKD|EXCEPTION| {ex.Message}");
            }
        }
        
        public class SubKQKD
        {
            public int d { get; set; }
            public string s { get; set; }
            public double ThuNhapLaiThuan { get; set; }
        }

        public class SubCDKT
        {
            public int d { get; set; }
            public string s { get; set; }
            public double TienGuiNHNN { get; set; }
            public double TienGuiTCTD { get; set; }
            public double ChoVayTCTD { get; set; }
            public double ChungKhoanKD { get; set; }
            public double ChoVayKH { get; set; }
            public double ChungKhoanDauTu { get; set; }
            public double ChungKhoanDaoHan { get; set; }
        }
    }
}