namespace BridgeStock
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            btnTuDoanhHNX = new Button();
            btnTuDoanhHSX2 = new Button();
            btnTuDoanhHSX = new Button();
            btnMuaBanNN = new Button();
            fileDialog = new OpenFileDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.AliceBlue;
            groupBox1.Controls.Add(btnTuDoanhHNX);
            groupBox1.Controls.Add(btnTuDoanhHSX2);
            groupBox1.Controls.Add(btnTuDoanhHSX);
            groupBox1.Controls.Add(btnMuaBanNN);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(426, 450);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thông tin";
            // 
            // btnTuDoanhHNX
            // 
            btnTuDoanhHNX.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTuDoanhHNX.Location = new Point(53, 308);
            btnTuDoanhHNX.Name = "btnTuDoanhHNX";
            btnTuDoanhHNX.Size = new Size(320, 47);
            btnTuDoanhHNX.TabIndex = 3;
            btnTuDoanhHNX.Text = "Tự doanh HNX, Upcom";
            btnTuDoanhHNX.UseVisualStyleBackColor = true;
            btnTuDoanhHNX.Click += btnTuDoanhHNX_Click;
            // 
            // btnTuDoanhHSX2
            // 
            btnTuDoanhHSX2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTuDoanhHSX2.Location = new Point(53, 255);
            btnTuDoanhHSX2.Name = "btnTuDoanhHSX2";
            btnTuDoanhHSX2.Size = new Size(320, 47);
            btnTuDoanhHSX2.TabIndex = 2;
            btnTuDoanhHSX2.Text = "Tự doanh HSX 2";
            btnTuDoanhHSX2.UseVisualStyleBackColor = true;
            btnTuDoanhHSX2.Click += btnTuDoanhHSX2_Click;
            // 
            // btnTuDoanhHSX
            // 
            btnTuDoanhHSX.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTuDoanhHSX.Location = new Point(53, 202);
            btnTuDoanhHSX.Name = "btnTuDoanhHSX";
            btnTuDoanhHSX.Size = new Size(320, 47);
            btnTuDoanhHSX.TabIndex = 1;
            btnTuDoanhHSX.Text = "Tự doanh HSX";
            btnTuDoanhHSX.UseVisualStyleBackColor = true;
            btnTuDoanhHSX.Click += btnTuDoanhHSX_Click;
            // 
            // btnMuaBanNN
            // 
            btnMuaBanNN.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnMuaBanNN.Location = new Point(53, 53);
            btnMuaBanNN.Name = "btnMuaBanNN";
            btnMuaBanNN.Size = new Size(320, 47);
            btnMuaBanNN.TabIndex = 0;
            btnMuaBanNN.Text = "Lấy mua bán NN";
            btnMuaBanNN.UseVisualStyleBackColor = true;
            btnMuaBanNN.Click += btnMuaBanNN_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.AliceBlue;
            ClientSize = new Size(426, 450);
            Controls.Add(groupBox1);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnTuDoanhHNX;
        private Button btnTuDoanhHSX2;
        private Button btnTuDoanhHSX;
        private Button btnMuaBanNN;
        private OpenFileDialog fileDialog;
    }
}