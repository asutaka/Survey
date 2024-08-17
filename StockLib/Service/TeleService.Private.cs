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
                if (StaticVal._lChungKhoan.Any(x => x.ToUpper().Equals(input.ToUpper())))//Ngành Chứng khoán
                {
                    await NganhChungKhoan(userId);
                    return;
                }
                if (StaticVal._lThep.Any(x => x.ToUpper().Equals(input.ToUpper())))//Ngành Thép
                {
                    await NganhThep(userId);
                    return;
                }
                if (StaticVal._lBanLe.Any(x => x.ToUpper().Equals(input.ToUpper())))//Ngành Bán Lẻ
                {
                    await NganhBanLe(userId);
                    return;
                }
                if ("VIN".ToUpper().Equals(input.ToUpper()))//Chỉ số cổ phiếu VIN Group
                {
                    await VIN_INDEX(userId);
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
                var lNganHang = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "8300")).OrderByDescending(x => x.p.lv).Select(x => x.s);
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

        private async Task NganhBatDongSan(long userId)
        {
            try
            {
                var lBDS = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "2357"
                                                                        || y.code == "8600")).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_BDS_DoanhThu_LoiNhuan(lBDS);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                Thread.Sleep(1000);
                //Tồn kho
                var streamTonKho = await _bllService.Chart_BDS_TonKho(lBDS);
                if (streamTonKho != null && streamTonKho.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTonKho));
                }

                Thread.Sleep(1000);
                //Người mua trả tiền trước
                var streamNguoiMua = await _bllService.Chart_BDS_NguoiMua(lBDS);
                if (streamNguoiMua != null && streamNguoiMua.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNguoiMua));
                }

                Thread.Sleep(1000);
                //Nợ trên vốn chủ sở hữu
                var streamNo = await _bllService.Chart_BDS_NoTrenVonChu(lBDS);
                if (streamNo != null && streamNo.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNo));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhBatDongSan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task VIN_INDEX(long userId)
        {
            try
            {
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_VIN_DoanhThu_LoiNhuan();
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                Thread.Sleep(1000);
                //Tồn kho
                var streamTonKho = await _bllService.Chart_VIN_TonKho();
                if (streamTonKho != null && streamTonKho.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTonKho));
                }

                Thread.Sleep(1000);
                //Người mua trả tiền trước
                var streamNguoiMua = await _bllService.Chart_VIN_NguoiMua();
                if (streamNguoiMua != null && streamNguoiMua.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNguoiMua));
                }

                Thread.Sleep(1000);
                //Nợ trên vốn chủ sở hữu
                var streamNo = await _bllService.Chart_VIN_NoTrenVonChu();
                if (streamNo != null && streamNo.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNo));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.VIN_INDEX|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhChungKhoan(long userId)
        {
            try
            {
                var lMaCK = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "8777")).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_CK_DoanhThu_LoiNhuan(lMaCK);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                Thread.Sleep(1000);
                //Tăng trưởng Margin
                var streamMargin = await _bllService.Chart_CK_TangTruongTinDung_RoomTinDung(lMaCK);
                if (streamMargin != null && streamMargin.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamMargin));
                }

                Thread.Sleep(1000);
                //Thống kê Môi giới
                var streamMoiGioi = await _bllService.Chart_CK_MoiGioi(lMaCK);
                if (streamMoiGioi != null && streamMoiGioi.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamMoiGioi));
                }

                Thread.Sleep(1000);
                //Thống kê tự doanh
                var streamTuDoanh = await _bllService.Chart_CK_TuDoanh(lMaCK);
                if (streamTuDoanh != null && streamTuDoanh.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTuDoanh));
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
                var lMaCK = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "1757")).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_Thep_DoanhThu_LoiNhuan(lMaCK);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                Thread.Sleep(1000);
                //Tồn kho
                var streamTonKho = await _bllService.Chart_Thep_TonKho(lMaCK);
                if (streamTonKho != null && streamTonKho.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTonKho));
                }

                Thread.Sleep(1000);
                //Nợ trên vốn chủ sở hữu
                var streamNo = await _bllService.Chart_Thep_NoTrenVonChu(lMaCK);
                if (streamNo != null && streamNo.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNo));
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
                var lMaCK = _lStock.Where(x => x.status == 1 && x.h24.Any(y => y.code == "5379"
                                                                                || y.code == "3530"
                                                                                || y.code == "3577")).OrderByDescending(x => x.p.lv).Select(x => x.s);
                //Doanh Thu, Loi Nhuan
                var streamLN = await _bllService.Chart_BanLe_DoanhThu_LoiNhuan(lMaCK);
                if (streamLN is null || streamLN.Length <= 500)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamLN));

                Thread.Sleep(1000);
                //Tồn kho
                var streamTonKho = await _bllService.Chart_BanLe_TonKho(lMaCK);
                if (streamTonKho != null && streamTonKho.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamTonKho));
                }

                Thread.Sleep(1000);
                //Nợ trên vốn chủ sở hữu
                var streamNo = await _bllService.Chart_BanLe_NoTrenVonChu(lMaCK);
                if (streamNo != null && streamNo.Length > 500)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(streamNo));
                }

                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhBanLe|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }
    }
}

/// Kết luận
/// Doanh thu: càng cao càng tốt
/// LNST: càng cao càng tốt
/// Trích lập dự phòng: càng nhỏ càng tốt
/// CIR: càng nhỏ càng tốt
/// Nim: càng cao càng tốt
/// Tăng trưởng tín dụng: càng cao càng tốt,
/// Giảm chi phí vốn: càng nhỏ càng tốt
/// Casa: càng cao càng tốt
/// Tỉ lệ nợ xấu: càng nhỏ càng tốt
/// Tăng trưởng nợ xấu: càng nhỏ càng tốt
/// Bao phủ nợ xấu: càng cao càng tốt
/// <returns></returns>