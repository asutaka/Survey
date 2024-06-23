
namespace SurveyStock
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnReadBuySell = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTuDoanhHSX2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnTuDoanhHSX1 = new DevExpress.XtraEditors.SimpleButton();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnTuDoanhHNX = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReadBuySell
            // 
            this.btnReadBuySell.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadBuySell.Location = new System.Drawing.Point(34, 47);
            this.btnReadBuySell.Name = "btnReadBuySell";
            this.btnReadBuySell.Size = new System.Drawing.Size(355, 46);
            this.btnReadBuySell.TabIndex = 0;
            this.btnReadBuySell.Text = "Lấy Mua Bán NN";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.groupBox1.Controls.Add(this.btnTuDoanhHNX);
            this.groupBox1.Controls.Add(this.btnTuDoanhHSX2);
            this.groupBox1.Controls.Add(this.btnTuDoanhHSX1);
            this.groupBox1.Controls.Add(this.btnReadBuySell);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 403);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "thông tin";
            // 
            // btnTuDoanhHSX2
            // 
            this.btnTuDoanhHSX2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTuDoanhHSX2.Location = new System.Drawing.Point(34, 217);
            this.btnTuDoanhHSX2.Name = "btnTuDoanhHSX2";
            this.btnTuDoanhHSX2.Size = new System.Drawing.Size(355, 46);
            this.btnTuDoanhHSX2.TabIndex = 2;
            this.btnTuDoanhHSX2.Text = "Tự doanh HSX 2";
            this.btnTuDoanhHSX2.Click += new System.EventHandler(this.btnTuDoanhHSX2_Click);
            // 
            // btnTuDoanhHSX1
            // 
            this.btnTuDoanhHSX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTuDoanhHSX1.Location = new System.Drawing.Point(34, 165);
            this.btnTuDoanhHSX1.Name = "btnTuDoanhHSX1";
            this.btnTuDoanhHSX1.Size = new System.Drawing.Size(355, 46);
            this.btnTuDoanhHSX1.TabIndex = 1;
            this.btnTuDoanhHSX1.Text = "Tự doanh HSX";
            this.btnTuDoanhHSX1.Click += new System.EventHandler(this.btnTuDoanhHSX1_Click);
            // 
            // fileDialog
            // 
            this.fileDialog.Filter = "Pdf Files|*.pdf";
            // 
            // btnTuDoanhHNX
            // 
            this.btnTuDoanhHNX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTuDoanhHNX.Location = new System.Drawing.Point(34, 269);
            this.btnTuDoanhHNX.Name = "btnTuDoanhHNX";
            this.btnTuDoanhHNX.Size = new System.Drawing.Size(355, 46);
            this.btnTuDoanhHNX.TabIndex = 3;
            this.btnTuDoanhHNX.Text = "Tự doanh HNX, Upcom";
            this.btnTuDoanhHNX.Click += new System.EventHandler(this.btnTuDoanhHNX_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 410);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnReadBuySell;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnTuDoanhHSX1;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private DevExpress.XtraEditors.SimpleButton btnTuDoanhHSX2;
        private DevExpress.XtraEditors.SimpleButton btnTuDoanhHNX;
    }
}