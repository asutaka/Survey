using DevExpress.Utils;
using DevExpress.XtraBars;
using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Survey.GUI
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        private DateTime? _dtPrev = null;
        private BackgroundWorker _bkgr = new BackgroundWorker();
        private List<TraceViewModel> _lView = new List<TraceViewModel>();
        private Timer _timer = new Timer();
        public frmMain()
        {
            InitializeComponent();
            _bkgr.DoWork += bkgrConfig_DoWork;
            _bkgr.RunWorkerCompleted += bkgrConfig_RunWorkerCompleted;
        }

        private void bkgrConfig_DoWork(object sender, DoWorkEventArgs e)
        {
            _lView = Analyze.AnalyzeViaIchimoku();
        }

        private void bkgrConfig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grid.BeginUpdate();
            grid.DataSource = _lView;
            grid.EndUpdate();
            _timer.Stop();
            lblProgress.Text = StaticVal._strProgressMain;
        }

        private void btnFollow_Click(object sender, EventArgs e)
        {
            new frmTrace(this).Show();
            Hide();
        }

        private void btnItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            HelperUtils.OpenLink(e.Item.Tag.ToString());
        }

        private void btnExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Startup._jMng != null)
            {
                Startup._jMng.Stop();
            }
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            var popup = new PopupMenu
            {
                Manager = barManager1
            };
            foreach (var item in Startup._lTrace)
            {
                var btnItem = new BarStaticItem();
                btnItem.Caption = $"{item.Coin}({item.RatioValue})";
                btnItem.Tag = item.Coin;
                btnItem.ItemClick += new ItemClickEventHandler(this.btnItem_ItemClick);
                popup.ItemLinks.Add(btnItem);
            }
            //btnExit
            var resources = new ComponentResourceManager(typeof(frmMain));
            var itemExit = new BarStaticItem { Caption = "Exit" };
            itemExit.ImageOptions.Image = ((Image)(resources.GetObject("btnExit.ImageOptions.Image")));
            itemExit.ImageOptions.LargeImage = ((Image)(resources.GetObject("btnExit.ImageOptions.LargeImage")));
            itemExit.ItemClick += new ItemClickEventHandler(this.btnExit_ItemClick);
            popup.ItemLinks.Add(itemExit);
            popup.ShowPopup(MousePosition);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Startup._jMng != null)
            {
                Startup._jMng.Stop();
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if(_dtPrev != null 
                && (DateTime.Now - _dtPrev).Value.TotalMinutes < 5)
            {
                HelperUtils.MesError("Chỉ phân tích cách nhau tối thiểu 5 phút!");
                return;
            }
            _timer.Tick += timer1_Tick;
            _timer.Interval = 1000;
            _timer.Start();

            StaticVal._strProgressMain = EProgress.Start.GetDisplayName();
            //background worker
            _bkgr.RunWorkerAsync();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var ea = e as DXMouseEventArgs;
                var info = gridView1.CalcHitInfo(ea.Location);
                if (info.InRow || info.InRowCell)
                {
                    var cellValue = gridView1.GetRowCellValue(info.RowHandle, "Coin").ToString();
                    HelperUtils.OpenLink(cellValue);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"frmMain.gridView1_DoubleClick|EXCEPTION| {ex.Message}");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblProgress.Text = StaticVal._strProgressMain;
        }
    }
}