using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Survey.GUI
{
    /// <summary>
    /// Form này chỉ show chart và chỉ báo, giá mua 
    /// </summary>
    public partial class frmChart : XtraForm
    {
        private readonly string _symbol;
        public frmChart(string symbol)
        {
            this._symbol = symbol;
            InitializeComponent();
        }
    }
}