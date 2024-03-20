using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using Survey.AdapterData;
using Survey.Utils;

namespace Survey.GUI
{

    public partial class FinancialChartingCustom_New : Form {
        const int InitialPointCountOnScreen = 90;
        const int MaxZoomPointCount = 300;
        private string _symbol = string.Empty;
        private BackgroundWorker _bkgr = new BackgroundWorker();

        RealTimeFinancialDataGenerator dataGenerator;
        object selectedObject = null;
        DefaultBoolean crosshairEnabled;

        XYDiagram XYDiagram {
            get { return (XYDiagram)this.chart.Diagram; }
        }
        AxisX AxisX {
            get { return XYDiagram.AxisX; }
        }
        AxisY AxisY {
            get { return XYDiagram.AxisY; }
        }
        Series PriceSeries {
            get { return chart.Series["Price"]; }
        }
        Series VolumeSeries {
            get { return chart.Series["Volume"]; }
        }
        BarSeriesView VolumeSeriesView {
            get { return (BarSeriesView)VolumeSeries.View; }
        }
        bool IsToolbarInteractionEnabled {
            get {
                return trendLineBarCheckItem.Checked || fibbArcBarCheckItem.Checked || fibbFansBarCheckItem.Checked ||
                    fibbRetrBarCheckItem.Checked || removeBarCheckItem.Checked;
            }
        }

        public FinancialChartingCustom_New() {
            InitializeComponent();
            chart.RuntimeHitTesting = true;
            _bkgr.DoWork += bkgrConfig_DoWork;
            _bkgr.RunWorkerCompleted += bkgrConfig_RunWorkerCompleted;
        }

        private void bkgrConfig_DoWork(object sender, DoWorkEventArgs e)
        {
            checkConfig();
        }

        private void bkgrConfig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Interval = int.Parse(barEditInterval.EditValue.ToString());
            timer.Start();
            barBtnStart.Caption = "Stop";
            IsStart = true;
            barBtnStart.ImageOptions.Image = Properties.Resources.pause_16x16;
            barBtnStart.Enabled = true;
        }

        void checkConfig()
        {
            var symbol = bartxtSymbol.EditValue.ToString();
            if (string.IsNullOrWhiteSpace(symbol))
                return;
            _symbol = symbol.Trim();
            if (!symbol.Contains("USDT"))
                symbol = $"{symbol.Trim().ToUpper()}USDT";
            
            chart.BeginInit();
            dataGenerator = new RealTimeFinancialDataGenerator();
            dataGenerator.InitialDataFastAll(symbol, GetInterval());
            InitChartControl();
            chart.EndInit();
            dataGenerator.Start();
        }
        void InitChartControl() {
            chart.DataSource = this.dataGenerator.DataSource;
            PriceSeries.SetFinancialDataMembers("DateTimeStamp", "Low", "High", "Open", "Close");
            VolumeSeries.SetDataMembers("DateTimeStamp", "Volume");
            AxisY.CustomLabels[0].BackColor = chart.GetPaletteEntries(2)[1].Color;
        }

        private EInterval GetInterval()
        {
            switch (AxisX.DateTimeScaleOptions.MeasureUnit)
            {
                case DateTimeMeasureUnit.Minute:
                    return EInterval.I15M;
                case DateTimeMeasureUnit.Hour:
                    {
                        int multiplier = AxisX.DateTimeScaleOptions.MeasureUnitMultiplier;
                        if (multiplier == 1)
                        {
                            return EInterval.I1H;
                        }
                        return EInterval.I4H;
                    }
                case DateTimeMeasureUnit.Day:
                    return EInterval.I1D;
                default:
                    return EInterval.I15M;
            }
        }

        void timer_Tick(object sender, EventArgs e) {
            if (dataGenerator != null)
            {
                dataGenerator.UpdateSource();
            }
            CustomAxisLabel currentValueLabel = AxisY.CustomLabels[0];
            if (PriceSeries.Points.Count > 0)
            {
                FinancialDataCollection dataSource = dataGenerator.DataSource;
                double currentClose = dataSource[dataSource.Count - 1].Close;
                currentValueLabel.AxisValue = currentClose;
                currentValueLabel.Name = string.Format("{0:0.0000}", currentClose);
            }
        }

        void chart_BeforeZoom(object sender, ChartBeforeZoomEventArgs e) {
            if (!(e.Axis is AxisX))
                return;
            double rangeLengthInMeasureUnits = e.NewRange.Max - e.NewRange.Min;
            if (rangeLengthInMeasureUnits > MaxZoomPointCount)
                e.Cancel = true;
        }
        void chart_Zoom(object sender, ChartZoomEventArgs e) {
            double rangeLengthInMeasureUnits = e.NewXRange.Max - e.NewXRange.Min;
            if (rangeLengthInMeasureUnits > 1.2 * InitialPointCountOnScreen)
                VolumeSeriesView.BarWidth = 1;
            else
                VolumeSeriesView.BarWidth = 0.6;
        }
        void chart_BoundDataChanged(object sender, EventArgs e) {
            chart.SetObjectSelection(this.selectedObject);
            crosshairEnabled = chart.CrosshairEnabled;
        }
        void chart_MouseUp(object sender, MouseEventArgs e) {
            if (IsToolbarInteractionEnabled)
                chart.CrosshairEnabled = crosshairEnabled;
        }
        void chartCommandBarCheckItem_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            bool isChecked = IsToolbarInteractionEnabled;
            timer.Enabled = !isChecked;
            chart.CrosshairEnabled = IsToolbarInteractionEnabled ? DefaultBoolean.False : crosshairEnabled;
        }
        void BeforePopup(object sender, EventArgs e) {
            timer.Enabled = false;
        }
        void BeforePopup(object sender, CancelEventArgs e) {
            BeforePopup(sender, (EventArgs)e);
        }
        void CloseUp(object sender, EventArgs e) {
            if (!IsToolbarInteractionEnabled)
                timer.Enabled = true;
        }
        void CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
            CloseUp(sender, (EventArgs)e);
        }

        private bool IsStart = false;
        private void barBtnStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barBtnStart.Enabled = false;
            if(IsStart)
            {
                timer.Stop();
                barBtnStart.Caption = "Start";
                IsStart = false;
                barBtnStart.ImageOptions.Image = Properties.Resources.media_16x16;
                barBtnStart.Enabled = true;
            }
            else
            {
                _bkgr.RunWorkerAsync();
            }
        }

        private void chart_DoubleClick(object sender, EventArgs e)
        {
        }

        private void chart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            chart.RuntimeHitTesting = true;
            ChartHitInfo hit = chart.CalcHitInfo(e.Location);
            SeriesPoint point = hit.SeriesPoint;
            if (point != null)
            {
                // Obtain the series point argument.
                string argument = "Argument: " + point.Argument.ToString();

                // Obtain series point values.
                string values = "Value(s): " + point.Values[0].ToString();

                if (point.Values.Length > 1)
                {
                    for (int i = 1; i < point.Values.Length; i++)
                    {
                        values = values + ", " + point.Values[i].ToString();
                    }
                }
                MessageBox.Show(argument + "\n" + values, "SeriesPoint Data");
            }
            //var tmp = dataGenerator._lstCalculate;

            //if (hi.InAnnotation)
            //{
            //    Annotation an = hi.Annotation;
            //}
        }
    }
}
