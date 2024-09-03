using MongoDB.Driver;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private string TongCucHaiQuanPrint(DateTime dt, bool isXuatKhau)
        {
            var lData = _haiquanRepo.GetAll();
            if (!(lData?.Any() ?? false))
                return string.Empty;
            if (isXuatKhau)
                lData = lData.Where(x => x.key != (int)EHaiQuan.SPSatThep_NK && x.key != (int)EHaiQuan.Oto9Cho_NK && x.key != (int)EHaiQuan.OtoVanTai_NK).ToList();
            else
                lData = lData.Where(x => x.key == (int)EHaiQuan.SPSatThep_NK || x.key == (int)EHaiQuan.Oto9Cho_NK || x.key == (int)EHaiQuan.OtoVanTai_NK).ToList();

            var lTime = lData.Select(x => x.d).Distinct().OrderByDescending(x => x);
            var maxTime = lTime.First();
            var prevTime = lTime.Skip(1).First();
            var year = maxTime / 10000;
            var month = (maxTime - year * 10000) / 100;
            var day = (maxTime - (year * 10000 + month * 100));
            var str = day <= 15 ? "nửa đầu" : "nửa cuối";

            var strBuilder = new StringBuilder();
            var strXuatKhau = isXuatKhau ? "xuất khẩu" : "nhập khẩu";
            strBuilder.AppendLine($"[Thông báo] Thống kê {strXuatKhau} {str} tháng {month}");
            strBuilder.AppendLine("*So với nửa tháng liền trước");
            var lcur = lData.Where(x => x.d == maxTime);
            var lprev = lData.Where(x => x.d == prevTime);
            var index = 1;
            foreach (var item in lcur)
            {
                var prev = lprev.FirstOrDefault(x => x.key == item.key);
                double rate = 0;
                if(prev != null && prev.va > 0)
                {
                    rate = Math.Round(100*(-1 + item.va / prev.va), 1);
                }
                var strRate = rate != 0 ? $"({rate}%)" : string.Empty;
                strBuilder.AppendLine($"{index++}. {((EHaiQuan)item.key).GetDisplayName()}: {item.va}tr USD{strRate}");

                if(item.price > 0)
                {
                    double ratePrice = 0;
                    if (prev != null && prev.price > 0)
                    {
                        ratePrice = Math.Round(100 * (-1 + item.price / prev.price), 1);
                    }
                    var strRatePrice = ratePrice != 0 ? $"({ratePrice}%)" : string.Empty;
                    strBuilder.AppendLine($"    Giá: {item.price}USD{strRatePrice}");
                }
            }
            return strBuilder.ToString();
        }
    }
}
