using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        //Xuất khẩu + Đầu tư công
        private (EPoint, string) DG_XiMang(string code)
        {
            try
            {
                //xk
                var xnk = XNK(EStockType.XiMang, 5, 15);
                //ĐTC 
                var dtc = ModeThongKe(EKeyTongCucThongKe.DauTuCong, 5, 15);
                
                var merge = MergeEpoint(xnk.Item1, dtc.Item1);
                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(xnk.Item2);
                sBuilder.AppendLine(dtc.Item2);

                return (merge, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_XiMang|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
