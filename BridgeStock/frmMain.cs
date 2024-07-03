using StockLibrary.Service;

namespace BridgeStock
{
    public partial class frmMain : Form
    {
        private readonly IFileService _fileService;
        private readonly IBllService _bllService;
        private readonly ITelegramService _telegramService;
        public frmMain(IFileService fileService,
                       IBllService bllService,
                       ITelegramService telegramService)
        {
            InitializeComponent();
            _fileService = fileService;
            _bllService = bllService;
            _telegramService = telegramService;
        }

        private void btnMuaBanNN_Click(object sender, EventArgs e)
        {
            _bllService.SyncGDNuocNgoai();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _telegramService.BotSyncUpdate();
        }

        private void btnTinhToan_Click(object sender, EventArgs e)
        {
            _bllService.BackgroundWork();
        }
    }
}