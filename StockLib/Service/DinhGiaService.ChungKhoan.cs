using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        /// <summary>
        /// - Môi gới: ý tưởng là so sánh khối lượng giao dịch của VNINDEX QoQ
        /// - Vay Margin: ý tưởng là so sánh lãi Margin quý này QoQ và QoQoY
        /// - Tự doanh: Tính lãi tự doanh trung bình 8 quý kết hợp check giá các mã tự doanh
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<(EPoint, string)> DG_ChungKhoan(string code)
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
                var eVNINDEX = EPointResponse(rate, 5, 15, "Khối lượng giao dịch tháng cùng kỳ");
                sBuilder.AppendLine(eVNINDEX.Item2);
                //margin
                var lck = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.s, code));
                if(lck is null || !lck.Any())
                {
                    return (EPoint.Unknown, string.Empty);
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
                    return (EPoint.Unknown, string.Empty);
                }
                var rateDebtQoQoY = Math.Round(100*(-1 + (double)cur.idebt / (double)near.idebt), 1);
                var eDebtQoQoY = EPointResponse(rateDebtQoQoY, 5, 15, "Lợi nhuận Margin quý liền trước");
                var eDebt = eDebtQoQoY;
                sBuilder.AppendLine(eDebt.Item2);

                var prev = lck.Skip(3).FirstOrDefault();
                if (prev is null)
                {
                    return (EPoint.Unknown, string.Empty);
                }
                var rateDebtQoQ = Math.Round(100 * (-1 + (double)cur.idebt / (double)prev.idebt), 1);
                var eDebtQoQ = EPointResponse(rateDebtQoQ, 5, 15, "Lợi nhuận Margin quý cùng kỳ");
                sBuilder.AppendLine(eDebtQoQ.Item2);
                eDebt = (MergeEnpoint(eDebt.Item1, eDebtQoQ.Item1), "");
                //Tự doanh
                var avg = lck.Take(8).Average(x => x.trade);

                var spec = _specRepo.GetEntityByFilter(Builders<SpecialInfo>.Filter.Eq(x => x.s, code));
                if(spec != null)
                {
                    var quarter = dt.GetQuarter();
                    var month = 1;
                    if(quarter == 2)
                    {
                        month = 4;
                    }
                    else if(quarter == 3)
                    {
                        month = 7;
                    }
                    else if(quarter == 4)
                    {
                        month = 10;
                    }

                    double sumrate = 0;
                    double rate_total = 0;
                    foreach (var item in spec.stocks)
                    {
                        sumrate += item.rate / 100;
                        var lData = await _apiService.SSI_GetDataStock(item.s);
                        if(lData is null || !lData.Any())
                            continue;

                        var startVal = lData.Where(x => x.Date.Year == dt.Year && x.Date.Month == month)?.MinBy(x => x.Date);
                        if (startVal is null)
                            continue;

                        var curVal = lData.Last();
                        var rateVal = (double)Math.Round(-1 + curVal.Close / startVal.Close, 1);
                        rate_total += rateVal * item.rate / 100;
                    }
                    sumrate = sumrate * rate_total * rate_tudoanh;
                    avg = Math.Round(avg * (1 + sumrate), 1);
                }
                sBuilder.AppendLine($"   - Tự doanh quý cùng kỳ: {Math.Round(100 * (-1 + avg / prev.trade), 1)}%");

                //Dự đoán
                double rateVNINDEX = 0;
                if(eVNINDEX.Item1 == EPoint.VeryPositive)
                {
                    rateVNINDEX = 0.2;
                }
                else if (eVNINDEX.Item1 == EPoint.Positive)
                {
                    rateVNINDEX = 0.1;
                }
                else if (eVNINDEX.Item1 == EPoint.Negative)
                {
                    rateVNINDEX = -0.1;
                }
                else if (eVNINDEX.Item1 == EPoint.VeryNegative)
                {
                    rateVNINDEX = -0.2;
                }

                double rateMargin = 0;
                if (eDebt.Item1 == EPoint.VeryPositive)
                {
                    rateMargin = 0.2;
                }
                else if (eDebt.Item1 == EPoint.Positive)
                {
                    rateMargin = 0.1;
                }
                else if (eDebt.Item1 == EPoint.Negative)
                {
                    rateMargin = -0.1;
                }
                else if (eDebt.Item1 == EPoint.VeryNegative)
                {
                    rateMargin = -0.2;
                }

                double pf_dudoan = (prev.broker - prev.bcost) * (1 + rateVNINDEX) + prev.idebt * (1 + rateMargin) + avg;
                double pf_sosanh = prev.idebt + prev.trade + prev.broker - prev.bcost;
                var rateResult = Math.Round(100 * (-1 + pf_dudoan / pf_sosanh), 1);
                var eresult = EPointResponse(rateResult, 5, 15, "");
                return (eresult.Item1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_ChungKhoan|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
