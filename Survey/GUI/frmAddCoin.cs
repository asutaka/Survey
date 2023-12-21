using DevExpress.XtraEditors;
using Survey.Models;
using Survey.Utils;
using System;
using System.Windows.Forms;

namespace Survey.GUI
{
    /// <summary>
    /// Form thêm mới Coin, có option noti Telegram
    /// </summary>
    public partial class frmAddCoin : XtraForm
    {
        public frmAddCoin()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            cmbCoin.Properties.ValueMember = "S";
            cmbCoin.Properties.DisplayMember = "S";
            cmbCoin.Properties.DataSource = Startup._lCoin;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOkAndSave_Click(object sender, EventArgs e)
        {
            if(!CheckValid())
            {
                MessageBox.Show("Đầu vào không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }    

            var userData = 0.LoadJsonFile<UserDataModel>("userdata");
            userData.FOLLOW.Add(new UserDataCoinModel
            {
                Coin = cmbCoin.EditValue.ToString(),
                Buy = nmBuy.Value,
                Value = nmValue.Value
            });

            userData.UpdateJson("userdata");
            MessageBox.Show("Đã lưu dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private bool CheckValid()
        {
            if(string.IsNullOrWhiteSpace(cmbCoin.EditValue.ToString())
                || nmBuy.Value <= 0
                || nmValue.Value <= 0)
                return false;
            return true;
        }
    }
}