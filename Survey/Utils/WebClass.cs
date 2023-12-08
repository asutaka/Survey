using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Survey.Utils
{
    public static class WebClass
    {
        private static HttpClient _client10s;
        private static HttpClient _client;

        private static HttpClient HttpGetInstance()
        {
            if (_client == null)
            {
                _client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                _client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
            }
            return _client;
        }

        private static HttpClient Http10sGetInstance()
        {
            if (_client10s == null)
            {
                _client10s = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                _client10s.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
            }
            return _client10s;
        }

        public static async Task<string> GetWebContent10s(string url)
        {
            // Thiết lập các Header nếu cần
            try
            {
                // Thực hiện truy vấn GET
                HttpResponseMessage response = Http10sGetInstance().GetAsync(url).GetAwaiter().GetResult();

                // Hiện thị thông tin header trả về
                //ShowHeaders(response.Headers);

                // Phát sinh Exception nếu mã trạng thái trả về là lỗi
                response.EnsureSuccessStatusCode();

                //Console.WriteLine($"Tải thành công - statusCode {(int)response.StatusCode} {response.ReasonPhrase}");

                //Console.WriteLine("Starting read data");

                // Đọc nội dung content trả về - ĐỌC CHUỖI NỘI DUNG
                string htmltext = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Nhận được {htmltext.Length} ký tự");
                //Console.WriteLine();
                return htmltext;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"StaticClass.GetWebContent10s|EXCEPTION| INPUT: {url}| {ex.Message}");
                return null;
            }
        }

        public static async Task<string> GetWebContent(string url)
        {
            // Thiết lập các Header nếu cần
            try
            {
                // Thực hiện truy vấn GET
                HttpResponseMessage response = HttpGetInstance().GetAsync(url).GetAwaiter().GetResult();

                // Hiện thị thông tin header trả về
                //ShowHeaders(response.Headers);

                // Phát sinh Exception nếu mã trạng thái trả về là lỗi
                response.EnsureSuccessStatusCode();

                //Console.WriteLine($"Tải thành công - statusCode {(int)response.StatusCode} {response.ReasonPhrase}");

                //Console.WriteLine("Starting read data");

                // Đọc nội dung content trả về - ĐỌC CHUỖI NỘI DUNG
                string htmltext = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Nhận được {htmltext.Length} ký tự");
                //Console.WriteLine();
                return htmltext;

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"StaticClass.GetWebContent|EXCEPTION| INPUT: {url}| {ex.Message}");
                return null;
            }
        }

        /// In ra thông tin các Header của HTTP Response
        public static void ShowHeaders(HttpHeaders headers)
        {
            Console.WriteLine("CÁC HEADER:");
            foreach (var header in headers)
            {
                foreach (var value in header.Value)
                {
                    Console.WriteLine($"{header.Key,25} : {value}");

                }
            }
            Console.WriteLine();
        }
    }
}
