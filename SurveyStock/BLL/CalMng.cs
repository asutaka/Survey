using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.BLL
{
    public static partial class CalMng
    {
        private static void DataInit()
        {
            if (!PrepareData._dic1d.Any())
            {
                PrepareData.LoadData1d();
                PrepareIndicator.Ma20_1d();
            }
        }
    }
}
