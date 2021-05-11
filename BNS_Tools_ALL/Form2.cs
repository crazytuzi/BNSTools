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
    public partial class Form2 : Form
    {
        DataChanged db;
        Form1 mains;
        public Form2(Form1 _mains,DataChanged _db)
        {
            InitializeComponent();
            db = _db;
            mains = _mains;
        }

        private void SQL_BTN_Click(object sender, EventArgs e)
        {
            if (SQL_BTN.Text == "连接并保存")
            {
                mains.SQL_STATUS.Text = "连接中…";
                mains.SQL_STATUS.ForeColor = Color.YellowGreen;
                db.Connection(IP_TB.Text.Trim(), ACCTION_TB.Text.Trim(), PASSWORD_TB.Text);
                this.Close();
            }
            else if (SQL_BTN.Text == "断开连接")
            {
                mains.SQL_STATUS.Text = "等候连接";
                mains.SQL_STATUS.ForeColor = Color.Red;
                SQL_BTN.Text = "连接并保存";
                db.Close();
            }
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            IP_TB.Text = Tools.GetAppConfig("ServerIP") == null ? IP_TB.Text : Tools.GetAppConfig("ServerIP");
            ACCTION_TB.Text = Tools.GetAppConfig("Acction") == null ? ACCTION_TB.Text : Tools.GetAppConfig("Acction");
            PASSWORD_TB.Text = Tools.GetAppConfig("Password") == null ? PASSWORD_TB.Text : Tools.GetAppConfig("Password");

            if (db.IsConnection)
            {
                SQL_BTN.Text = "断开连接";
            }
        }
    }
}
