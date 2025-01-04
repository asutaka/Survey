using Microsoft.Extensions.Logging;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StockLib.Service
{
    public interface ITeleService
    {
        Task BotSyncUpdate();
        Task BotCoinSyncUpdate();
        Task SubcribeCoin();
        Task SendTextMessageAsync(long channelID, string mes);
    }
    public partial class TeleService : ITeleService
    {
        private readonly ILogger<TeleService> _logger;
        private static TelegramBotClient _bot = null;
        private static TelegramBotClient _botCoin = null;
        private static List<UserMessage> _lUserMes = new List<UserMessage>();
        private static List<UserMessage> _lUserMesCoin = new List<UserMessage>();
        private readonly int _numThread = 1;
        private object objLock = 1;

        private readonly IStockRepo _stockRepo;
        private readonly IUserMessageRepo _userMessageRepo;
        private readonly IUserMessageCoinRepo _userMessageCoinRepo;
        private readonly IBllService _bllService;
        public TeleService(ILogger<TeleService> logger,
                            IStockRepo stockRepo,
                            IBllService bllService,
                            IUserMessageRepo userMessageRepo,
                            IUserMessageCoinRepo userMessageCoinRepo)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _userMessageRepo = userMessageRepo;
            _bllService = bllService;
            _userMessageCoinRepo = userMessageCoinRepo;
            StockInstance();
        }

        public async Task SendTextMessageAsync(long channelID, string mes)
        {
            await BotInstance().SendTextMessageAsync(channelID, mes);
        }

        public async Task BotSyncUpdate()
        {
            await func();
            async Task func()
            {
                #region Các bước chuẩn bị
                try
                {
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
                catch(Exception ex)
                {
                    _logger.LogError($"TeleService.BotSyncUpdate|EXCEPTION| {ex.Message}");
                }
                
            }
        }
       
        public async Task BotCoinSyncUpdate()
        {
            await func();
            async Task func()
            {
                #region Các bước chuẩn bị
                try
                {
                    var lUpdate = await BotCoinInstance().GetUpdatesAsync();
                    if (!lUpdate.Any())
                        return;

                    if (!_lUserMesCoin.Any())
                    {
                        _lUserMesCoin = _userMessageCoinRepo.GetAll();
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
                               var entityUserMes = _lUserMesCoin.FirstOrDefault(x => x.u == item.Message.From.Id);
                               if (entityUserMes != null)
                               {
                                   if (entityUserMes.t >= item.Message.Date)
                                       return;

                                   entityUserMes.t = item.Message.Date;
                                   _userMessageCoinRepo.Update(entityUserMes);
                               }
                               else
                               {
                                   var entityMes = new UserMessage
                                   {
                                       u = item.Message.From.Id,
                                       t = item.Message.Date
                                   };
                                   _lUserMesCoin.Add(entityMes);
                                   _userMessageCoinRepo.InsertOne(entityMes);
                               }
                               #endregion
                               //action
                               await AnalyzeCoin(item.Message.From.Id, item.Message.Text);
                           }
                           catch (Exception ex)
                           {
                               _logger.LogError($"TeleService.BotCoinSyncUpdate|EXCEPTION| {ex.Message}");
                           }
                       });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"TeleService.BotCoinSyncUpdate|EXCEPTION| {ex.Message}");
                }

            }
        }
    }
}
