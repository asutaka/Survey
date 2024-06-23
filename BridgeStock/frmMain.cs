using StockLibrary.Service;

namespace BridgeStock
{
    public partial class frmMain : Form
    {
        private readonly IFileService _fileService;
        private readonly IBllService _bllService;
        public frmMain(IFileService fileService,
                       IBllService bllService)
        {
            InitializeComponent();
            _fileService = fileService;
            _bllService = bllService;
        }

        private void btnMuaBanNN_Click(object sender, EventArgs e)
        {
            
        }

        private void btnTuDoanhHSX_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lData = _fileService.HSX(fileDialog.FileName);
                    var count = _bllService.InsertTransaction(lData);
                    MessageBox.Show($"Đã lưu dữ liệu({fileDialog.FileName} : {count} bản ghi)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Lỗi không xác định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTuDoanhHSX2_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show($"Đọc file tự doanh HSX bằng phương pháp này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult != DialogResult.Yes)
                return;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lData = _fileService.HSX2(fileDialog.FileName);
                    var count = _bllService.InsertTransaction(lData);
                    MessageBox.Show($"Đã lưu dữ liệu({fileDialog.FileName} : {count} bản ghi)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Lỗi không xác định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTuDoanhHNX_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lData = _fileService.HNX(fileDialog.FileName);
                    var count = _bllService.InsertTransaction(lData);
                    MessageBox.Show($"Đã lưu dữ liệu({fileDialog.FileName} : {count} bản ghi)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Lỗi không xác định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}