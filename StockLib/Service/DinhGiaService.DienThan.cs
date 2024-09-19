using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_DienThan(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("coal");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Giá Than");
                var gia = ModeThongKe(EKeyTongCucThongKe.QUY_GiaNVL_Dien, step1, step2);

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(bdi.Item2);
                sBuilder.AppendLine(gia.Item2);

                return (MergeEnpoint(Swap(bdi.Item1), gia.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_DienThan|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
