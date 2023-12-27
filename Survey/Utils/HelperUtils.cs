using Newtonsoft.Json.Linq;
using Survey.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
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

        public static JArray GetJsonArray(string url)
        {
            if (WebRequest.Create(url) is HttpWebRequest webRequest)
            {
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";
                try
                {
                    using (var s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            var contributorsAsJson = sr.ReadToEnd();
                            var contributors = JArray.Parse(contributorsAsJson);
                            return contributors;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static string GetContent(string url)
        {
            if (WebRequest.Create(url) is HttpWebRequest webRequest)
            {
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";
                try
                {
                    using (var s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            var contributorsAsJson = sr.ReadToEnd();
                            return contributorsAsJson;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
