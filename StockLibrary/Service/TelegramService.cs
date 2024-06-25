using MongoDB.Driver.Core.Events;
using StockLibrary.DAL;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using static iTextSharp.text.pdf.AcroFields;

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
        private readonly ITuDoanhMongoRepo _tuDoanhRepo;
        private readonly IForeignMongoRepo _foreignRepo;

        public TelegramService(IStockMongoRepo stockRepo,
                                ITuDoanhMongoRepo tuDoanhRepo,
                                IForeignMongoRepo foreignRepo)
        {
            _stockRepo = stockRepo;
            _tuDoanhRepo = tuDoanhRepo;
            _foreignRepo = foreignRepo;
            StockInstance();
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
            var entityStock = _lStock.FirstOrDefault(x => x.MaCK.Equals(input));
            if (entityStock is null)
            {
                output.Append("Không tìm thấy dữ liệu!");
                return output.ToString();
            }

            output.AppendLine($"Mã cổ phiếu: {input}");
            output.AppendLine();
            output.AppendLine(TuDoanhBuildStr(input));

            return output.ToString();
        }

        private string TuDoanhBuildStr(string input)
        {
            var output = new StringBuilder();
            try
            {
                var lTuDoanh = _tuDoanhRepo.GetWithCodeOrderby(1, 1, input, 0);
                if (lTuDoanh is null
                    || !lTuDoanh.Any()
                    || (DateTimeOffset.Now.ToUnixTimeSeconds() - lTuDoanh.FirstOrDefault().d) / 3600 > 36)// cũ hơn một một nhất định
                {
                    output.AppendLine("[Tự doanh] Không có dữ liệu tự doanh");
                    return output.ToString();
                }

                var entity = lTuDoanh.FirstOrDefault();
                var div = entity.kl_mua - entity.kl_ban;
                var mode = div >= 0 ? "Mua ròng" : "Bán ròng";
                output.AppendLine($"[Tự doanh ngày {entity.d.UnixTimeStampToDateTime().ToString("dd/MM/yyyy")}]");
                output.AppendLine($"(MUA: {entity.kl_mua.ToString("#,##0")}|BÁN: {entity.kl_ban.ToString("#,##0")}) ==> {mode} {Math.Abs(div).ToString("#,##0")} cổ phiếu");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"TelegramService.TuDoanhBuildStr|EXCEPTION| {ex.Message}");
            }
            return output.ToString();
        }

        private string ForeignBuildStr(string input)
        {
            var output = new StringBuilder();
            //try
            //{
            //    var lForeign = _foreignRepo.GetWithCodeOrderby(1, 1, input, 0);
            //    if (lTuDoanh is null
            //        || !lTuDoanh.Any()
            //        || (DateTimeOffset.Now.ToUnixTimeSeconds() - lTuDoanh.FirstOrDefault().d) / 3600 > 36)// cũ hơn một một nhất định
            //    {
            //        output.AppendLine("[Tự doanh] Không có dữ liệu tự doanh");
            //        return output.ToString();
            //    }

            //    var entity = lTuDoanh.FirstOrDefault();
            //    var div = entity.kl_mua - entity.kl_ban;
            //    var mode = div >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($"[Tự doanh ngày {entity.d.UnixTimeStampToDateTime().ToString("dd/MM/yyyy")}]");
            //    output.AppendLine($"(MUA: {entity.kl_mua.ToString("#,##0")}|BÁN: {entity.kl_ban.ToString("#,##0")}) ==> {mode} {Math.Abs(div).ToString("#,##0")} cổ phiếu");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"TelegramService.ForeignBuildStr|EXCEPTION| {ex.Message}");
            //}
            return output.ToString();
        }

        private class TelegramModel
        {
            public DateTime CreateAt { get; set; }
            public long UserId { get; set; }
        }
    }
}
