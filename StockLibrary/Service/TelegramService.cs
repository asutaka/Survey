using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task BotSyncUpdate()
        {
            try
            {
                //var tmp = await BotInstance().get

                var lUpdate = await BotInstance().GetUpdatesAsync();
                if (lUpdate.Any())
                {
                    foreach (var item in lUpdate)
                    {
                        //await BotInstance().SendTextMessageAsync(item.Message.From.Id, "Hỏi gì anh đấy?");

                        if (item.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                        {
                            var len = item.Message.Text.Length;
                            //if (len >= 10 && len <= 20)
                            //{
                            //    var parsePhone = item.Message.Text.Trim().PhoneFormat(false);
                            //    if (!string.IsNullOrWhiteSpace(parsePhone))
                            //    {
                            //        var phone = long.Parse(parsePhone);
                            //        var id = item.Message.From.Id;
                            //        var entity = await _repo.MatchingRoom_GetRoomMatching(phone);
                            //        if (entity == null)
                            //        {
                            //            await _repo.MatchingRoom_Insert(new MatchingRoom { RoomId = phone, RoomIdMatching = (int)id });
                            //        }
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TelegramService.SendMessageRoomBotTelegram|EXCEPTION| {ex.Message}");
            }
        }

        private async Task BotSendMessage()
        {
            //await BotInstance().SendTextMessageAsync(entityMatching.RoomIdMatching, item.Content);
        }
    }
}
