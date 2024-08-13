using Microsoft.Extensions.Logging;
using StockLib.DAL.Entity;
using StockLib.Service.Settings;
using StockLib.Utils;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StockLib.Service
{
    public partial class TeleService
    {
        private TelegramBotClient BotInstance()
        {
            try
            {
                if (_bot == null)
                    _bot = new TelegramBotClient(ServiceSetting._botToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.BotInstance|EXCEPTION| {ex.Message}");
            }

            return _bot;
        }
        private List<Stock> StockInstance()
        {
            if (_lStock != null && _lStock.Any())
                return _lStock;
            _lStock = _stockRepo.GetAll();
            return _lStock;
        }

        private async Task Analyze(long userId, string input)
        {
            var output = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            input = input.Trim();
            if((input.StartsWith("[") && input.EndsWith("]"))
                || (input.StartsWith("*") && input.EndsWith("*"))
                || (input.StartsWith("@") && input.EndsWith("@")))//Nhóm ngành
            {
                input = input.Replace("[", "").Replace("]", "").Replace("*", "").Replace("@", "");
                if(StaticVal._lNganHang.Any(x => x.ToUpper().Equals(input.ToUpper())))//Ngành Ngân Hàng
                {
                    await NganhNganHang(userId);
                    return;
                }
                if (StaticVal._lBatDongSan.Any(x => x.ToUpper().Equals(input.ToUpper())))//Ngành Bất động sản
                {
                    await NganhBatDongSan(userId);
                    return;
                }
            }

            //var entityStock = _lStock.FirstOrDefault(x => x.s.Equals(input.ToUpper()));
            //if (entityStock != null)
            //{
            //    await _bllService.OnlyStock(userId, entityStock, BotInstance());
            //    return;
            //}
            //if (input.ToUpper().Contains("VONHOA_"))//Trả về chart vốn hóa các cp trong nhóm ngành
            //{
            //    var stream = await _bllService.Chart_VonHoa_Category(input);
            //    if (stream is null || stream.Length <= 0)
            //        return;

            //    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
            //    return;
            //}
            //if (input.ToUpper().Contains("LN_"))//Trả về chart tăng trưởng Doanh Thu, Lợi Nhuận các cp trong nhóm ngành
            //{
            //    var stream = await _bllService.Chart_LN_Category(input);
            //    if (stream is null || stream.Length <= 0)
            //        return;

            //    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
            //    return;
            //}

            //if (input.ToUpper().Contains("chart_chienluocdautu".ToUpper())
            //    || input.ToUpper().Contains("chart_cl".ToUpper()))//Chiến lược đầu tư
            //{
            //    var stream = await _bllService.Chart_ChienLuocDauTu();
            //    if (stream is null || stream.Length <= 0)
            //        return;

            //    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
            //    return;
            //}
            //if (input.ToUpper().Contains("chart_".ToUpper()))//Tổng hợp về nhóm ngành
            //{
            //    //Lợi nhuận
            //    var streamLN = await _bllService.Chart_LN_Category(input);
            //    if (streamLN?.Length > 0)
            //    {
            //        await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));
            //    }

            //    //Tồn kho
            //    var streamTonKho = await _bllService.Chart_GG_Category(input, "Tồn kho", "TonKho");
            //    if (streamTonKho?.Length > 0)
            //    {
            //        await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTonKho));
            //    }

            //    //Người mua trả tiền trước
            //    var streamNguoiMua = await _bllService.Chart_GG_Category(input, "Người mua", "NguoiMuaTraTienTruoc");
            //    if (streamNguoiMua?.Length > 0)
            //    {
            //        await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNguoiMua));
            //    }

            //    //Nợ 
            //    return;
            //}
        }

        private async Task NganhNganHang(long userId)
        {
            try
            {
                var lNganHang = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name.Equals("Ngân hàng", StringComparison.OrdinalIgnoreCase))).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_NganHang_DoanhThu_LoiNhuan(lNganHang);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));
                Thread.Sleep(1000);
                //Tăng trưởng tín dụng, room tín dụng
                var streamTinDung = await _bllService.Chart_NganHang_TangTruongTinDung_RoomTinDung(lNganHang);
                if (streamTinDung != null && streamTinDung.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTinDung));
                }
                Thread.Sleep(1000);
                //Nợ xấu, trích lập dự phòng
                var streamNoXau = await _bllService.Chart_NganHang_NoXau(lNganHang);
                if (streamNoXau != null && streamNoXau.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNoXau));
                }

                //Tiết giảm chi phí vốn, Nim, Casa
                var streamNimCasa = await _bllService.Chart_NganHang_NimCasaChiPhiVon(lNganHang);
                if (streamNimCasa != null && streamNimCasa.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNimCasa));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch(Exception ex)
            {
                _logger.LogError($"TeleService.NganhNganHang|EXCEPTION| INPUT: UserID: { userId }|{ex.Message}");
            }
        }

        private async Task NganhBatDongSan(long userId)
        {
            try
            {
                var lBDS = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.name.Equals("Bất động sản", StringComparison.OrdinalIgnoreCase))).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_BDS_DoanhThu_LoiNhuan(lBDS);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhBatDongSan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }
    }
}
