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
        public async Task<(int, string)> TongCucHaiQuan(DateTime dt)
        {
            try
            {
                var mode = EConfigDataType.TongCucHaiQuan;
                var builder = Builders<ConfigData>.Filter;
                var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
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

                //var dic = new Dictionary<int, string>();
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //var package = new ExcelPackage(stream);
                //var lSheet = package.Workbook.Worksheets;
                //foreach (var sheet in lSheet)
                //{
                //    if (false) { }
                //    else if (_lIIP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        IIP(sheet, dt);
                //    }
                //    else if (_lSPCongNghiep.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        SanPhamCongNghiep(sheet, dt);
                //    }
                //    else if (_lVonDauTu.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        VonDauTuNhaNuoc(sheet, dt);
                //    }
                //    else if (_lFDI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        FDI(sheet, dt);
                //    }
                //    else if (_lBanLe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        BanLe(sheet, dt);
                //    }
                //    else if (_lXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        XuatKhau(sheet, dt);
                //    }
                //    else if (_lNK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        NhapKhau(sheet, dt);
                //    }
                //    else if (_lCPI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        CPI(sheet, dt);
                //    }
                //    else if (_lVanTaiHangHoa.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        VanTaiHangHoa(sheet, dt);
                //    }
                //    else if (_lKhachQuocTe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        KhachQuocTe(sheet, dt);
                //    }
                //}

                var mes = TongCucThongKeThangPrint(dt);
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)mode,
                    t = t
                });
                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucHaiQuan|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }
    }
}
