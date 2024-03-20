using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using Survey.AdapterData;
using Survey.Models;
using Survey.Utils;

namespace Survey.GUI
{

    public partial class FinancialChartingCustom : Form {
        const int InitialPointCountOnScreen = 90;
        const int MaxZoomPointCount = 300;
        private string _symbol = string.Empty;

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
        SecondaryAxisY VolumeAxisY {
            get { return XYDiagram.SecondaryAxesY["VolumeAxisY"]; }
        }
        Series PriceSeries {
            get { return this.chart.Series["Price"]; }
        }
        Series VolumeSeries {
            get { return this.chart.Series["Volume"]; }
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

        public FinancialChartingCustom() {
            InitializeComponent();
        }
        void checkConfig()
        {
            var symbol = bartxtSymbol.EditValue.ToString();
            if (string.IsNullOrWhiteSpace(symbol))
                return;
            _symbol = symbol.Trim();
            if (!symbol.Contains("USDT"))
                symbol = $"{symbol.Trim().ToUpper()}USDT";
            //AutoMergeRibbon = true;
            chart.BeginInit();
            this.dataGenerator = new RealTimeFinancialDataGenerator();
            //this.dataGenerator.InitialDataFastAll("GMTUSDT",3000);
            this.dataGenerator.InitialDataFast(symbol, EInterval.I4H);
            InitChartControl();
            SetVisualRangesAndGridOptions();
            chart.EndInit();
            this.dataGenerator.Start();
            //this.timer.Enabled = true;
        }
        void InitChartControl() {
            chart.DataSource = this.dataGenerator.DataSource;
            PriceSeries.SetFinancialDataMembers("DateTimeStamp", "Low", "High", "Open", "Close");
            VolumeSeries.SetDataMembers("DateTimeStamp", "Volume");
            SetCustomLabelColor();
            selectAxisMeasureUnitBarItem1.EditValue = selectAxisMeasureUnitRepositoryItemComboBox1.Items[1];
        }
        void SetVisualRangesAndGridOptions() {
            TimeSpan visibleInterval;
            int multiplier = AxisX.DateTimeScaleOptions.MeasureUnitMultiplier;
            double timeIntervalToShow = InitialPointCountOnScreen * multiplier;
            switch (AxisX.DateTimeScaleOptions.MeasureUnit) {
                case DateTimeMeasureUnit.Minute:
                    visibleInterval = TimeSpan.FromMinutes(timeIntervalToShow);
                    VolumeAxisY.WholeRange.SideMarginsValue = 1000000 * multiplier;
                    break;
                case DateTimeMeasureUnit.Hour:
                    visibleInterval = TimeSpan.FromHours(timeIntervalToShow);
                    VolumeAxisY.WholeRange.SideMarginsValue = 60000000 * multiplier;
                    break;
                case DateTimeMeasureUnit.Day:
                    visibleInterval = TimeSpan.FromDays(timeIntervalToShow);
                    VolumeAxisY.WholeRange.SideMarginsValue = 1440000000 * multiplier;
                    break;
                case DateTimeMeasureUnit.Week:
                    visibleInterval = TimeSpan.FromDays(timeIntervalToShow * 7);
                    VolumeAxisY.WholeRange.SideMarginsValue = 7 * 1440000000d * multiplier;
                    break;
                default:
                    throw new NotSupportedException("This measure unit is not supported.");
            }
            AxisX.VisualRange.SetMinMaxValues(this.dataGenerator.LastArgument - visibleInterval, this.dataGenerator.LastArgument);
        }
        void SetCustomLabelColor() {
            AxisY.CustomLabels[0].BackColor = chart.GetPaletteEntries(2)[1].Color;
        }

        void timer_Tick(object sender, EventArgs e) {
            if (dataGenerator != null)
            {
                dataGenerator.UpdateSource();
                //Check2Buy();
                //Check2Sell();
            }
            CustomAxisLabel currentValueLabel = AxisY.CustomLabels[0];
            if (PriceSeries.Points.Count > 0)
            {
                FinancialDataCollection dataSource = this.dataGenerator.DataSource;
                double currentClose = dataSource[dataSource.Count - 1].Close;
                currentValueLabel.AxisValue = currentClose;
                currentValueLabel.Name = string.Format("{0:0.0000}", currentClose);
            }
        }

        #region Calculate
        bool hasBuy = false;
        FinancialDataPoint buyEntity = new FinancialDataPoint();
        int indexBuy = 0;
        double prevMCDX = 0;
        double valBottomWave = 0;
        //private void Check2Buy()
        //{
        //    if (hasBuy)
        //        return;

        //    var checkMCDX = prevMCDX;
        //    var last = dataGenerator._lstCalculate.Last();
        //    prevMCDX = last.Volume;
        //    if (last.Volume <= 0 || last.Volume < checkMCDX || last.Close < last.Open)
        //        return;

        //    var count = dataGenerator._lstCalculate.Count();
        //    var MA20Price = CalculateMng.MA(dataGenerator._lstCalculate.Select(x => x.Close).ToArray(), Core.MAType.Sma, 20, count);
        //    if (last.High < MA20Price || last.Low > MA20Price)
        //        return;

        //    var bottomWaveIndex = GetBottomWave();
        //    var entityBotWave = dataGenerator._lstCalculate.ElementAt(bottomWaveIndex);
        //    var dateBot = dataGenerator._lstCalculate.ElementAt(bottomWaveIndex).DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss");
        //    var content = $"Stock Level: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}";
        //    //LogM.Log(content);
        //    //Check nến hiện tại có phải là đáy hay không
        //    if (entityBotWave.DateTimeStamp == last.DateTimeStamp)
        //    {
        //        //Nếu là nến bao trùm tăng thì vẫn chấp nhận
        //        var entityPrev = dataGenerator._lstCalculate.ElementAt(count - 2);
        //        if(last.High <= entityPrev.High)
        //        {
        //            var content1 = $"ERROR1: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}| Loại vì nến hiện tại là đáy";
        //            //LogM.Log(content1);
        //            return;
        //        }    
        //    }
        //    //Check chỉ duy nhất nến hiện tại cắt qua MA20
        //    for (int i = bottomWaveIndex; i < count - 2; i++)
        //    {
        //        var element = dataGenerator._lstCalculate.ElementAt(i);
        //        var MA20PriceElement = CalculateMng.MA(dataGenerator._lstCalculate.Select(x => x.Close).ToArray(), Core.MAType.Sma, 20, i);
        //        if (element.High >= MA20PriceElement && element.Low < MA20PriceElement)
        //        {
        //            var content1 = $"ERROR2: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}| Loại vì từ đáy đến nến hiện tại có nến vượt MA20";
        //            //LogM.Log(content1);
        //            //LogM.Log(element.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss"));
        //            return;
        //        }
        //    }
        //    //Check từ đáy đến nến hiện tại >= 4
        //    if (count - bottomWaveIndex >= 5)
        //    {
        //        var content1 = $"ERROR3: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}| Loại vì từ đáy đến nến hiện tại quá xa({count - bottomWaveIndex})";
        //        //LogM.Log(content1);
        //        return;
        //    }
        //    //Check từ đáy đến MA20 hiện tại phải < 3%
        //    var rateBotWave = (MA20Price - entityBotWave.Low) * 100 / entityBotWave.Low;
        //    if (rateBotWave > 3)
        //    {
        //        var content1 = $"ERROR4: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}| Loại vì từ đáy đến MA20 hiện tại lớn hơn 3%({rateBotWave})";
        //        //LogM.Log(content1);
        //        return;
        //    }    

        //    var contentSuccess = $"Success: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}; BottomWave: {dateBot}";
        //    LogM.Log(contentSuccess);
        //    valBottomWave = entityBotWave.Low;
        //    buyEntity = last;
        //    indexBuy = count - 1;
        //    hasBuy = true;
        //}
        private int GetBottomWave()
        {
            var indexBottomWave = -1;
            double valBottomWave = 0;
            var flag = 0;
            var count = dataGenerator._lstCalculate.Count();
            for (int i = count - 1; i >= count - 15; i--)
            {
                flag++;
                var element = dataGenerator._lstCalculate.ElementAt(i);
                if(valBottomWave == 0 || valBottomWave > element.Low)
                {
                    valBottomWave = element.Low;
                    indexBottomWave = i;
                    flag = 0;
                }
                //check
                if(flag >= 3)
                {
                    return indexBottomWave;
                }    
            }
            return indexBottomWave;
        }

        double valTakeProfit = 0;
        double valSell = 0;
        int countSell = 0;
        //private void Check2Sell()
        //{
        //    if (!hasBuy)
        //        return;

        //    var last = dataGenerator._lstCalculate.Last();
        //    var count = dataGenerator._lstCalculate.Count();
        //    var no = count - (1 + indexBuy);
        //    //Init
        //    if(valSell == 0)
        //    {
        //        valSell = buyEntity.Close;
        //    }

        //    if(last.Close > valSell)
        //    {
        //        valSell = last.Close;
        //        countSell = 0;
        //    }
        //    else
        //    {
        //        countSell++;
        //    }
        //    if(countSell >= 4)
        //    {
        //        var contentSell = $"[Sell]|Time: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}|TP: {(last.Close - buyEntity.Close) * 100 / buyEntity.Close}%|No: {no}";
        //        LogM.Log(contentSell);
        //        hasBuy = false;
        //        valTakeProfit = 0;
        //        valSell = 0;
        //        countSell = 0;
        //        return;
        //    }

        //    if (valTakeProfit > 0)
        //    {
        //        if(last.Close > valTakeProfit)
        //        {
        //            valTakeProfit = last.Close;
        //            return;
        //        }

        //        var rate = (valTakeProfit - last.Low) * 100 / last.Low;
        //        if(Math.Abs(rate) >= 2)
        //        {
        //            var contentSell = $"[TP] Sell|Time: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}|TP: {(valTakeProfit * 0.98 - buyEntity.Close) * 100 / buyEntity.Close}%|No: {no}";
        //            LogM.Log(contentSell);
        //            hasBuy = false;
        //            valTakeProfit = 0;
        //            valSell = 0;
        //            countSell = 0;
        //            return;
        //        }
        //    }


        //    if (last.Low <= valBottomWave && no > 0)
        //    {
        //        var contentSell = $"[Sell] Cutloss1|Time: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}|TP: {(valBottomWave - buyEntity.Close) *100/buyEntity.Close}%|No: {no}";
        //        LogM.Log(contentSell);
        //        hasBuy = false;
        //        valTakeProfit = 0;
        //        valSell = 0;
        //        countSell = 0;
        //        return;
        //    }    

        //    if(no >= 3 && last.Close >= buyEntity.Close)
        //    {
        //        var rate = (last.Close - buyEntity.Close) * 100 / buyEntity.Close;
        //        var MA20Price = CalculateMng.MA(dataGenerator._lstCalculate.Select(x => x.Close).ToArray(), Core.MAType.Sma, 20, count);
        //        if (last.Low < MA20Price && rate < 2)
        //        {
        //            var contentSell = $"[Sell] Cutloss2|Time: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}|TP: {rate}%|No: {no}";
        //            LogM.Log(contentSell);
        //            hasBuy = false;
        //            valTakeProfit = 0;
        //            valSell = 0;
        //            countSell = 0;
        //            return;
        //        }
        //    }    

        //    if(no >= 5)
        //    {
        //        var rate = (last.Close - buyEntity.Close) * 100 / buyEntity.Close;
        //        if(rate < 2)
        //        {
        //            var contentSell = $"[Sell] Cutloss3|Time: {last.DateTimeStamp.ToString("dd/MM/yyyy HH:mm:ss")}|TP: {rate}%|No: {no}";
        //            LogM.Log(contentSell);
        //            hasBuy = false;
        //            valTakeProfit = 0;
        //            valSell = 0;
        //            countSell = 0;
        //            return;
        //        }    
        //    }

        //    var rateMax = (last.Close - buyEntity.Close) * 100 / buyEntity.Close;
        //    if(rateMax >= 9 && valTakeProfit == 0)
        //    {
        //        valTakeProfit = last.Close;
        //        return;
        //    }
        //}
        #endregion


        void selectChartMeasureUnitRepositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e) {  
            SetVisualRangesAndGridOptions();
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
        void BeforePopup(object sender, System.ComponentModel.CancelEventArgs e) {
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
            if(IsStart)
            {
                this.timer.Stop();
                barBtnStart.Caption = "Start";
                IsStart = false;
                //this.barBtnStart.ImageOptions.Image = Properties.Resources.media_16x16;
                //this.barBtnStart.ImageOptions.LargeImage = Properties.Resources.media_32x32;
            }
            else
            {
                if (!bartxtSymbol.EditValue.ToString().Trim().Equals(_symbol))
                {
                    checkConfig();
                }
                this.timer.Interval = int.Parse(barEditInterval.EditValue.ToString());
                this.timer.Start();
                barBtnStart.Caption = "Stop";
                IsStart = true;
                //this.barBtnStart.ImageOptions.Image = Properties.Resources.pause_16x16;
                //this.barBtnStart.ImageOptions.LargeImage = Properties.Resources.pause_32x32;
            }
        }
    }
}
