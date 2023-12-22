using System.Windows.Forms;

namespace Survey.Utils
{
    public class Helper
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
    }
}
