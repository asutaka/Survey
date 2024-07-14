using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;

namespace SLib.Model.APIModel
{
    public class LoiNhuanAPIModel
    {
        public int status { get; set; }
        public LoiNhuanAPIData data { get; set; }
    }

    public class LoiNhuanAPIData
    {
        [JsonProperty(PropertyName = "x-axis")]
        public List<LoiNhuanAPIXAxis> xAxis { get; set; }
        public List<LoiNhuanAPIPoint> points { get; set; }
    }

    public class LoiNhuanAPIXAxis
    {
        public string name { get; set; }
        public long value { get; set; }
    }

    public class LoiNhuanAPIPoint
    {
        public long x { get; set; }
        public decimal y { get; set; }
        public decimal y1 { get; set; }
        public decimal y2 { get; set; }
        public decimal y3 { get; set; }
    }

    public class LoiNhuanAPIDetail
    {
        public string Name { get; set; }
        public decimal Profit { get; set; }
        public decimal Rate_qoq { get; set; }
    }
}
