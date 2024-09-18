using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DinhGia_Forex(EForex forex, double step1, double step2)
        {
            try
            {
                var lVal = await _apiService.VietStock_GetForex(forex.ToString());
                if (lVal is null || !lVal.t.Any())
                    return (EPoint.VeryNegative, string.Empty);

                var c_last = lVal.c.Last();
                var t_prev = ((long)lVal.t.Last()).UnixTimeStampToDateTime().AddYears(-1).AddMonths(1);
                var dtPrev = new DateTime(t_prev.Year, t_prev.Month, 1, 0, 0, 0);
                var timestamp = new DateTimeOffset(dtPrev).ToUnixTimeSeconds();
                var t_index = lVal.t.Where(x => x < timestamp).Max(x => x);
                var index = lVal.t.IndexOf(t_index);
                var c_prev = lVal.c.ElementAt(index);

                var qoq = Math.Round(100 * (-1 + c_last / c_prev), 1);

                var total_qoq = 0;
                if (qoq > step2)
                {
                    return (EPoint.VeryPositive, $"   - {forex.GetDisplayName()} QoQ: {qoq}%");
                }
                else if (qoq <= step2 && qoq > step1)
                {
                    return (EPoint.Positive, $"   - {forex.GetDisplayName()} QoQ: {qoq}%");
                }
                else if (qoq <= step1 && qoq >= -step1)
                {
                    return (EPoint.Normal, $"   - {forex.GetDisplayName()} QoQ: {qoq}%");
                }
                else if (qoq < -step1 && qoq >= -step2)
                {
                    return (EPoint.Negative, $"   - {forex.GetDisplayName()} QoQ: {qoq}%");
                }
                else
                {
                    return (EPoint.VeryNegative, $"   - {forex.GetDisplayName()} QoQ: {qoq}%");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.Forex|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
