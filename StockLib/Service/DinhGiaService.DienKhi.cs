using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_DienKhi(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                var giadautho = await DinhGia_Forex(EForex.CL, step1, step2);
                var gia = ModeThongKe(EKeyTongCucThongKe.QUY_GiaNVL_Dien, step1, step2);

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(giadautho.Item2);
                sBuilder.AppendLine(gia.Item2);

                return (MergeEnpoint(Swap(giadautho.Item1), gia.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_DienKhi|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
