using Microsoft.Extensions.Logging;
using StockLib.DAL.Entity;
using StockLib.Service.Settings;
using System.Text;
using Telegram.Bot;

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
    }
}
