using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class HighChartAPIModel
    {
        public HighChartAPIModel(string input) 
        {
            infile = input;
            width = false;
            scale = false;
            constr = "chart";
            type = "image/png";
            b64 = false;
        }
        public string infile { get; set; }
        public bool width { get; set; }
        public bool scale { get; set; }
        public string constr { get; set; }
        public string type { get; set; }
        public bool b64 { get; set; }
    }
    public class HighchartBasicColumn
    {
        public HighchartBasicColumn(string titl, List<string> lCat, List<HighChartSeries_BasicColumn> lSeries) 
        {
            title = new HighChartTitle { text = titl };
            xAxis = new HighChartXAxis { categories = lCat };
            yAxis = new HighChartYAxis { title = new HighChartTitle { text = string.Empty } };
            series = lSeries;
            credits = new HighChartCredits { enabled = false };
        }
        public HighChartTitle title { get; set; }
        public HighChartXAxis xAxis { get; set; }
        public HighChartYAxis yAxis { get; set; }
        public List<HighChartSeries_BasicColumn> series { get; set; }
        public HighChartCredits credits { get; set; }
    }

    public class HighChartBasicColumnCustomColor
    {
        public HighChartBasicColumnCustomColor(string titl, string titlSeries, List<List<object>> lSeries, bool isMixColor = true)
        {
            title = new HighChartTitle { text = titl };
            xAxis = new HighChartXAxis { type = "category" };
            yAxis = new HighChartYAxis { title = new HighChartTitle { text = string.Empty } };
            series = new List<HighChartSeries_BasicColumnCustomColor>
            {
                new HighChartSeries_BasicColumnCustomColor
                {
                    data = lSeries,
                    type = "column",
                    name = titlSeries,
                    colorByPoint = isMixColor,
                    colors = new List<string>
                    {
                        "#9b20d9", "#9215ac", "#861ec9", "#7a17e6", "#7010f9", "#691af3",
                        "#6225ed", "#5b30e7", "#533be1", "#4c46db", "#4551d5", "#3e5ccf",
                        "#3667c9", "#2f72c3", "#277dbd", "#1f88b7", "#1693b1", "#0a9eaa",
                        "#03c69b",  "#00f194"
                    }
                }
            };
            credits = new HighChartCredits { enabled = false };
        }
        public HighChartTitle title { get; set; }
        public HighChartXAxis xAxis { get; set; }
        public HighChartYAxis yAxis { get; set; }
        public List<HighChartSeries_BasicColumnCustomColor> series { get; set; }
        public HighChartCredits credits { get; set; }
    }

    #region ChienLuocDauTu
    public class HighChartChienLuocDauTu
    {
        public HighChartChienLuocDauTu(string titl, List<HighChartSeries_ChienLuocDauTu> lSeries)
        {
            title = new HighChartTitle { text = titl };
            series = lSeries;
            credits = new HighChartCredits { enabled = false };
        }
        public HighChartTitle title { get; set; }
        public List<HighChartSeries_ChienLuocDauTu> series { get; set; }
        public HighChartCredits credits { get; set; }
    }

    public class HighChartSeries_ChienLuocDauTu
    {
        public string type { get; set; }
        public List<HighChartSeriesData_ChienLuocDauTu> data { get; set; }
        public HighChartSeriesMarker_ChienLuocDauTu marker { get; set; }
        public int borderRadius { get; set; }
        public List<HighChartSeriesLevel_ChienLuocDauTu> levels { get; set; }
    }

    public class HighChartSeriesData_ChienLuocDauTu
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string name { get; set; }
    }

    public class HighChartSeriesMarker_ChienLuocDauTu
    {
        public string symbol { get; set; }
        public string width { get; set; }
    }

    public class HighChartSeriesLevel_ChienLuocDauTu
    {
        public int level { get; set; }
        public bool levelIsConstant { get; set; }
        public bool colorByPoint { get; set; }
        public HighChartSeriesColor_ChienLuocDauTu colorVariation { get; set; }
    }

    public class HighChartSeriesColor_ChienLuocDauTu
    {
        public string key { get; set; }
        public double to { get; set; }
    } 
    #endregion

    public class HighChartTitle
    {
        public string text { get; set; }
    }

    public class HighChartXAxis
    {
        public string type { get; set; }
        public List<string> categories { get; set; }
    }

    public class HighChartYAxis
    {
        public HighChartTitle title { get; set; }
    }

    public class HighChartSeries
    {
        public string type { get; set; }
        public string name { get; set; }
        public List<string> colors { get; set; }
        public string color { get; set; }
        public bool colorByPoint { get; set; }
    }

    public class HighChartSeries_BasicColumn : HighChartSeries
    {
        public List<double> data { get; set; }
    }

    public class HighChartSeries_BasicColumnCustomColor : HighChartSeries
    {
        public List<List<object>> data { get; set; }
    }

    public class HighChartCredits
    {
        public bool enabled { get; set; }
    }
}
