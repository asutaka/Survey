using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_Logistic(string code)
        {
            try
            {
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_DuongBo, 5, 15);
                var gia = ModeThongKe(EKeyTongCucThongKe.QUY_GiaVT_BuuChinh, 5, 15);

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(vt.Item2);
                sBuilder.AppendLine(gia.Item2);

                return (MergeEnpoint(vt.Item1, gia.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Logistic|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
