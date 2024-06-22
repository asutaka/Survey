using SurveyStock.DAL.MongoDAL;
using SurveyStock.Service;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SurveyStock
{
    public partial class Form1 : Form
    {
        private readonly IBllService _bllService;

        public Form1(IBllService bllService)
        {
            InitializeComponent();
            _bllService = bllService;
            _bllService.SyncCompany();
        }
       
    }
}
