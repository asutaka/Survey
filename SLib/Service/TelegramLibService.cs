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
        private static List<Stock> _lStock = new List<Stock>();
        private static List<UserMessage> _lUserMes = new List<UserMessage>();
        private object objLock = 1;
        private readonly int _numThread = 1;
        private readonly IBllService _bllService;
        private readonly IAPIService _apiService;
        private readonly IUserMessageRepo _userMessageRepo;
        //private const long _idChannel = -1002247826353;
        //private const long _idUser = 1066022551;
        //private const long _idGroup = -4237476810;

        public TelegramLibService(
                                IBllService bllService,
                                IUserMessageRepo userMessageRepo,
                                IAPIService apiService)
        {
            _bllService = bllService;
            _apiService = apiService;
            _userMessageRepo = userMessageRepo;
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
                           var mesResult = await Analyze(item.Message.Text);
                           await BotInstance().SendTextMessageAsync(item.Message.From.Id, mesResult.Item2);
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

        private async Task<(EMessageMode,string)> Analyze(string input)
        {
            var entityStock = _lStock.FirstOrDefault(x => x.s.Equals(input.Trim().ToUpper()));
            if(entityStock != null)
            {
                return (EMessageMode.OnlyStock, await OnlyStock(entityStock));
            }
    
            var output = new StringBuilder();
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

            return (EMessageMode.Other, output.ToString());
        }

        private async Task<string> OnlyStock(Stock entity)
        {
            return await _bllService.OnlyStock(entity);
        }
    }
}
