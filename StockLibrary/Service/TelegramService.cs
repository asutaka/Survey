using MongoDB.Driver;
using StockLibrary.DAL;
using StockLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace StockLibrary.Service
{
    public interface ITelegramService
    {
        Task BotSyncUpdate();
    }
    public class TelegramService : ITelegramService
    {
        private static TelegramBotClient _bot;
        private static List<TelegramModel> _lMessage = new List<TelegramModel>();
        private static List<Stock> _lStock = new List<Stock>();
        private object objLock = 1;
        private readonly int _numThread = 1;
        private readonly IStockMongoRepo _stockRepo;
        private readonly IBllService _bllService;

        public TelegramService(IStockMongoRepo stockRepo,
                                IBllService bllService)
        {
            _stockRepo = stockRepo;
            StockInstance();
            _bllService = bllService;
        }

        private TelegramBotClient BotInstance()
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
            var lUpdate = await BotInstance().GetUpdatesAsync();
            if (!lUpdate.Any())
                return;

            Parallel.ForEach(lUpdate, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
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
                       if (entityUser is not null)
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
                       var mes = await Analyze(item.Message.Text);
                       await BotInstance().SendTextMessageAsync(item.Message.From.Id, mes);
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"TelegramService.BotSyncUpdate|EXCEPTION| {ex.Message}");
                   }
               });
        }

        private async Task<string> Analyze(string input)
        {
            var output = new StringBuilder();
            //clean
            //find
            input = input.Trim().ToUpper();
            if(input.Length == 3)
            {
                return OnlyStock(input);
            }

            //
            if (input.Equals("[ttd]", StringComparison.OrdinalIgnoreCase))
            {
                output.AppendLine(_bllService.TongTuDoanhStr());
                return output.ToString();
            }
            if (input.Equals("[tnn]", StringComparison.OrdinalIgnoreCase))
            {
                output.AppendLine(_bllService.TongGDNNStr());
                return output.ToString();
            }

            return output.ToString();
        }

        private string OnlyStock(string input)
        {
            var output = new StringBuilder();
            var entityStock = _lStock.FirstOrDefault(x => x.MaCK.Equals(input));
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
