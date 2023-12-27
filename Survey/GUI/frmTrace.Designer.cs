
namespace Survey.GUI
{
    partial class frmTrace
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
            DevExpress.XtraGrid.GridFormatRule gridFormatRule3 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet2 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
            DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet2 = new DevExpress.XtraEditors.FormatConditionIconSet();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon5 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon6 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon7 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon8 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
            DevExpress.XtraGrid.GridFormatRule gridFormatRule4 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRule3ColorScale formatConditionRule3ColorScale2 = new DevExpress.XtraEditors.FormatConditionRule3ColorScale();
            this.grid = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Coin = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Value = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Buy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CurValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DivValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RatioValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.Location = new System.Drawing.Point(1, 37);
            this.grid.MainView = this.gridView1;
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(471, 370);
            this.grid.TabIndex = 1;
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
            gridFormatRule3.Name = "FormatRate";
            formatConditionRuleIconSet2.AllowAnimation = DevExpress.Utils.DefaultBoolean.True;
            formatConditionIconSet2.CategoryName = "Directional";
            formatConditionIconSetIcon5.PredefinedName = "Arrows4_1.png";
            formatConditionIconSetIcon5.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            formatConditionIconSetIcon5.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon6.PredefinedName = "Arrows4_2.png";
            formatConditionIconSetIcon6.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon7.PredefinedName = "Arrows4_3.png";
            formatConditionIconSetIcon7.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147418112});
            formatConditionIconSetIcon7.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSetIcon8.PredefinedName = "Arrows4_4.png";
            formatConditionIconSetIcon8.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            formatConditionIconSetIcon8.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
            formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon5);
            formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon6);
            formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon7);
            formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon8);
            formatConditionIconSet2.Name = "Arrows4Colored";
            formatConditionIconSet2.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Percent;
            formatConditionRuleIconSet2.IconSet = formatConditionIconSet2;
            gridFormatRule3.Rule = formatConditionRuleIconSet2;
            gridFormatRule4.Name = "Format1";
            formatConditionRule3ColorScale2.AllowAnimation = DevExpress.Utils.DefaultBoolean.True;
            formatConditionRule3ColorScale2.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            formatConditionRule3ColorScale2.MaximumColor = System.Drawing.Color.Green;
            formatConditionRule3ColorScale2.MaximumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale2.MiddleColor = System.Drawing.Color.White;
            formatConditionRule3ColorScale2.MiddleType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale2.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            formatConditionRule3ColorScale2.MinimumColor = System.Drawing.Color.Red;
            formatConditionRule3ColorScale2.MinimumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
            formatConditionRule3ColorScale2.PredefinedName = "Green, White, Red";
            gridFormatRule4.Rule = formatConditionRule3ColorScale2;
            this.gridView1.FormatRules.Add(gridFormatRule3);
            this.gridView1.FormatRules.Add(gridFormatRule4);
            this.gridView1.GridControl = this.grid;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
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
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 32);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // frmTrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 408);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.grid);
            this.Name = "frmTrace";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Theo dõi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTrace_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn STT;
        private DevExpress.XtraGrid.Columns.GridColumn Coin;
        private DevExpress.XtraGrid.Columns.GridColumn Buy;
        private DevExpress.XtraGrid.Columns.GridColumn Value;
        private DevExpress.XtraGrid.Columns.GridColumn DivValue;
        private DevExpress.XtraGrid.Columns.GridColumn RatioValue;
        private DevExpress.XtraGrid.Columns.GridColumn CurValue;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private System.Windows.Forms.Timer timer1;
    }
}