using Amazon.Auth.AccessControlPolicy;
using Google.Apis.Drive.v3.Data;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using SLib.DAL;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using static iTextSharp.text.pdf.AcroFields;

namespace SLib.Service
{
    public interface ITelegramLibService
    {
        Task BotSyncUpdate();
        TelegramBotClient BotInstance();
    }
    public class TelegramLibService : ITelegramLibService
    {
        private static TelegramBotClient _bot;
        private static List<Stock> _lStock = new List<Stock>();
        private static List<UserMessage> _lUserMes = new List<UserMessage>();
        private object objLock = 1;
        private readonly int _numThread = 1;
        private readonly IBllService _bllService;
        private readonly IAPIService _apiService;
        private readonly IGoogleService _ggService;
        private readonly IUserMessageRepo _userMessageRepo;
        //private const long _idChannel = -1002247826353;
        private const long _idUser = 1066022551;
        //private const long _idGroup = -4237476810;

        public TelegramLibService(
                                IBllService bllService,
                                IUserMessageRepo userMessageRepo,
                                IGoogleService ggService,
                                IAPIService apiService)
        {
            _bllService = bllService;
            _apiService = apiService;
            _userMessageRepo = userMessageRepo;
            _ggService = ggService;
            StockInstance();
        }

        public TelegramBotClient BotInstance()
        {
            try
            {
                if (_bot == null)
                    _bot = new TelegramBotClient(ServiceSetting._botToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TelegramService.BotInstance|EXCEPTION| {ex.Message}");
            }

            return _bot;
        }
        private List<Stock> StockInstance()
        {
            if (_lStock != null && _lStock.Any())
                return _lStock;
            _lStock = _bllService.GetStock();
            return _lStock;
        }
        public async Task BotSyncUpdate()
        {
            await func();

            //var dt = DateTime.Now;
            //var first = StaticVal.lGoogleData?.FirstOrDefault();
            //if(first is null
            //    || first.Time.Day != dt.Day)
            //{
            //    _ggService.GGLoadData();
            //    await BotInstance().SendTextMessageAsync(_idUser, $"{DateTime.Now.ToString("dd/MM/yyyy")}: GGSheet load complete!");
            //}

            async Task func()
            {
                var lUpdate = await BotInstance().GetUpdatesAsync();
                if (!lUpdate.Any())
                    return;

                if(!_lUserMes.Any())
                {
                    _lUserMes = _userMessageRepo.GetAll();
                }

                var lUpdateClean = new List<Update>();
                var lGroup = lUpdate.Where(x => x.Message != null).GroupBy(x => x.Message.From.Id);
                foreach (var item in lGroup)
                {
                    var lItem = item.ToList().OrderByDescending(x => x.Message.Date);
                    lUpdateClean.Add(lItem.First());
                }

                Parallel.ForEach(lUpdateClean, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
                   async item =>
                   {
                       try
                       {
                           if (item.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                           {
                               return;
                           }

                           Monitor.TryEnter(objLock, TimeSpan.FromSeconds(1));
                           var entityUserMes = _lUserMes.FirstOrDefault(x => x.u == item.Message.From.Id);
                           if (entityUserMes != null)
                           {
                               if (entityUserMes.t >= item.Message.Date)
                                   return;

                               entityUserMes.t = item.Message.Date;
                               _userMessageRepo.Update(entityUserMes);
                           }
                           else
                           {
                               var entityMes = new UserMessage
                               {
                                   u = item.Message.From.Id,
                                   t = item.Message.Date
                               };
                               _lUserMes.Add(entityMes);
                               _userMessageRepo.InsertOne(entityMes);
                           }
                           //action
                           await Analyze(item.Message.From.Id, item.Message.Text);
                           //if (mesResult.Item1 == EMessageMode.OnlyStock)
                           //{
                           //    await BotInstance().SendTextMessageAsync(item.Message.From.Id, await AnalyzeFA(item.Message.Text));
                           //    await BotInstance().SendTextMessageAsync(item.Message.From.Id, await AnalyzeTA(item.Message.Text));
                           //}
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine($"TelegramService.BotSyncUpdate|EXCEPTION| {ex.Message}");
                       }
                   });
            }
        }

        private async Task<string> AnalyzeFA(string input)
        {
            var output = new StringBuilder();
            try
            {
                output.AppendLine("[Góc nhìn phân tích cơ bản]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TelegramService.AnalyzeFA|EXCEPTION| {ex.Message}");
            }
            return output.ToString();
        }

        private async Task<string> AnalyzeTA(string input)
        {
            var output = new StringBuilder();
            try
            {
                var lData = await _apiService.GetDataStock(input);
                var ema10 = lData.GetEma(10).Last();
                var ichi = lData.GetIchimoku().Last();
                var bb = lData.GetBollingerBands().Last();
                var macd = lData.GetMacd().Last();

                var entityData = lData.Last();
                output.AppendLine("[Góc nhìn phân tích kỹ thuật]");
                output.AppendLine($"Giá hiện tại: {entityData.Close}");
                output.AppendLine($"EMA10: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                output.AppendLine($"MA20: {(entityData.Close >= (decimal)bb.Sma ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Ichimoku: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Bollinger Band: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"MACD: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với đầu quý: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với ngày ra BCTC quý: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với đáy gần nhất: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với đỉnh gần nhất: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với tín hiệu mua: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
                //output.AppendLine($"Giá so với tín hiệu bán: {(entityData.Close >= (decimal)ema10.Ema ? "Nằm phía trên" : "Nằm phía dưới")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TelegramService.AnalyzeTA|EXCEPTION| {ex.Message}");
            }
            return output.ToString();
        }

        private async Task Analyze(long userId, string input)
        {
            var output = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            input = input.Trim();

            var entityStock = _lStock.FirstOrDefault(x => x.s.Equals(input.ToUpper()));
            if(entityStock != null)
            {
                var mes = await _bllService.OnlyStock(entityStock);
                if (string.IsNullOrWhiteSpace(mes))
                    return;
                await BotInstance().SendTextMessageAsync(userId, mes);
                return;
            }
            if (input.ToUpper().Contains("VONHOA_"))//Trả về chart vốn hóa các cp trong nhóm ngành
            {
                var stream = await _bllService.Chart_VonHoa_Category(input);
                if (stream is null || stream.Length <= 0)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                return;
            }
            if (input.ToUpper().Contains("LN_"))//Trả về chart tăng trưởng Doanh Thu, Lợi Nhuận các cp trong nhóm ngành
            {
                var stream = await _bllService.Chart_LN_Category(input);
                if (stream is null || stream.Length <= 0)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                return;
            }
            if (input.ToUpper().Contains("GGDOANHTHU_"))//Đồng bộ Doanh Thu từ mongo lên file Excel
            {
                var nhomNganh = input.Split("_").Last().Trim();
                var entityHotKey = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Contains(nhomNganh));
                int res = -1;
                if(entityHotKey.Key != null)
                {
                    var entityGG = StaticVal.lHotKeyGG.FirstOrDefault(x => x.Key == entityHotKey.Key);
                    if(entityGG.Key != null)
                    {
                        res = _ggService.GGDoanhThu(entityGG.Value);
                    }
                }

                var mes = res > 0 ? $"Đã đồng bộ Doanh Thu của {input}" : "Dữ liệu nhập vào không đúng";
                await BotInstance().SendTextMessageAsync(userId, mes);
                return;
            }
            if (input.ToUpper().Contains("GGLOINHUAN_"))//Đồng bộ Lợi Nhuận từ mongo lên file Excel
            {
                var nhomNganh = input.Split("_").Last().Trim();
                var entityHotKey = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Contains(nhomNganh));
                int res = -1;
                if (entityHotKey.Key != null)
                {
                    var entityGG = StaticVal.lHotKeyGG.FirstOrDefault(x => x.Key == entityHotKey.Key);
                    if (entityGG.Key != null)
                    {
                        res = _ggService.GGLoiNhuan(entityGG.Value);
                    }
                }

                var mes = res > 0 ? $"Đã đồng bộ Lợi Nhuận của {input}" : "Dữ liệu nhập vào không đúng";
                await BotInstance().SendTextMessageAsync(userId, mes);
                return;
            }
            if (input.ToUpper().Contains("DongBoNgayCongBoBCTC".ToUpper()))//Đồng bộ ngày công bố bctc từ trên web về database
            {
                await _bllService.DongBoNgayCongBoBCTC();
                var mes = "Đã đồng bộ Ngày công bố BCTC từ web về db";
                await BotInstance().SendTextMessageAsync(userId, mes);
                return;
            }
            if (input.ToUpper().Contains("DongBoDoanhThuLoiNhuan".ToUpper()))//Đồng bộ Doanh thu, Lợi nhuận từ trên web về database
            {
                await _bllService.DongBoDoanhThuLoiNhuan();
                var mes = "Đã đồng bộ Doanh thu, Lợi nhuận từ web về db";
                await BotInstance().SendTextMessageAsync(userId, mes);
                return;
            }
            if (input.ToUpper().Contains("chart_chienluocdautu".ToUpper())
                || input.ToUpper().Contains("chart_cl".ToUpper()))//Chiến lược đầu tư
            {
                var stream = await _bllService.Chart_ChienLuocDauTu();
                if (stream is null || stream.Length <= 0)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                return;
            }
            if(input.ToUpper().Contains("chart_"))//Tổng hợp về nhóm ngành
            {
                var stream = await _bllService.Chart_LN_Category(input);
                if (stream is null || stream.Length <= 0)
                    return;

                await BotInstance().SendPhotoAsync(userId, InputFile.FromStream(stream));
                return;
            }

            //
            //if (input.Equals("[ttd]", StringComparison.OrdinalIgnoreCase))
            //{
            //    output.AppendLine(_bllService.TongTuDoanhStr());
            //    return (EMessageMode.Other, output.ToString());
            //}
            //if (input.Equals("[tnn]", StringComparison.OrdinalIgnoreCase))
            //{
            //    output.AppendLine(_bllService.TongGDNNStr());
            //    return (EMessageMode.Other, output.ToString());
            //}
            //if (input.Equals("[tttt]", StringComparison.OrdinalIgnoreCase))
            //{
            //    output.AppendLine(_bllService.ThongKeThiTruongStr());
            //    return (EMessageMode.Other, output.ToString());
            //}
        }
    }
}
