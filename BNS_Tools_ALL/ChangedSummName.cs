using BNS_Tools_ALL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools_ALL
{
    public partial class ChangedSummName : Form
    {
        public string ReturnValue { get; set; }

        public ChangedSummName()
        {
            InitializeComponent();
        }
        public ChangedSummName(string _Name) 
        {
            InitializeComponent();
            textBox1.Text = _Name;
            this.Text = $"SummName:[{_Name}]";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue = textBox1.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
