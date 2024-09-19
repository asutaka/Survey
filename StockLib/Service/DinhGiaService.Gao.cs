using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_Gao(string code)
        {
            try
            {
                var xk = XNK(EStockType.Gao, 5, 15);

                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("rice");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Giá Gạo");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(xk.Item2);
                sBuilder.AppendLine(bdi.Item2);

                return (MergeEnpoint(xk.Item1, bdi.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Gao|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
