using Microsoft.Extensions.Logging;
using StockLib.DAL;
using StockLib.DAL.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StockLib.Service
{
    public interface ITeleService
    {
        Task BotSyncUpdate();
    }
    public partial class TeleService : ITeleService
    {
        private readonly ILogger<TeleService> _logger;
        private static TelegramBotClient _bot = null;
        private static List<Stock> _lStock = new List<Stock>();
        private static List<UserMessage> _lUserMes = new List<UserMessage>();
        private readonly int _numThread = 1;
        private object objLock = 1;

        private readonly IStockRepo _stockRepo;
        private readonly IUserMessageRepo _userMessageRepo;
        public TeleService(ILogger<TeleService> logger,
                            IStockRepo stockRepo,
                            IUserMessageRepo userMessageRepo)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _userMessageRepo = userMessageRepo;
            StockInstance();
        }

        public async Task BotSyncUpdate()
        {
            await func();
            async Task func()
            {
                #region Các bước chuẩn bị
                var lUpdate = await BotInstance().GetUpdatesAsync();
                if (!lUpdate.Any())
                    return;

                if (!_lUserMes.Any())
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
                #endregion

                Parallel.ForEach(lUpdateClean, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
                   async item =>
                   {
                       try
                       {
                           if (item.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                           {
                               return;
                           }

                           #region Lưu trữ thời gian mới nhất của từng User 
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
                           #endregion
                           //action
                           await Analyze(item.Message.From.Id, item.Message.Text);
                       }
                       catch (Exception ex)
                       {
                           _logger.LogError($"TeleService.BotSyncUpdate|EXCEPTION| {ex.Message}");
                       }
                   });
            }
        }
    }
}
