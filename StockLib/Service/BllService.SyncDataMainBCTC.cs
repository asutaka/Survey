using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Utils;

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
                _logger.LogError($"BllService.BCTCRead|EXCEPTION| {ex.Message}");
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
    }
}