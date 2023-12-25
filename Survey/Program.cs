﻿using Survey.GUI;
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
            Startup.Instance();
            //Application.Run(new frmTrace());
            Application.Run(new frmMain());
        }
    }
}
