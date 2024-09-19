using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_CangBien(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_DuongBien, step1, step2);
                var gia = ModeThongKe(EKeyTongCucThongKe.QUY_GiaVT_KhoBai, step1, step2);

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(vt.Item2);
                sBuilder.AppendLine(gia.Item2);

                return (MergeEnpoint(vt.Item1, gia.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_CangBien|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
