using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<EPoint> DinhGia_Forex(EForex forex, double step1, double step2)
        {
            try
            {
                var lVal = await _apiService.VietStock_GetForex(forex.ToString());
                if (lVal is null || !lVal.t.Any())
                    return EPoint.VeryNegative;

                var c_last = lVal.c.Last();
                var c_near = lVal.c.SkipLast(1).Last();
                var t_prev = ((long)lVal.t.Last()).UnixTimeStampToDateTime().AddYears(-1).AddMonths(1);
                var dtPrev = new DateTime(t_prev.Year, t_prev.Month, 1, 0, 0, 0);
                var timestamp = new DateTimeOffset(dtPrev).ToUnixTimeSeconds();
                var t_index = lVal.t.Where(x => x < timestamp).Max(x => x);
                var index = lVal.t.IndexOf(t_index);
                var c_prev = lVal.c.ElementAt(index);

                var qoq = Math.Round(100 * (-1 + c_last / c_prev), 1);
                var qoqoy = Math.Round(100 * (-1 + c_last / c_near), 1);

                var total_qoq = 0;
                if (qoq > step2)
                {
                    total_qoq = (int)EPoint.VeryPositive;
                }
                else if (qoq <= step2 && qoq > step1)
                {
                    total_qoq = (int)EPoint.Positive;
                }
                else if (qoq <= step1 && qoq >= -step1)
                {
                    total_qoq = (int)EPoint.Normal;
                }
                else if (qoq < -step1 && qoq >= -step2)
                {
                    total_qoq = (int)EPoint.Negative;
                }
                else
                {
                    total_qoq = (int)EPoint.VeryNegative;
                }

                var total_qoqoy = 0;
                if (qoqoy > step2)
                {
                    total_qoqoy = (int)EPoint.VeryPositive;
                }
                else if (qoqoy <= step2 && qoqoy > step1)
                {
                    total_qoqoy = (int)EPoint.Positive;
                }
                else if (qoqoy <= step1 && qoqoy >= -step1)
                {
                    total_qoqoy = (int)EPoint.Normal;
                }
                else if (qoqoy < -step1 && qoqoy >= -step2)
                {
                    total_qoqoy = (int)EPoint.Negative;
                }
                else
                {
                    total_qoqoy = (int)EPoint.VeryNegative;
                }

                var total = total_qoq * 0.6 + total_qoqoy * 0.4;
                if (total < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if (total < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if (total < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if (total < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.Forex|EXCEPTION| {ex.Message}");
            }
            return EPoint.VeryNegative;
        }
    }
}
