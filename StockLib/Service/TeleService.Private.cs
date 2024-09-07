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

        private async Task Analyze(long userId, string input)
        {
            var output = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            input = input.Trim().ToUpper();
            if((input.StartsWith("[") && input.EndsWith("]"))
                || (input.StartsWith("*") && input.EndsWith("*"))
                || (input.StartsWith("@") && input.EndsWith("@")))//Nhóm ngành
            {
                input = input.Replace("[", "").Replace("]", "").Replace("*", "").Replace("@", "");
                if(StaticVal._lNganHang.Any(x => x.ToUpper().Equals(input)))//Ngành Ngân Hàng
                {
                    await NganhNganHang(userId);
                    return;
                }
                if (StaticVal._lBatDongSan.Any(x => x.ToUpper().Equals(input)))//Ngành Bất động sản
                {
                    await NganhBatDongSan(userId);
                    return;
                }
                if (StaticVal._lChungKhoan.Any(x => x.ToUpper().Equals(input)))//Ngành Chứng khoán
                {
                    await NganhChungKhoan(userId);
                    return;
                }
                if (StaticVal._lThep.Any(x => x.ToUpper().Equals(input)))//Ngành Thép
                {
                    await NganhThep(userId);
                    return;
                }
                if (StaticVal._lBanLe.Any(x => x.ToUpper().Equals(input)))//Ngành Bán Lẻ
                {
                    await NganhBanLe(userId);
                    return;
                }
                if (StaticVal._lDien.Any(x => x.ToUpper().Equals(input)))//Ngành Điện
                {
                    await NganhDien(userId);
                    return;
                }
                if (StaticVal._lCangBien.Any(x => x.ToUpper().Equals(input)))//Cảng biển
                {
                    await NganhCangBien(userId);
                    return;
                }
                if (StaticVal._lLogistic.Any(x => x.ToUpper().Equals(input)))//Logistic
                {
                    await NganhLogistic(userId);
                    return;
                }
                if (StaticVal._lHangKhong.Any(x => x.ToUpper().Equals(input)))//Hàng không
                {
                    await NganhHangKhong(userId);
                    return;
                }
            }
            else if(input.Length == 3) //Mã chứng khoán
            {
                await MaChungKhoan(userId, input);
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

        private List<Stock> StockInstance()
        {
            if (StaticVal._lStock != null && StaticVal._lStock.Any())
                return StaticVal._lStock;
            StaticVal._lStock = _stockRepo.GetAll();
            return StaticVal._lStock;
        }
        private async Task MaChungKhoan(long userId, string input)
        {
            try
            {
                var lStream = await _bllService.Chart_MaCK(input);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.MaChungKhoan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhBatDongSan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1
                                            && x.h24.Any(y => y.code == "2357"
                                                        || y.code == "8600"))
                                    .Where(x => !StaticVal._lXayDung.Contains(x.s))
                                    .Where(x => !StaticVal._lKCN.Contains(x.s))
                                    .Where(x => !StaticVal._lVin.Contains(x.s))
                                    .Select(x => x.s);

                var lStream = await _bllService.Chart_BatDongSan(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhBatDongSan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhNganHang(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "8300")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);
                var lStream = await _bllService.Chart_NganHang(lMaCK);
                if (lStream is null)
                    return;

                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                //Mô tả 
                var strBuilder = new StringBuilder();
                strBuilder.AppendLine("- Tỷ suất Lợi nhuận(lợi nhuận/doanh thu): càng cao càng tốt");
                strBuilder.AppendLine("- Room tín dụng: càng cao càng tốt");
                strBuilder.AppendLine("- Bao phủ nợ xấu: càng cao càng tốt");
                strBuilder.AppendLine("- Tỉ lệ nợ xấu: càng nhỏ càng tốt");
                strBuilder.AppendLine("- Tăng trưởng nợ nhóm 2: càng nhỏ càng tốt");
                strBuilder.AppendLine("- NIM(Lãi thuần/TS sinh lời bình quân): càng cao càng tốt");
                strBuilder.AppendLine("- CASA(Tiền gửi không kỳ hạn/Tổng tiền gửi): càng cao càng tốt");
                strBuilder.AppendLine("- CIR(chi phí hoạt động/doanh thu hoạt động): càng nhỏ càng tốt");
                strBuilder.AppendLine("- Tăng trưởng chi phí vốn: càng nhỏ càng tốt");
                strBuilder.AppendLine("done!");

                await BotInstance().SendTextMessageAsync(userId, strBuilder.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError($"TeleService.NganhNganHang|EXCEPTION| INPUT: UserID: { userId }|{ex.Message}");
            }
        }

        private async Task NganhChungKhoan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1
                                            && x.h24.Any(y => y.code == "8777"))
                                    .OrderByDescending(x => x.p.lv)
                                    .Take(StaticVal._TAKE)
                                    .Select(x => x.s);

                var lStream = await _bllService.Chart_ChungKhoan(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhChungKhoan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhThep(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "1757")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Thep(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, $"Giá HRC: https://tradingeconomics.com/commodity/hrc-steel");
                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhThep|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhBanLe(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "5379"
                                                                                || y.code == "3530"
                                                                                || y.code == "3577")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_BanLe(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhBanLe|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhDien(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "7535")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Dien(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhDien|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhHangKhong(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "5751")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_HangKhong(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhHangKhong|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhLogistic(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "2773"
                                                                                    || y.code == "2779")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Logistic(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhLogistic|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhCangBien(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "2777")).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_CangBien(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhCangBien|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }
    }
}