using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Survey.GUI
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnFollow_Click(object sender, EventArgs e)
        {
            new frmTrace().ShowDialog();
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
            Analyze.AnalyzeViaIchimoku();
        }
    }
}