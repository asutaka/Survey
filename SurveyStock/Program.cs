using SurveyStock.DAL;
using SurveyStock.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SurveyStock
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            sqliteDayDB.Instance();

            Test20240421.MainFunc();

            //Application.Run(new Form1());
        }
    }
}
