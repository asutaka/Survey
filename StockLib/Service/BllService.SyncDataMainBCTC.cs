using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace StockLib.Service
{
    public partial class BllService 
    {
        public async Task SyncDataMainBCTC()
        {
            try
            {
                var lFinancial = _financialRepo.GetAll();
                var lStock = _stockRepo.GetAll();
                var lBDS = lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name == "Xây dựng" || y.name == "Bất động sản")).Select(x => x.s);

                lFinancial = lFinancial.Where(x => lBDS.Any(y => y == x.s)).ToList();

                foreach (var item in lFinancial)
                {
                    var checkUpdate = item.eq > 0;
                    //var checkUpdate = item.rv.va > 0
                    //    && item.pf > 0
                    //    && item.fi.va > 0
                    //    && item.inv.va > 0
                    //    && item.ta > 0
                    //    && item.bp.va > 0
                    //    && item.la.va > 0
                    //    && item.lal.va > 0
                    //    && item.tl > 0
                    //    && item.eq > 0
                    //    && item.ce.va > 0;
                    if (checkUpdate)
                        continue;
                    var year = item.d / 10;
                    var quarter = item.d - year * 10;

                    var path = $"https://static2.vietstock.vn/data/HOSE/{year}/BCTC/VN/QUY%20{quarter}/{item.s}_Baocaotaichinh_Q{quarter}_{year}_Hopnhat.pdf";
                    var stream = await _apiService.BCTCRead(path);
                    if (stream is null)
                    {
                        _logger.LogInformation($"{item.s}| FAIL");
                        continue;
                    }
                    var lText = await pdfTextList(stream);
                    var res = ExtractData(lText);
                    if (item.fi.va == 0 && res.Item1 > 0)
                        item.fi.va = res.Item1;
                    if (item.inv.va == 0 && res.Item2 > 0)
                        item.inv.va = res.Item2;
                    if (item.ta == 0 && res.Item3 > 0)
                        item.ta = res.Item3;
                    if (item.bp.va == 0 && res.Item4 > 0)
                        item.bp.va = res.Item4;
                    if (item.la.va == 0 && res.Item5 > 0)
                        item.la.va = res.Item5;
                    if (item.lal.va == 0 && res.Item6 > 0)
                        item.lal.va = res.Item6;
                    if (item.eq == 0 && res.Item7 > 0)
                        item.eq = res.Item7;
                    if (item.ce.va == 0 && res.Item8 > 0)
                        item.ce.va = res.Item8;

                    checkUpdate = item.fi.va == 0
                        && item.inv.va == 0
                        && item.ta == 0
                        && item.bp.va == 0
                        && item.la.va == 0
                        && item.lal.va == 0
                        && item.tl == 0
                        && item.eq == 0
                        && item.ce.va == 0;
                    if (checkUpdate)
                    {
                        _logger.LogInformation($"{item.s}| Extract FAIL");
                        continue;
                    }
                    item.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();

                    _logger.LogInformation($"{item.s}|OUTPUT: {JsonConvert.SerializeObject(res)}");
                    _financialRepo.Update(item);
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncDataMainBCTC|EXCEPTION| {ex.Message}");
            }
        }

        private async Task<List<string>> pdfTextList(Stream data)
        {
            var reader = new PdfReader(data);
            var lResult = new List<string>();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                lResult.Add(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            reader.Close();
            return lResult;
        }

        private (double, double, double, double, double, double, double, double) ExtractData(List<string> lInput)
        {
            double DauTuTaiChinh = 0, TonKho = 0, TongTaiSan = 0, NguoiMuaTraTienTruoc = 0, NoNganHan = 0, NoDaiHan = 0, VonChuSoHuu = 0, ChiPhiVon = 0;
            int div = 1000000000;
            try
            {
                int indexMain = -1;
                foreach (var item in lInput)
                {
                    indexMain++;
                    var textVietnam = item.FormatVietnamese();
                    var lSplit = textVietnam.Split("\n");
                    foreach (var row in lSplit)
                    {
                        Extract(row, KeyMap.lDauTuTaiChinh, ref DauTuTaiChinh);
                        Extract(row, KeyMap.lTonKho, ref TonKho);
                        Extract(row, KeyMap.lTongTaiSan, ref TongTaiSan);
                        Extract(row, KeyMap.lNguoiMuaTraTienTruoc, ref NguoiMuaTraTienTruoc);
                        Extract(row, KeyMap.lNoNganHan, ref NoNganHan);
                        Extract(row, KeyMap.lNoDaiHan, ref NoDaiHan);
                        Extract(row, KeyMap.lVonChuSoHuu, ref VonChuSoHuu);
                        Extract(row, KeyMap.lChiPhiVon, ref ChiPhiVon);
                    }
                }
                return (DauTuTaiChinh, TonKho, TongTaiSan, NguoiMuaTraTienTruoc, NoNganHan, NoDaiHan, VonChuSoHuu, ChiPhiVon);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.LayDoanhThu|EXCEPTION| {ex.Message}");
            }
            return (0, 0, 0, 0, 0, 0, 0, 0);

            double Extract(string row, List<string> keyMap, ref double valCheck)
            {
                if (valCheck != 0)
                    return valCheck;
                foreach (var itemKey in keyMap)
                {
                    if (row.ToUpper().RemoveSignVietnamese().Contains(itemKey.ToUpper()))
                    {
                        var lSpace = row.Split(" ");
                        foreach (var space in lSpace)
                        {
                            var isValid = double.TryParse(space.Replace(",", "").Replace(".", "").Replace("(", "").Replace(")", ""), out var val);
                            if (isValid && Math.Abs(val) >= 1000)
                            {
                                valCheck = Math.Round(val / div, 1);
                                return valCheck;
                            }
                        }
                    }
                }
                return 0;
            }
        }

        private async Task SyncDataMainBCTCFromWeb_BDS(string code)
        {
            try
            {
                var batchCount = 8;
                var lCode = await _apiService.VietStock_GetListReportNorm_BCTT_ByStockCode(code);
                Thread.Sleep(1000);
                var lReportID = await _apiService.VietStock_BCTT_GetListReportData(code);
                Thread.Sleep(1000);
                var totalCount = lReportID.data.Count();
                lReportID.data = lReportID.data.Where(x => (x.Isunited == 0 || x.Isunited == 1) && x.yearPeriod >= 2020).ToList();
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
                        strBuilder.Append($"listReportDataIds[{i}][YearPeriod]={element.yearPeriod}");
                    }
                    var txt = strBuilder.ToString().Replace("]", "%5D").Replace("[", "%5B");
                    var lData = await _apiService.VietStock_GetReportDataDetailValue_BCTT_ByReportDataIds(txt);
                    Thread.Sleep(1000);
                    if (lData is null || lData.data is null)
                        continue;

                    for (int i = 0; i < count; i++)
                    {
                        var element = item[i];
                        var year = element.yearPeriod;
                        var quarter = element.ReportTermID - 1;
                        FilterDefinition<StockFinancial> filter = null;
                        var builder = Builders<StockFinancial>.Filter;
                        var lFilter = new List<FilterDefinition<StockFinancial>>
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
                        if (lUpdate is null || !lUpdate.Any())
                            continue;
                        var entityUpdate = lUpdate.FirstOrDefault();
                        //
                        var DoanhThu = lData?.data.FirstOrDefault(x => x.ReportnormId == 2216);
                        var LoiNhuan = lData?.data.FirstOrDefault(x => x.ReportnormId == 2212);
                        var ChiPhiTaiChinh = lData?.data.FirstOrDefault(x => x.ReportnormId == 3004);
                        var TonKho = lData?.data.FirstOrDefault(x => x.ReportnormId == 3006);
                        var TongTaiSan = lData?.data.FirstOrDefault(x => x.ReportnormId == 2996);
                        var NoPhaiTra = lData?.data.FirstOrDefault(x => x.ReportnormId == 2997);
                        var VonChu = lData?.data.FirstOrDefault(x => x.ReportnormId == 2998);
                        var GiaVon = lData?.data.FirstOrDefault(x => x.ReportnormId == 2207);

                        if(i == 0)
                        {
                            if((DoanhThu.Value1 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value1 ?? 0;
                            }

                            if ((LoiNhuan.Value1 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value1 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value1 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value1 ?? 0;
                            }

                            if ((TonKho.Value1 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value1 ?? 0;
                            }

                            if ((TongTaiSan.Value1 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value1 ?? 0;
                            }

                            if ((NoPhaiTra.Value1 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value1 ?? 0;
                            }

                            if ((VonChu.Value1 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value1 ?? 0;
                            }

                            if ((GiaVon.Value1 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value1 ?? 0;
                            }
                        }
                        else if (i == 1)
                        {
                            if ((DoanhThu.Value2 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value2 ?? 0;
                            }

                            if ((LoiNhuan.Value2 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value2 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value2 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value2 ?? 0;
                            }

                            if ((TonKho.Value2 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value2 ?? 0;
                            }

                            if ((TongTaiSan.Value2 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value2 ?? 0;
                            }

                            if ((NoPhaiTra.Value2 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value2 ?? 0;
                            }

                            if ((VonChu.Value2 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value2 ?? 0;
                            }

                            if ((GiaVon.Value2 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value2 ?? 0;
                            }
                        }
                        else if (i == 2)
                        {
                            if ((DoanhThu.Value3 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value3 ?? 0;
                            }

                            if ((LoiNhuan.Value3 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value3 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value3 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value3 ?? 0;
                            }

                            if ((TonKho.Value3 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value3 ?? 0;
                            }

                            if ((TongTaiSan.Value3 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value3 ?? 0;
                            }

                            if ((NoPhaiTra.Value3 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value3 ?? 0;
                            }

                            if ((VonChu.Value3 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value3 ?? 0;
                            }

                            if ((GiaVon.Value3 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value3 ?? 0;
                            }
                        }
                        else if (i == 3)
                        {
                            if ((DoanhThu.Value4 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value4 ?? 0;
                            }

                            if ((LoiNhuan.Value4 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value4 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value4 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value4 ?? 0;
                            }

                            if ((TonKho.Value4 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value4 ?? 0;
                            }

                            if ((TongTaiSan.Value4 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value4 ?? 0;
                            }

                            if ((NoPhaiTra.Value4 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value4 ?? 0;
                            }

                            if ((VonChu.Value4 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value4 ?? 0;
                            }

                            if ((GiaVon.Value4 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value4 ?? 0;
                            }
                        }
                        else if (i == 4)
                        {
                            if ((DoanhThu.Value5 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value5 ?? 0;
                            }

                            if ((LoiNhuan.Value5 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value5 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value5 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value5 ?? 0;
                            }

                            if ((TonKho.Value5 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value5 ?? 0;
                            }

                            if ((TongTaiSan.Value5 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value5 ?? 0;
                            }

                            if ((NoPhaiTra.Value5 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value5 ?? 0;
                            }

                            if ((VonChu.Value5 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value5 ?? 0;
                            }

                            if ((GiaVon.Value5 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value5 ?? 0;
                            }
                        }
                        else if (i == 5)
                        {
                            if ((DoanhThu.Value6 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value6 ?? 0;
                            }

                            if ((LoiNhuan.Value6 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value6 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value6 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value6 ?? 0;
                            }

                            if ((TonKho.Value6 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value6 ?? 0;
                            }

                            if ((TongTaiSan.Value6 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value6 ?? 0;
                            }

                            if ((NoPhaiTra.Value6 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value6 ?? 0;
                            }

                            if ((VonChu.Value6 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value6 ?? 0;
                            }

                            if ((GiaVon.Value6 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value6 ?? 0;
                            }
                        }
                        else if (i == 6)
                        {
                            if ((DoanhThu.Value7 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value7 ?? 0;
                            }

                            if ((LoiNhuan.Value7 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value7 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value7 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value7 ?? 0;
                            }

                            if ((TonKho.Value7 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value7 ?? 0;
                            }

                            if ((TongTaiSan.Value7 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value7 ?? 0;
                            }

                            if ((NoPhaiTra.Value7 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value7 ?? 0;
                            }

                            if ((VonChu.Value7 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value7 ?? 0;
                            }

                            if ((GiaVon.Value7 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value7 ?? 0;
                            }
                        }
                        else if (i == 7)
                        {
                            if ((DoanhThu.Value8 ?? 0) > 0)
                            {
                                entityUpdate.rv.va = DoanhThu.Value8 ?? 0;
                            }

                            if ((LoiNhuan.Value8 ?? 0) > 0)
                            {
                                entityUpdate.pf = LoiNhuan.Value8 ?? 0;
                            }

                            if ((ChiPhiTaiChinh.Value8 ?? 0) > 0)
                            {
                                entityUpdate.fi.va = ChiPhiTaiChinh.Value8 ?? 0;
                            }

                            if ((TonKho.Value8 ?? 0) > 0)
                            {
                                entityUpdate.inv.va = TonKho.Value8 ?? 0;
                            }

                            if ((TongTaiSan.Value8 ?? 0) > 0)
                            {
                                entityUpdate.ta = TongTaiSan.Value8 ?? 0;
                            }

                            if ((NoPhaiTra.Value8 ?? 0) > 0)
                            {
                                entityUpdate.tl = NoPhaiTra.Value8 ?? 0;
                            }

                            if ((VonChu.Value8 ?? 0) > 0)
                            {
                                entityUpdate.eq = VonChu.Value8 ?? 0;
                            }

                            if ((GiaVon.Value8 ?? 0) > 0)
                            {
                                entityUpdate.ce.va = GiaVon.Value8 ?? 0;
                            }
                        }

                        entityUpdate.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                        _financialRepo.Update(entityUpdate);
                    }
                }

                var tmp = 1;


               

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.SyncDataMainBCTCFromWeb|EXCEPTION| {ex.Message}");
            }
        }

        public async Task SyncDataMainBCTCFromWeb()
        {
            try
            {
                var lStock = _stockRepo.GetAll();
                var lBDS = lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name == "Xây dựng" || y.name == "Bất động sản")).Select(x => x.s);

                foreach (var item in lBDS)
                {
                    await SyncDataMainBCTCFromWeb_BDS(item);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"BllService.SyncDataMainBCTCFromWeb|EXCEPTION| {ex.Message}");
            }
        }
    }
}