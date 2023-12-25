using Quartz;
using Survey.Utils;
using System;
using System.Linq;

namespace Survey.Job
{
    [DisallowConcurrentExecution]
    public class UpdateDataJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (Startup._lTrace == null
                || !Startup._lTrace.Any()
                || SubcribeJob._binanceTicks == null
                || !SubcribeJob._binanceTicks.Any())
                return;

            var lBinance = SubcribeJob._binanceTicks;
            foreach (var item in Startup._lTrace)
            {
                var entity = lBinance.FirstOrDefault(x => x.Symbol == item.Coin);
                if (entity == null)
                    continue;

                try
                {
                    var price = entity.LastPrice;
                    item.CurValue = price;
                    item.DivValue = price - item.Buy;
                    item.RatioValue = $"{Math.Round(item.DivValue * 100 / item.Buy, 1)}%";
                }
                catch(Exception ex)
                {
                    NLogLogger.PublishException(ex, $"UpdateDataJob.Execute|EXCEPTION| {ex.Message}");
                }
            }
        }
    }
}
