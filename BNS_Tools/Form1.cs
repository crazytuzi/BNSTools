using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools
{
    public partial class Form1 : Form
    {
        DataChanged db;
        public Form1()
        {
            InitializeComponent();

        }
        private void ConnectionStatus(bool Status, string IP, string Acction, string Password)
        {
            if (Status)
            {
                this.Invoke(new Action(delegate
                {
                    button3.Text = "断开连接";
                    button1_Click(null,null);
                }));

                Tools.UpdateAppConfig("ServerIP", IP);
                Tools.UpdateAppConfig("Acction", Acction);
                Tools.UpdateAppConfig("Password", Password);
            }
            else
            {
                //连接失败
                this.Invoke(new Action(delegate
                {
                    button3.Text = "重新连接";
                }));
            }
        }
        

        private void Form1_Shown(object sender, EventArgs e)
        {

            textBox1.Text = GetAppConfig("ServerIP") == null ? textBox1.Text : GetAppConfig("ServerIP");
            textBox2.Text = GetAppConfig("Acction") == null ? textBox2.Text : GetAppConfig("Acction");
            textBox3.Text = GetAppConfig("Password") == null ? textBox3.Text : GetAppConfig("Password");

            db = new DataChanged(this);
            db.ConnStatus += ConnectionStatus;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = "连接中...";
            db.Connection(textBox1.Text, textBox2.Text, textBox3.Text);
        }

        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节增加一对键值对  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            bool exist = false;
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == newKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void 添加公告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2(db).ShowDialog();
        }

        private void 删除公告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int registerID = Convert.ToInt32(listView1.SelectedItems[0].Tag); ;
            Task.Run(() =>
            {
                string sqlstr = $@"delete [ManagementDB].dbo.[Announce] where [registerID] = '{registerID}'";
                int count = db._SQL_IDU(sqlstr);
                this.Invoke(new Action(delegate
                {
                    if (count > 0)
                    {
                        MessageBox.Show("删除成功!", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                string sqlstr = $@"select * from [ManagementDB].dbo.[Announce]";
                List<Announce_Entity> list = new List<Announce_Entity>();
                using (SqlDataReader sdr = db._SQLRead(sqlstr))
                {
                    while (sdr.Read())
                    {
                        Announce_Entity entity = new Announce_Entity()
                        {
                            registerID = Convert.ToInt32(sdr["registerID"]),
                            author = sdr["author"].ToString(),
                            dayInterval = Convert.ToInt32(sdr["dayInterval"]),
                            endDate = Convert.ToDateTime(sdr["endDate"]),
                            startDate = Convert.ToDateTime(sdr["startDate"]),
                            godSayMessage = sdr["godSayMessage"].ToString(),
                            godSayPosition = Convert.ToInt32(sdr["godSayPosition"]),
                            godSayType = Convert.ToInt32(sdr["godSayType"]),
                            minInterval = Convert.ToInt32(sdr["minInterval"]),
                            repeatCount = Convert.ToInt32(sdr["repeatCount"]),
                            title = sdr["title"].ToString(),
                            worlds = sdr["worlds"].ToString()
                        };
                        list.Add(entity);
                    }
                }
                List<ListViewItem> lvi = new List<ListViewItem>();
                foreach (var item in list)
                {
                    ListViewItem l = new ListViewItem();
                    l.Tag = item.registerID;
                    l.Text = (lvi.Count + 1).ToString();
                    l.SubItems.Add(item.title);
                    l.SubItems.Add(item.godSayMessage);
                    l.SubItems.Add(item.worlds);
                    l.SubItems.Add(item.author);
                    l.SubItems.Add(item.dayInterval.ToString());
                    l.SubItems.Add(item.minInterval.ToString());
                    l.SubItems.Add(item.repeatCount.ToString());
                    l.SubItems.Add(item.startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    l.SubItems.Add(item.endDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    lvi.Add(l);
                }
                this.Invoke(new Action(delegate
                {
                    listView1.Items.Clear();
                    listView1.Items.AddRange(lvi.ToArray());
                }));
            });
        }
    }
}
