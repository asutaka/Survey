using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        /// <summary>
        /// - Môi gới: ý tưởng là so sánh khối lượng giao dịch của VNINDEX QoQ
        /// - Vay Margin: ý tưởng là so sánh lãi Margin quý này QoQ và QoQoY
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<string> DG_ChungKhoan(string code)
        {
            try
            {
                var lvnindex = await _apiService.SSI_GetDataStock("VNINDEX");
                lvnindex = lvnindex.OrderBy(x => x.Date).ToList();

                var dt = DateTime.Now;
                var dtCur = dt.AddMonths(-1);
                var dtPrev = dtCur.AddYears(-1);
                var sBuilder = new StringBuilder();

                var volCur = lvnindex.Where(x => x.Date.Year == dtCur.Date.Year && x.Date.Month == dtCur.Date.Month).Sum(x => x.Volume);
                var volPrev = lvnindex.Where(x => x.Date.Year == dtPrev.Date.Year && x.Date.Month == dtPrev.Date.Month).Sum(x => x.Volume);
                double rate = 0;
                if(volPrev > 0)
                {
                    rate = Math.Round(100 * (-1 + (double)volCur / (double)volPrev), 1);
                }
                sBuilder.AppendLine($"VNINDEX Volume(YoY): {rate}%");
                //margin
                var lck = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
                if(lck is null || !lck.Any())
                {
                    return string.Empty;
                }    
                lck = lck.OrderByDescending(x => x.d).ToList();

                var cur = lck.First();
                var pf_moigioi = cur.broker - cur.bcost;
                var pf_sum = pf_moigioi + cur.idebt + cur.trade;
                var rate_moigioi = Math.Round(pf_moigioi / pf_sum, 1);
                var rate_margin = Math.Round(cur.idebt / pf_sum, 1);
                var rate_tudoanh = Math.Round(1 - (rate_moigioi + rate_margin), 1);

                var near = lck.Skip(1).FirstOrDefault();
                if(near is null || near.idebt == 0)
                {
                    return sBuilder.ToString();
                }
                var rateDebtQoQoY = Math.Round(100*(-1 + (double)cur.idebt / (double)near.idebt), 1);

                var prev = lck.Skip(3).FirstOrDefault();
                if (prev is null)
                {
                    return sBuilder.ToString();
                }
                var rateDebtQoQ = Math.Round(100 * (-1 + (double)cur.idebt / (double)prev.idebt), 1);
                sBuilder.AppendLine($"Lợi nhuận cho vay: QoQ: {rateDebtQoQ}%| QoQoY: {rateDebtQoQoY}%");
                
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_ChungKhoan|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }
    }
}
