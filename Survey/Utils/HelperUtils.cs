using Survey.Models;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Survey.Utils
{
    public class HelperUtils
    {
        public static void MesSuccess(string mes = "", string title = "")
        {
            var mesContent = "Đã lưu dữ liệu!";
            var mesTitle = "Phản hồi";
            if (!string.IsNullOrWhiteSpace(mes))
            {
                mesContent = mes;
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                mesTitle = title;
            }
            MessageBox.Show(mesContent, mesTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MesError(string mes = "", string title = "")
        {
            var mesContent = "Lỗi không xác định!";
            var mesTitle = "Cảnh báo";
            if (!string.IsNullOrWhiteSpace(mes))
            {
                mesContent = mes;
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                mesTitle = title;
            }
            MessageBox.Show(mesContent, mesTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void OpenLink(string coin)
        {
            try
            {
                var formatCoin = coin.Replace("USDT", "_USDT");
                var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
                var sInfo = new ProcessStartInfo($"{settings.ViewWeb.Single}/{formatCoin}");
                Process.Start(sInfo);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"Helper.OpenLink|EXCEPTION| {ex.Message}");
            }
        }
    }
}
