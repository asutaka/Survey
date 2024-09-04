using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfficeOpenXml;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        //28 -> 5
        public async Task<(int, string)> TongCucThongKeQuy(DateTime dt)
        {
            if((dt.Day <= 5 && dt.Month % 3 == 0)
                || (dt.Day >= 28 && dt.Month % 3 == 1))
            {
                return (0, null);
            }

            if (dt.Day <= 5 && dt.Month % 3 == 1)
            {
                dt = dt.AddMonths(-1);
            }
            var t = long.Parse($"{dt.Year}{dt.GetQuarter()}"); 
                
            try
            {
                var mode = EConfigDataType.TongCucThongKeQuy;
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TongCucThongKe(dt);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, null);

                var dic = new Dictionary<int, string>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(stream);
                var lSheet = package.Workbook.Worksheets;
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (_lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaVanTai(sheet, dt);
                    }
                    else if (_lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaNguyenVatLieu(sheet, dt);
                    }
                }

                var mes = TongCucThongKeQuyPrint(dt);
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)mode,
                    t = t
                });
                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeQuy|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private List<string> _lGiaVT = new List<string>
        {
            "Gia Van Tai",
            "Gia VT"
        };
        private void GiaVanTai(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_Bien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Bien");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_HangKhong, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Hang Khong");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_KhoBai, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Kho Bai");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_BuuChinh, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Buu Chinh");
        }

        private List<string> _lGiaNVL = new List<string>
        {
            "Gia Nguyen Vat Lieu",
            "Gia NVL"
        };
        private void GiaNguyenVatLieu(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaNVL_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Dien");
        }

        private bool InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, string textCompare, string textIgnore = "", int colQoQTry = -1)
        {
            try
            {
                if (colContent <= 0)
                    return false;

                var unitStr = string.Empty;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    var valContent = sheet.Cells[i, colContent].Value?.ToString().Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(valContent))
                        continue;

                    if (!valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textCompare.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    if (!string.IsNullOrWhiteSpace(textIgnore) && valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textIgnore.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    var model = new ThongKeQuy
                    {
                        d = int.Parse($"{dt.Year}{dt.GetQuarter()}"),
                        key = (int)eThongKe
                    };
                    model.content = valContent;

                    if (colVal > 0)
                    {
                        var valStr = sheet.Cells[i, colVal].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.va = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colQoQ > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQ].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoq = isDouble ? Math.Round(val, 1) : 0;

                        if(model.qoq == 0 && colQoQTry > 0)
                        {
                            valStr = sheet.Cells[i, colQoQTry].Value?.ToString().Trim() ?? string.Empty;
                            isDouble = double.TryParse(valStr.Replace(",", ""), out var valTry);
                            model.qoq = isDouble ? Math.Round(valTry, 1) : 0;
                        }
                    }

                    if (colQoQoY > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQoY].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoqoy = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colUnit > 0)
                    {
                        var valStr = sheet.Cells[i, colUnit].Value?.ToString().Trim() ?? string.Empty;//
                        if (string.IsNullOrWhiteSpace(valStr.Replace("'", "").Replace("\"", "")))
                        {
                            valStr = unitStr;
                        }
                        model.unit = valStr;
                        unitStr = valStr;
                    }

                    if (model.va <= 0 && model.qoq <= 0 && model.qoqoy <= 0)
                        continue;

                    _thongkeQuyRepo.InsertOne(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKeOnlyRecord_Quy|EXCEPTION| {ex.Message}");
            }
            return false;
        }
    }
}
