using DevExpress.XtraEditors;
using Survey.Models;
using Survey.Utils;
using System;
using System.Linq;
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
            var coin = cmbCoin.EditValue.ToString();
            var buy = nmBuy.Value;
            var val = nmValue.Value;

            var userData = 0.LoadJsonFile<UserDataModel>("userdata");
            var entityExists = userData.FOLLOW.FirstOrDefault(x => x.Coin == coin);
            if (entityExists != null)
            {
                userData.FOLLOW.Remove(entityExists);
                entityExists.Buy = Math.Round(((entityExists.Value + val) * entityExists.Buy * buy) / (buy * entityExists.Value + entityExists.Buy * val), 6);
                entityExists.Value += nmValue.Value;
                userData.FOLLOW.Add(entityExists);
            }
            else
            {
                userData.FOLLOW.Add(new UserDataCoinModel
                {
                    Coin = coin,
                    Buy = buy,
                    Value = val
                });
            }
            userData.UpdateJson("userdata");
            Utils.HelperUtils.MesSuccess();
            DialogResult = DialogResult.OK;
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