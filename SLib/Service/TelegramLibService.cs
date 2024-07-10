using MongoDB.Driver;
using Skender.Stock.Indicators;
using SLib.DAL;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
        private static List<TelegramModel> _lMessage = new List<TelegramModel>();
        private static List<Stock> _lStock = new List<Stock>();
        private object objLock = 1;
        private readonly int _numThread = 1;
        private readonly IStockRepo _stockRepo;
        private readonly IBllService _bllService;
        private readonly IAPIService _apiService;
        private const long _idMain = -1002247826353;

        public TelegramLibService(IStockRepo stockRepo,
                                IBllService bllService,
                                IAPIService apiService)
        {
            _stockRepo = stockRepo;
            StockInstance();
            _bllService = bllService;
            _apiService = apiService;
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
            _lStock = _stockRepo.GetAll();
            return _lStock;
        }

        public async Task BotSyncUpdate()
        {
            await func();

            async Task func()
            {
                var lUpdate = await BotInstance().GetUpdatesAsync();
                if (!lUpdate.Any())
                    return;

                var lUpdateClean = new List<Update>();
                var lGroup = lUpdate.Where(x => x.Message.From.Id != _idMain).GroupBy(x => x.Message.From.Id);
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
                           var entityUser = _lMessage.FirstOrDefault(x => x.UserId == item.Message.From.Id);
                           if (entityUser != null)
                           {
                               if (entityUser.CreateAt >= item.Message.Date)
                                   return;

                               entityUser.CreateAt = item.Message.Date;
                           }
                           else
                           {

                               _lMessage.Add(new TelegramModel
                               {
                                   UserId = item.Message.From.Id,
                                   CreateAt = item.Message.Date
                               });
                           }
                           Monitor.Exit(objLock);
                           //action
                           var mesResult = await Analyze(item.Message.Text);
                           await BotInstance().SendTextMessageAsync(item.Message.From.Id, mesResult.Item2);
                           if(mesResult.Item1 == EMessageMode.OnlyStock)
                           {
                               await BotInstance().SendTextMessageAsync(item.Message.From.Id, await AnalyzeFA(item.Message.Text));
                               await BotInstance().SendTextMessageAsync(item.Message.From.Id, await AnalyzeTA(item.Message.Text));
                           }
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

        private async Task<(EMessageMode,string)> Analyze(string input)
        {
            var output = new StringBuilder();
            //clean
            //find
            input = input.Trim().ToUpper();
            if(input.Length == 3)
            {
                return (EMessageMode.OnlyStock, OnlyStock(input));
            }

            //
            if (input.Equals("[ttd]", StringComparison.OrdinalIgnoreCase))
            {
                output.AppendLine(_bllService.TongTuDoanhStr());
                return (EMessageMode.Other, output.ToString());
            }
            if (input.Equals("[tnn]", StringComparison.OrdinalIgnoreCase))
            {
                output.AppendLine(_bllService.TongGDNNStr());
                return (EMessageMode.Other, output.ToString());
            }
            if (input.Equals("[tttt]", StringComparison.OrdinalIgnoreCase))
            {
                output.AppendLine(_bllService.ThongKeThiTruongStr());
                return (EMessageMode.Other, output.ToString());
            }

            return (EMessageMode.Other, output.ToString());
        }

        private string OnlyStock(string input)
        {
            var output = new StringBuilder();
            var entityStock = _lStock.FirstOrDefault(x => x.s.Equals(input));
            if (entityStock is null)
            {
                output.Append("Không tìm thấy dữ liệu!");
                return output.ToString();
            }

            output.AppendLine($"Mã cổ phiếu: {input}");
            output.AppendLine();
            output.AppendLine(_bllService.TuDoanhBuildStr(input));
            output.AppendLine(_bllService.ForeignBuildStr(input));
            return output.ToString();
        }

        private class TelegramModel
        {
            public DateTime CreateAt { get; set; }
            public long UserId { get; set; }
        }
    }
}
