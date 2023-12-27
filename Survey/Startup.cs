using Newtonsoft.Json;
using Quartz;
using Survey.Job;
using Survey.Job.ScheduleJob;
using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey
{
    public class Startup
    {
        public static ScheduleMng _jMng = ScheduleMng.Instance();
        public static List<CryptonDetailDataModel> _lCoin;
        public static List<TraceCoinModel> _lTrace = new List<TraceCoinModel>();

        private static Startup _instance = null;
        public static Startup Instance()
        {
            _instance = _instance ?? new Startup();
            return _instance;
        }

        private Startup()
        {
            var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
            var content = HelperUtils.GetContent(settings.API.Coin);
            if (!string.IsNullOrWhiteSpace(content))
            {
                _lCoin = JsonConvert.DeserializeObject<CryptonDataModel>(content).Data
                           .Where(x => x.S.EndsWith("USDT")
                                   && !x.S.EndsWith("UPUSDT")
                                   && !x.S.EndsWith("DOWNUSDT"))
                           .OrderBy(x => x.S).ToList();
            }
            LoadlTrace();

            _jMng.AddSchedule(new ScheduleMember(ScheduleMng.Instance().GetScheduler(), JobBuilder.Create<SubcribeJob>(), "0/10 * * * * ?", nameof(SubcribeJob)));
            _jMng.AddSchedule(new ScheduleMember(ScheduleMng.Instance().GetScheduler(), JobBuilder.Create<UpdateDataJob>(), "* * * * * ?", nameof(UpdateDataJob)));
            _jMng.StartAllJob();
        }

        public static void LoadlTrace()
        {
            var userData = 0.LoadJsonFile<UserDataModel>("userdata");
            var index = 1;
            _lTrace.Clear();
            foreach (var item in userData.FOLLOW)
            {
                _lTrace.Add(new TraceCoinModel
                {
                    STT = index++,
                    Coin = item.Coin,
                    Buy = item.Buy,
                    Value = item.Value
                });
            }
        }
    }
}
