﻿namespace BridgeStock
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
            btnTinhToan = new Button();
            button1 = new Button();
            btnMuaBanNN = new Button();
            fileDialog = new OpenFileDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.AliceBlue;
            groupBox1.Controls.Add(btnTinhToan);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(btnMuaBanNN);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(426, 503);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thông tin";
            // 
            // btnTinhToan
            // 
            btnTinhToan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnTinhToan.Location = new Point(53, 119);
            btnTinhToan.Name = "btnTinhToan";
            btnTinhToan.Size = new Size(320, 47);
            btnTinhToan.TabIndex = 5;
            btnTinhToan.Text = "Tính toán";
            btnTinhToan.UseVisualStyleBackColor = true;
            btnTinhToan.Click += btnTinhToan_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.Location = new Point(53, 414);
            button1.Name = "button1";
            button1.Size = new Size(320, 47);
            button1.TabIndex = 4;
            button1.Text = "Test";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
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
            ClientSize = new Size(426, 503);
            Controls.Add(groupBox1);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnMuaBanNN;
        private OpenFileDialog fileDialog;
        private Button button1;
        private Button btnTinhToan;
    }
}