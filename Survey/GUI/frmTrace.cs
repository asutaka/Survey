using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Survey.Utils;
using Survey.Models;
using Quartz;
using Survey.Job;
using Survey.Job.ScheduleJob;
using DevExpress.Utils;
using System.Diagnostics;

namespace Survey.GUI
{
    /// <summary>
    /// Form này show những C đang trace, có thêm và xóa C
    /// </summary>
    public partial class frmTrace : XtraForm
    {
        public frmTrace()
        {
            InitializeComponent();
            InitData();
            timer1 = new Timer();
            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000;
            timer1.Start();
        }

        public void ShowForm()
        {
            this.Show();
        }

        public void InitData()
        {
            grid.BeginUpdate();
            grid.DataSource = Startup._lTrace;
            grid.EndUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gridView1.RefreshData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(new frmAddCoin().ShowDialog() == DialogResult.OK)
            {
               Startup.LoadlTrace();
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int[] selRows = ((GridView)grid.MainView).GetSelectedRows();
                if (selRows.Length <= 0)
                    return;
                var entityRow = (TraceCoinModel)(((GridView)grid.MainView).GetRow(selRows[0]));
                if (MessageBox.Show($"Xóa coin {entityRow.Coin} khỏi danh sách Follow?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var userData = 0.LoadJsonFile<UserDataModel>("userdata");
                    var entity = userData.FOLLOW.FirstOrDefault(x => x.Coin.Equals(entityRow.Coin, StringComparison.InvariantCultureIgnoreCase));
                    if (entity != null)
                    {
                        userData.FOLLOW.Remove(entity);
                        userData.UpdateJson("userdata");
                        Startup.LoadlTrace();
                    }
                    Utils.Helper.MesSuccess();
                }
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
                var ea = e as DXMouseEventArgs;
                var info = gridView1.CalcHitInfo(ea.Location);
                if (info.InRow || info.InRowCell)
                {
                    var cellValue = gridView1.GetRowCellValue(info.RowHandle, "Coin").ToString();
                    var sInfo = new ProcessStartInfo($"{settings.ViewWeb.Single}/{cellValue.Replace("USDT", "_USDT")}");
                    Process.Start(sInfo);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"frmTrace.gridView1_DoubleClick|EXCEPTION| {ex.Message}");
            }
        }

        private void frmTrace_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}