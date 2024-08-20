using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfficeOpenXml;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TongCucThongKe(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TongCucThongKe);
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
                bool isRead = false;
                var package = new ExcelPackage(stream);
                var lSheet = package.Workbook.Worksheets;
                foreach (var sheet in lSheet)
                {
                    if (_lKhachQuocTe.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().EndsWith(x.ToUpper())))
                    {
                        KhachQuocTe(sheet);
                    }
                }

                void AddDic(string val, int index)
                {
                    if (val.Equals("Q1/2020", StringComparison.OrdinalIgnoreCase))
                    {
                        var check = dic.FirstOrDefault(x => x.Value == val);
                        if (check.Value is not null)
                            return;
                        dic.Add(index, val);
                    }
                }

                //var lData = _fileService.HSX(stream);
                //var lOutput = InsertTuDoanh(lData);

                //strOutput.AppendLine($"[Thông báo] Tự doanh HOSE ngày {dt.ToString("dd/MM/yyyy")}:");
                //var lTopBuy = lOutput.Where(x => x.net > 0).OrderByDescending(x => x.net).Take(10);
                //var lTopSell = lOutput.Where(x => x.net < 0).OrderBy(x => x.net).Take(10);
                //if (lTopBuy.Any())
                //{
                //    strOutput.AppendLine($"*Top mua ròng:");
                //}
                //var index = 1;
                //foreach (var item in lTopBuy)
                //{
                //    item.net = Math.Round(item.net / 1000000, 1);
                //    item.net_pt = Math.Round(item.net_pt / 1000000, 1);
                //    if (item.net == 0)
                //        continue;

                //    var content = $"{index++}. {item.s} (Mua ròng {Math.Abs(item.net).ToString("#,##0.#")} tỷ)";
                //    if (item.net_pt != 0)
                //    {
                //        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                //        content += $" - {buySell_pt} {Math.Abs(item.net_pt).ToString("#,##0.#")} tỷ";
                //    }
                //    strOutput.AppendLine(content);
                //}
                //if (lTopSell.Any())
                //{
                //    strOutput.AppendLine();
                //    strOutput.AppendLine($"*Top bán ròng:");
                //}
                //index = 1;
                //foreach (var item in lTopSell)
                //{
                //    item.net = Math.Round(item.net / 1000000, 1);
                //    item.net_pt = Math.Round(item.net_pt / 1000000, 1);
                //    if (item.net == 0)
                //        continue;

                //    var content = $"{index++}. {item.s} (Bán ròng {Math.Abs(item.net).ToString("#,##0.#")} tỷ)";
                //    if (item.net_pt != 0)
                //    {
                //        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                //        content += $" - {buySell_pt} {Math.Abs(item.net_pt).ToString("#,##0.#")} tỷ";
                //    }
                //    strOutput.AppendLine(content);
                //}

                //_configRepo.InsertOne(new ConfigData
                //{
                //    ty = (int)EConfigDataType.TuDoanhHose,
                //    t = t
                //});

                //return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKe|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private void IIP(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.IIP|EXCEPTION| {ex.Message}");
            }
        }

        private void SanPhamCongNghiep(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"SanPhamCongNghiep.IIP|EXCEPTION| {ex.Message}");
            }
        }

        private void VonDauTuNhaNuoc(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"VonDauTuNhaNuoc.IIP|EXCEPTION| {ex.Message}");
            }
        }

        private void FDI(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.IIP|EXCEPTION| {ex.Message}");
            }
        }

        private void BanLe(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.BanLe|EXCEPTION| {ex.Message}");
            }
        }

        private void XuatKhau(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.XuatKhau|EXCEPTION| {ex.Message}");
            }
        }

        private void NhapKhau(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.NhapKhau|EXCEPTION| {ex.Message}");
            }
        }

        private void CPI(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.CPI|EXCEPTION| {ex.Message}");
            }
        }

        private void VanTaiHangHoa(ExcelWorksheet sheet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.VanTaiHangHoa|EXCEPTION| {ex.Message}");
            }
        }


        private List<string> _lKhachQuocTe = new List<string>
        {
            "KQT",
            "Du Lich"
        };
        private void KhachQuocTe(ExcelWorksheet sheet)
        {
            try
            {
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value;
                        //if (cellValueCur != null)
                        //{
                        //    var val = cellValueCur.ToString().Trim();
                        //    AddDic(val, j);

                        //    if (j == 1 && val.Equals("CASA", StringComparison.OrdinalIgnoreCase))
                        //    {
                        //        isRead = true;
                        //        break;
                        //    }
                        //}
                    }

                    //if (isRead)
                    //{
                    //    foreach (var item in dic)
                    //    {
                    //        var strSplit = item.Value.ToString().Replace("Q", "").Split('/');
                    //        var d = int.Parse($"{strSplit[1]}{strSplit[0]}");
                    //        var res = service.GetFinancial_NH(d, name.Trim().ToUpper());
                    //        if (res is null)
                    //            continue;
                    //        var val = sheet.Cells[i, item.Key].Value;
                    //        var check = double.TryParse(val?.ToString() ?? string.Empty, out var valDouble);
                    //        if (!check)
                    //            continue;

                    //        res.casa_r = Math.Round(valDouble * 100, 1);
                    //        res.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                    //        service.UpdateFinancial_NH(res);
                    //    }
                    //    break;
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"FDI.KhachQuocTe|EXCEPTION| {ex.Message}");
            }
        }
    }
}
