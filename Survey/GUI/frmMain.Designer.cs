
namespace Survey.GUI
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraGrid.GridFormatRule gridFormatRule1 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet1 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
            DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet1 = new DevExpress.XtraEditors.FormatConditionIconSet();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon1 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraGrid.GridFormatRule gridFormatRule2 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRule3ColorScale formatConditionRule3ColorScale1 = new DevExpress.XtraEditors.FormatConditionRule3ColorScale();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnFollow = new DevExpress.XtraEditors.SimpleButton();
            this.grid = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Coin = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Value = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Buy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CurValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DivValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RatioValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnAnalyze = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFollow
            // 
            this.btnFollow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFollow.Location = new System.Drawing.Point(1, 2);
            this.btnFollow.Name = "btnFollow";
            this.btnFollow.Size = new System.Drawing.Size(75, 32);
            this.btnFollow.TabIndex = 8;
            this.btnFollow.Text = "Theo dõi";
            this.btnFollow.Click += new System.EventHandler(this.btnFollow_Click);
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.Location = new System.Drawing.Point(1, 35);
            this.grid.MainView = this.gridView1;
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(651, 360);
            this.grid.TabIndex = 7;
            this.grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.STT,
            this.Coin,
            this.Value,
            this.Buy,
            this.CurValue,
            this.DivValue,
            this.RatioValue});
            gridFormatRule1.Name = "FormatRate";
            formatConditionRuleIconSet1.AllowAnimation = DevExpress.Utils.DefaultBoolean.True;
            formatConditionIconSet1.CategoryName = "Directional";
            formatConditionIconSetIcon1.PredefinedName = "Arrows4_1.png";
            formatConditionIconSetIcon1.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            formatConditionIconSetIcon1.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon2.PredefinedName = "Arrows4_2.png";
            formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon3.PredefinedName = "Arrows4_3.png";
            formatConditionIconSetIcon3.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147418112});
            formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon4.PredefinedName = "Arrows4_4.png";
            formatConditionIconSetIcon4.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon1);
            formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon2);
            formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon3);
            formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon4);
            formatConditionIconSet1.Name = "Arrows4Colored";
            formatConditionIconSet1.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Percent;
            formatConditionRuleIconSet1.IconSet = formatConditionIconSet1;
            gridFormatRule1.Rule = formatConditionRuleIconSet1;
            gridFormatRule2.Name = "Format1";
            formatConditionRule3ColorScale1.AllowAnimation = DevExpress.Utils.DefaultBoolean.True;
            formatConditionRule3ColorScale1.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            formatConditionRule3ColorScale1.MaximumColor = System.Drawing.Color.Green;
            formatConditionRule3ColorScale1.MaximumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale1.MiddleColor = System.Drawing.Color.White;
            formatConditionRule3ColorScale1.MiddleType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale1.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            formatConditionRule3ColorScale1.MinimumColor = System.Drawing.Color.Red;
            formatConditionRule3ColorScale1.MinimumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale1.PredefinedName = "Green, White, Red";
            gridFormatRule2.Rule = formatConditionRule3ColorScale1;
            this.gridView1.FormatRules.Add(gridFormatRule1);
            this.gridView1.FormatRules.Add(gridFormatRule2);
            this.gridView1.GridControl = this.grid;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // STT
            // 
            this.STT.AppearanceCell.Options.UseTextOptions = true;
            this.STT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.STT.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.STT.Caption = "STT";
            this.STT.FieldName = "STT";
            this.STT.MaxWidth = 45;
            this.STT.MinWidth = 45;
            this.STT.Name = "STT";
            this.STT.OptionsColumn.AllowEdit = false;
            this.STT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.STT.Visible = true;
            this.STT.VisibleIndex = 0;
            this.STT.Width = 45;
            // 
            // Coin
            // 
            this.Coin.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.Coin.AppearanceCell.ForeColor = System.Drawing.Color.Green;
            this.Coin.AppearanceCell.Options.UseFont = true;
            this.Coin.AppearanceCell.Options.UseForeColor = true;
            this.Coin.AppearanceCell.Options.UseTextOptions = true;
            this.Coin.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Coin.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Coin.AppearanceHeader.Options.UseTextOptions = true;
            this.Coin.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Coin.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Coin.Caption = "Coin";
            this.Coin.FieldName = "Coin";
            this.Coin.MaxWidth = 90;
            this.Coin.MinWidth = 90;
            this.Coin.Name = "Coin";
            this.Coin.OptionsColumn.AllowEdit = false;
            this.Coin.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.Coin.Visible = true;
            this.Coin.VisibleIndex = 1;
            this.Coin.Width = 90;
            // 
            // Value
            // 
            this.Value.AppearanceCell.Options.UseTextOptions = true;
            this.Value.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Value.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Value.AppearanceHeader.Options.UseTextOptions = true;
            this.Value.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Value.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Value.Caption = "Giá trị";
            this.Value.FieldName = "Value";
            this.Value.MaxWidth = 75;
            this.Value.MinWidth = 75;
            this.Value.Name = "Value";
            this.Value.Visible = true;
            this.Value.VisibleIndex = 2;
            // 
            // Buy
            // 
            this.Buy.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Buy.AppearanceCell.Options.UseBackColor = true;
            this.Buy.AppearanceCell.Options.UseTextOptions = true;
            this.Buy.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Buy.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Buy.AppearanceHeader.Options.UseTextOptions = true;
            this.Buy.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Buy.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Buy.Caption = "Giá mua";
            this.Buy.DisplayFormat.FormatString = "#,##0.0";
            this.Buy.FieldName = "Buy";
            this.Buy.MaxWidth = 75;
            this.Buy.MinWidth = 75;
            this.Buy.Name = "Buy";
            this.Buy.OptionsColumn.AllowEdit = false;
            this.Buy.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.Buy.Visible = true;
            this.Buy.VisibleIndex = 3;
            // 
            // CurValue
            // 
            this.CurValue.AppearanceCell.Options.UseTextOptions = true;
            this.CurValue.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.CurValue.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.CurValue.AppearanceHeader.Options.UseTextOptions = true;
            this.CurValue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.CurValue.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.CurValue.Caption = "Giá hiện tại";
            this.CurValue.DisplayFormat.FormatString = "\"#,##0.0\"";
            this.CurValue.FieldName = "CurValue";
            this.CurValue.MaxWidth = 75;
            this.CurValue.MinWidth = 75;
            this.CurValue.Name = "CurValue";
            this.CurValue.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.CurValue.Visible = true;
            this.CurValue.VisibleIndex = 4;
            // 
            // DivValue
            // 
            this.DivValue.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.DivValue.AppearanceCell.Options.UseBackColor = true;
            this.DivValue.AppearanceCell.Options.UseTextOptions = true;
            this.DivValue.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.DivValue.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.DivValue.AppearanceHeader.Options.UseTextOptions = true;
            this.DivValue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.DivValue.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.DivValue.Caption = "Thay đổi";
            this.DivValue.DisplayFormat.FormatString = "\"#,##0.0\"";
            this.DivValue.FieldName = "DivValue";
            this.DivValue.MaxWidth = 80;
            this.DivValue.MinWidth = 80;
            this.DivValue.Name = "DivValue";
            this.DivValue.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.DivValue.Width = 80;
            // 
            // RatioValue
            // 
            this.RatioValue.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.RatioValue.AppearanceCell.Options.UseBackColor = true;
            this.RatioValue.AppearanceCell.Options.UseTextOptions = true;
            this.RatioValue.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.RatioValue.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.RatioValue.AppearanceHeader.Options.UseTextOptions = true;
            this.RatioValue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.RatioValue.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.RatioValue.Caption = "Gia tăng(%)";
            this.RatioValue.FieldName = "RatioValue";
            this.RatioValue.MaxWidth = 85;
            this.RatioValue.MinWidth = 85;
            this.RatioValue.Name = "RatioValue";
            this.RatioValue.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.RatioValue.Visible = true;
            this.RatioValue.VisibleIndex = 5;
            this.RatioValue.Width = 85;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.Text = "Tools";
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(653, 20);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 376);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(653, 20);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 20);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 356);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(653, 20);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 356);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAnalyze.Location = new System.Drawing.Point(82, 2);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(75, 32);
            this.btnAnalyze.TabIndex = 13;
            this.btnAnalyze.Text = "Phân tích";
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 396);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.btnFollow);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnFollow;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn STT;
        private DevExpress.XtraGrid.Columns.GridColumn Coin;
        private DevExpress.XtraGrid.Columns.GridColumn Value;
        private DevExpress.XtraGrid.Columns.GridColumn Buy;
        private DevExpress.XtraGrid.Columns.GridColumn CurValue;
        private DevExpress.XtraGrid.Columns.GridColumn DivValue;
        private DevExpress.XtraGrid.Columns.GridColumn RatioValue;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SimpleButton btnAnalyze;
    }
}