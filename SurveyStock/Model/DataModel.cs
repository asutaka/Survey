﻿using Skender.Stock.Indicators;

namespace SurveyStock.Model
{
    public class DataModel
    {
        public decimal t { get; set; }
        public decimal o { get; set; }
        public decimal h { get; set; }
        public decimal l { get; set; }
        public decimal c { get; set; }
        public decimal v { get; set; }
    }

    public class QuoteEx : Quote
    {
        public double Ma20 { get; set; }
    }
}