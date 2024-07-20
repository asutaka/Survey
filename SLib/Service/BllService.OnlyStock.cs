using SLib.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<string> OnlyStock(Stock entity)
        {
            var output = new StringBuilder();
            output.AppendLine($"Mã cổ phiếu: {entity.s}");
            output.AppendLine($"Tên: {entity.p.n.Replace("Công ty", "").Replace("Cổ phần", "").Trim()}");
            output.AppendLine($"Ngành: {entity.h24.Last().name}");
            //API
            var dt = DateTime.Now;
            var dtFirstWeek = dt.AddDays(-(int)dt.DayOfWeek);
            var lDataStock = await _apiService.GetDataStock(entity.s);
            var lForeign = await _apiService.GetForeign(entity.s, 1, 1000, dtFirstWeek.ToString("dd/MM/yyyy"), dt.AddDays(1).ToString("dd/MM/yyyy"));
            var lLoiNhuan = await _apiService.ThongKeLoiNhuan(entity.s);
            var KeHoach = await _apiService.GetKeHoachThucHien(entity.s);
            //TA: Giá hiện tại, chỉ báo bắt đáy, ma20, ichi, rsi zero, vol, ema21, ema50, e21cross50
            var entityTA = TA(lDataStock);
            if (entityTA.Item1 > 0)
            {
                output.AppendLine(entityTA.Item2);
            }

            //Thống kê giao dịch: + NN mua bán + Tự doanh + Mua bán chủ động
            var entityTKGD = await ThongKeGD(entity.s, lForeign);
            if (entityTKGD.Item1 > 0)
            {
                output.AppendLine(entityTKGD.Item2);
            }

            //FA: + Lợi nhuận + Kế hoạch năm +BCTC quý x
            var entityFA = await FA(entity.s, lLoiNhuan, KeHoach);
            if (entityFA.Item1 > 0)
            {
                output.AppendLine(entityFA.Item2);
            }

            //Thống kê khác: + Lợi nhuận DN tb năm + Đà tăng giá cp tb năm + buy MAup/sell MAdown
            var entityKhac = await ThongKeKhac(entity, lDataStock, KeHoach);
            if (entityKhac.Item1 > 0)
            {
                output.AppendLine(entityKhac.Item2);
            }

            //Chuyên sâu: + Cơ cấu lợi nhuận + Phân tích lợi nhuận + Động lực tăng trưởng 
            var entityChuyenSau = await PTChuyenSau(entity.s);
            if (entityChuyenSau.Item1 > 0)
            {
                output.AppendLine(entityChuyenSau.Item2);
            }
            return output.ToString();
        }
    }
}
