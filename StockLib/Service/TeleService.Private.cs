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

        private TelegramBotClient BotCoinInstance()
        {
            try
            {
                if (_botCoin == null)
                    _botCoin = new TelegramBotClient(ServiceSetting._botTokenCoin);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.BotCoinInstance|EXCEPTION| {ex.Message}");
            }

            return _botCoin;
        }
        private async Task PrintImage(string code, long userId, bool isNganh)
        {
            var curDirectory = Directory.GetCurrentDirectory();
            var path = string.Empty;
            if(isNganh)
            {
                path = $"{curDirectory}/Resource/Nganh/{code}.png";
            }
            else
            {
                path = $"{curDirectory}/Resource/{code}.png";
            }
            if (System.IO.File.Exists(path))
            {
                var file = new FileStream(path, FileMode.Open);
                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(file));
            }
        }

        private async Task Analyze(long userId, string input)
        {
            var output = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            input = input.RemoveSpace().ToUpper();
            if((input.StartsWith("[") && input.EndsWith("]"))
                || (input.StartsWith("*") && input.EndsWith("*"))
                || (input.StartsWith("@") && input.EndsWith("@")))//Nhóm ngành
            {
                input = input.Replace("[", "").Replace("]", "").Replace("*", "").Replace("@", "");
                if (StaticVal._lBanLeKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bán Lẻ
                {
                    await PrintImage(EStockType.BanLe.ToString(), userId, true);
                    await NganhBanLe(userId);
                    return;
                }
                if (StaticVal._lBatDongSanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bất động sản
                {
                    await PrintImage(EStockType.BDS.ToString(), userId, true);
                    await NganhBatDongSan(userId);
                    return;
                }
                if (StaticVal._lCangBienKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Cảng biển
                {
                    await PrintImage(EStockType.CangBien.ToString(), userId, true);
                    await NganhCangBien(userId);
                    return;
                }
                if (StaticVal._lCaoSuKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Cao Su
                {
                    await PrintImage(EStockType.CaoSu.ToString(), userId, true);
                    await NganhCaoSu(userId);
                    return;
                }
                if (StaticVal._lChungKhoanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Chứng khoán
                {
                    await PrintImage(EStockType.ChungKhoan.ToString(), userId, true);
                    await NganhChungKhoan(userId);
                    return;
                }
                if (StaticVal._lDetMayKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Dệt may
                {
                    await PrintImage(EStockType.DetMay.ToString(), userId, true);
                    await NganhDetMay(userId);
                    return;
                }
                if (StaticVal._lDienKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Điện
                {
                    await PrintImage(EStockType.NangLuong.ToString(), userId, true);
                    await NganhDien(userId);
                    return;
                }
                if (StaticVal._lGoKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Gỗ
                {
                    await PrintImage(EStockType.Go.ToString(), userId, true);
                    await NganhGo(userId);
                    return;
                }
                if (StaticVal._lHangKhongKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Hàng không
                {
                    await PrintImage(EStockType.HangKhong.ToString(), userId, true);
                    await NganhHangKhong(userId);
                    return;
                }
                if (StaticVal._lLogisticKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Logistic
                {
                    await PrintImage(EStockType.VanTaiBien.ToString(), userId, true);
                    await NganhLogistic(userId);
                    return;
                }
                if (StaticVal._lNganHangKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Ngân Hàng
                {
                    await PrintImage(EStockType.NganHang.ToString(), userId, true);
                    await NganhNganHang(userId);
                    return;
                }
                if (StaticVal._lNhuaKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Nhựa
                {
                    await PrintImage(EStockType.Nhua.ToString(), userId, true);
                    await NganhNhua(userId);
                    return;
                }
                if (StaticVal._lOtoKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ô tô
                {
                    await PrintImage(EStockType.Oto.ToString(), userId, true);
                    await NganhOto(userId);
                    return;
                }
                if (StaticVal._lPhanBonKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Phân bón, hóa chất
                {
                    await PrintImage(EStockType.PhanBon.ToString(), userId, true);
                    await NganhPhanBon(userId);
                    return;
                }
                if (StaticVal._lThanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Than
                {
                    await PrintImage(EStockType.Than.ToString(), userId, true);
                    await NganhThan(userId);
                    return;
                }
                if (StaticVal._lThepKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Thép
                {
                    await PrintImage(EStockType.Thep.ToString(), userId, true);
                    await NganhThep(userId);
                    return;
                }
                if (StaticVal._lThuySanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Thủy sản
                {
                    await PrintImage(EStockType.ThuySan.ToString(), userId, true);
                    await NganhThuySan(userId);
                    return;
                }
                if (StaticVal._lXimangKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Xi măng
                {
                    await PrintImage(EStockType.XiMang.ToString(), userId, true);
                    await NganhXimang(userId);
                    return;
                }
                if (StaticVal._lXayDungKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Xây dựng
                {
                    await PrintImage(EStockType.XayDung.ToString(), userId, true);
                    await NganhXayDung(userId);
                    return;
                }

                if (StaticVal._lKCNKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành KCN
                {
                    await PrintImage(EStockType.KCN.ToString(), userId, true);
                    await NganhKCN(userId);
                    return;
                }

                if (StaticVal._lDauKhiKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Dầu Khí
                {
                    await PrintImage(EStockType.DauKhi.ToString(), userId, true);
                    await NganhDauKhi(userId);
                    return;
                }
            }
            else if(input.Length == 3) //Mã chứng khoán
            {
                await PrintImage(input.ToUpper(), userId, false);
                await MaChungKhoan(userId, input);
            }
            else if(input.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                var curDirectory = Directory.GetCurrentDirectory();
                var path = $"{curDirectory}/Resource/BAF.png";
                if(System.IO.File.Exists(path))
                {
                    var file = new FileStream(path, FileMode.Open);
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(file));
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
                    if (stream is null) continue;
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                var mesDinhGia = await _bllService.Mes_DinhGia(input);
                if (!string.IsNullOrWhiteSpace(mesDinhGia))
                {
                    await BotInstance().SendTextMessageAsync(userId, mesDinhGia);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.MaChungKhoan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhBanLe(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.BanLe)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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

        private async Task NganhBatDongSan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1
                                            && x.cat.Any(x => x.ty == (int)EStockType.BDS))
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

        private async Task NganhCangBien(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.CangBien)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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

        private async Task NganhCaoSu(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lCaoSu;

                var lStream = await _bllService.Chart_CaoSu(lMaCK);
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
                _logger.LogError($"TeleService.NganhCaoSu|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhChungKhoan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1
                                            && x.cat.Any(x => x.ty == (int)EStockType.ChungKhoan))
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

        private async Task NganhDetMay(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.DetMay)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_DetMay(lMaCK);
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
                _logger.LogError($"TeleService.NganhDetMay|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhDien(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && (x.cat.Any(x => x.ty == (int)EStockType.DienGio 
                                                                                        || x.ty == (int)EStockType.DienKhi 
                                                                                        || x.ty == (int)EStockType.DienMatTroi
                                                                                        || x.ty == (int)EStockType.DienThan
                                                                                        || x.ty == (int)EStockType.ThuyDien
                                                                                        || x.ty == (int)EStockType.NangLuong))).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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

        private async Task NganhGo(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.Go)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Go(lMaCK);
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
                _logger.LogError($"TeleService.NganhGo|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhHangKhong(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.HangKhong)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.Logistic || x.ty == (int)EStockType.VanTaiBien)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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

        private async Task NganhNganHang(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.NganHang)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);
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

        private async Task NganhNhua(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lNhua;

                var lStream = await _bllService.Chart_Nhua(lMaCK);
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
                _logger.LogError($"TeleService.NganhNhua|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhOto(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.Oto || x.ty == (int)EStockType.OtoTai)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Oto(lMaCK);
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
                _logger.LogError($"TeleService.NganhOto|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhPhanBon(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lPhanBon;

                var lStream = await _bllService.Chart_PhanBon(lMaCK);
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
                _logger.LogError($"TeleService.NganhPhanBon|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhThan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.Than)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_Than(lMaCK);
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
                _logger.LogError($"TeleService.NganhThan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhThep(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.Thep)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

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

        private async Task NganhThuySan(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lThuySan;

                var lStream = await _bllService.Chart_ThuySan(lMaCK);
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
                _logger.LogError($"TeleService.NganhThuySan|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhXimang(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lXimang;

                var lStream = await _bllService.Chart_Ximang(lMaCK);
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
                _logger.LogError($"TeleService.NganhXimang|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhXayDung(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lXayDung;

                var lStream = await _bllService.Chart_XayDung(lMaCK);
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
                _logger.LogError($"TeleService.NganhXayDung|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhKCN(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lKCN;

                var lStream = await _bllService.Chart_KCN(lMaCK);
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
                _logger.LogError($"TeleService.NganhKCN|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }

        private async Task NganhDauKhi(long userId)
        {
            try
            {
                var lMaCK = StaticVal._lStock.Where(x => x.status == 1 && x.cat.Any(x => x.ty == (int)EStockType.DauKhi)).OrderByDescending(x => x.p.lv).Take(StaticVal._TAKE).Select(x => x.s);

                var lStream = await _bllService.Chart_DauKhi(lMaCK);
                if (lStream is null)
                    return;
                foreach (var stream in lStream)
                {
                    await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                }

                await BotInstance().SendTextMessageAsync(userId, $"Giá dầu thô: https://tradingeconomics.com/commodity/crude-oil");
                await BotInstance().SendTextMessageAsync(userId, $"Chu kỳ kinh tế: https://thetechnicaltraders.com/how-to-tell-if-the-stock-market-is-bullish-or-bearish");
                await BotInstance().SendTextMessageAsync(userId, "done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.NganhDauKhi|EXCEPTION| INPUT: UserID: {userId}|{ex.Message}");
            }
        }
    }
}