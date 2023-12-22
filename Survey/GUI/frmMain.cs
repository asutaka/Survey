using DevExpress.XtraBars;
using Newtonsoft.Json;
using Survey.Job;
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
    /// <summary>
    /// Form này show ra các C có tín hiệu tốt, sắp xếp theo mức gia tăng giá, có nút thêm C trace
    /// </summary>
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                return;
            }    
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void notifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Right)
            {
                return;
            }

            var resources = new ComponentResourceManager(typeof(frmMain));
            PopupMenu popup = new PopupMenu();
            popup.Manager = barManager1;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            var userData = 0.LoadJsonFile<UserDataModel>("userdata");
            foreach (var item in userData.FOLLOW)
            {
                var btnItem = new BarStaticItem();
                btnItem.Caption = item.Coin;
                btnItem.Tag = item.Coin;
                btnItem.ItemClick += new ItemClickEventHandler(this.btnItem_ItemClick);
                popup.ItemLinks.Add(btnItem);
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            //btnExit
            var itemExit = new BarStaticItem { Caption = "Exit" };
            itemExit.ImageOptions.Image = ((Image)(resources.GetObject("btnExit.ImageOptions.Image")));
            itemExit.ImageOptions.LargeImage = ((Image)(resources.GetObject("btnExit.ImageOptions.LargeImage")));
            itemExit.ItemClick += new ItemClickEventHandler(this.btnExit_ItemClick);
            popup.ItemLinks.Add(itemExit);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            popup.ShowPopup(MousePosition);
        }

        private void btnItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            new frmChart(e.Item.Tag.ToString()).Show();
        }

        private void btnExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Startup._jMng != null)
            {
                Startup._jMng.Stop();
            }
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Startup._jMng != null)
            {
                Startup._jMng.Stop();
            }    
        }
    }
}
