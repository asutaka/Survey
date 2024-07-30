using MongoDB.Driver.Linq;
using SLib.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task OnlyStock(long userID, Stock entity, TelegramBotClient bot)
        {
            var output = new StringBuilder();
            try
            {
                output.AppendLine($"Mã cổ phiếu: {entity.s}");
                output.AppendLine($"Tên: {entity.p.n.Replace("Công ty", "").Replace("Cổ phần", "").Trim()}");
                output.AppendLine($"Ngành: {entity.h24.Last().name}");
                //API
                var dt = DateTime.Now;
                var dtWeek = dt.AddDays(-7);
                var lDataStock = await _apiService.GetDataStock(entity.s);
                var lForeign = await _apiService.GetForeign(entity.s, 1, 1000, dtWeek.ToString("dd/MM/yyyy"), dt.AddDays(1).ToString("dd/MM/yyyy"));
                //var lLoiNhuan = await _apiService.ThongKeLoiNhuan(entity.s);
                var KeHoach = await _apiService.GetKeHoachThucHien(entity.s);

                //*Giá hiện tại
                var lastElement = lDataStock.Last();
                var nearLastElement = lDataStock.SkipLast(1).Last();
                var curRate = Math.Round(100 * (-1 + lastElement.Close / nearLastElement.Close), 2);
                output.AppendLine();
                output.AppendLine($"**Giá hiện tại: {lastElement.Close}({curRate}% so với giá tham chiếu)");
                await bot.SendTextMessageAsync(userID, output.ToString());
                output.Clear();
                //*FA
                //Lợi nhuận
                var streamLN = await Chart_LN_Stock(entity.s);
                if (streamLN?.Length > 0)
                {
                    await bot.SendPhotoAsync(userID, InputFile.FromStream(streamLN));
                }

                //Tồn kho
                var streamTonKho = await Chart_GG_Stock(entity.s, "Tồn kho", "TonKho");
                if (streamTonKho?.Length > 0)
                {
                    await bot.SendPhotoAsync(userID, InputFile.FromStream(streamTonKho));
                }

                //Người mua trả tiền trước
                var streamNguoiMua = await Chart_GG_Stock(entity.s, "Người mua", "NguoiMuaTraTienTruoc");
                if (streamNguoiMua?.Length > 0)
                {
                    await bot.SendPhotoAsync(userID, InputFile.FromStream(streamNguoiMua));
                }

                //Kế hoạch năm
                var streamKeHoach = await Chart_KeHoachNam_Stock(entity.s, KeHoach.data);
                if (streamKeHoach?.Length > 0)
                {
                    await bot.SendPhotoAsync(userID, InputFile.FromStream(streamKeHoach));
                }

                //Cơ cấu doanh thu
                var streamCoCau = await Chart_CoCauDoanhThu_Stock(entity.s);
                if (streamCoCau?.Length > 0)
                {
                    await bot.SendPhotoAsync(userID, InputFile.FromStream(streamCoCau));
                }

                //TA: Chỉ báo bắt đáy, ma20, ichi, rsi zero, vol, ema21, ema50, e21cross50
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

                //Thống kê khác: + Lợi nhuận DN tb năm + Đà tăng giá cp tb năm + buy MAup/sell MAdown
                var entityKhac = await ThongKeKhac(entity, lDataStock, KeHoach);
                if (entityKhac.Item1 > 0)
                {
                    output.AppendLine(entityKhac.Item2);
                }

                ////Chuyên sâu: + Cơ cấu doanh thu + Tỷ suất lợi nhuận + người mua trả tiền trước chi tiết + Tồn kho chi tiết(bds: x%)
                //var entityChuyenSau = await PTChuyenSau(entity.s);
                //if (entityChuyenSau.Item1 > 0)
                //{
                //    output.AppendLine(entityChuyenSau.Item2);
                //}

                //Link BCTC
                var entityNguonThamKhao = await NguonThamKhao(entity.s);
                if (entityNguonThamKhao.Item1 > 0)
                {
                    output.AppendLine(entityNguonThamKhao.Item2);
                }

                //end
                await bot.SendTextMessageAsync(userID, output.ToString());
                output.Clear();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }
        }
    }
}
