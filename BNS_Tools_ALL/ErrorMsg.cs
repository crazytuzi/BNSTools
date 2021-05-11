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
    public partial class ErrorMsg : Form
    {
        Dictionary<string, string> msg;
        public ErrorMsg(Dictionary<string, string> dic)
        {
            InitializeComponent();
            msg = dic;
        }

        private void ErrorMsg_Shown(object sender, EventArgs e)
        {
            Task.Run(() => 
            {
                string str_1 = string.Empty;
                string str_2 = string.Empty;

                foreach (var item in msg)
                {
                    str_1 += $"{item.Key}\r\n";
                    str_2 += $"{item.Value}\r\n";
                }

                this.Invoke(new Action(delegate
                {
                    textBox1.Text = $"失败的:[UserID]\r\n{str_1}";
                    textBox2.Text = $"失败的:[UserName]\r\n{str_2}";
                }));

            });
            
        }
    }
}
