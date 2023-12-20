using Quartz;
using Survey.GUI;
using Survey.Job;
using Survey.Job.ScheduleJob;
using Survey.Models;
using Survey.TestData;
using Survey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Survey
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //new Test14122023().HandleData();
            //Test14122023.Test1();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            new ScheduleMember(ScheduleMng.Instance().GetScheduler(), JobBuilder.Create<SubcribeJob>(), "0/10 * * * * ?", nameof(SubcribeJob)).Start();
            Application.Run(new frmMain());
        }
    }
}
