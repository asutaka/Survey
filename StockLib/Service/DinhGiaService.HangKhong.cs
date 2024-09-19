using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_HangKhong(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_HangKhong, step1, step2);
                var hk   = ModeThongKe(EKeyTongCucThongKe.HanhKhach_HangKhong, step1, step2);
                var giave   = ModeThongKe(EKeyTongCucThongKe.QUY_GiaVT_HangKhong, step1, step2);
                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(vt.Item2);
                sBuilder.AppendLine(hk.Item2);
                sBuilder.AppendLine(giave.Item2);

                return (MergeEnpoint(vt.Item1, hk.Item1, giave.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_HangKhong|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
