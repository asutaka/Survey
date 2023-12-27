using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Linq;
using System.Windows.Forms;
using Survey.Utils;
using Survey.Models;
using DevExpress.Utils;

namespace Survey.GUI
{
    public partial class frmTrace : XtraForm
    {
        private frmMain _frmMain = null;
        public frmTrace(frmMain frm)
        {
            InitializeComponent();
            _frmMain = frm;
            InitData();
            timer1 = new Timer();
            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000;
            timer1.Start();
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
                    HelperUtils.MesSuccess();
                }
            }
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
                NLogLogger.PublishException(ex, $"frmTrace.gridView1_DoubleClick|EXCEPTION| {ex.Message}");
            }
        }

        private void frmTrace_FormClosing(object sender, FormClosingEventArgs e)
        {
            _frmMain.Show();
        }
    }
}