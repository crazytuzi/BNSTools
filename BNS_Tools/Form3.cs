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
    public partial class Form3 : Form
    {
        DataChanged db;
        List<DataMode> list = new List<DataMode>();
        List<dj_item> Data;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            list.Clear();

            sql += $"select a.[PostID],d.[dataID],d.[Amount],a.[SenderName],a.[RecipientName],a.[ReceiptTime],e.[LoginName] as 'SendAcction',f.[LoginName] as 'RecipientAcction' from [BlGame01].dbo.[UserPost] as a inner join [BlGame01].dbo.[CreatureProperty] as b on a.[SenderPCID] = b.[PCID] inner join [BlGame01].dbo.[CreatureProperty] as c on a.[RecipientPCID] = c.[PCID] left join [BlGame01].dbo.[UserPostAttachment] as d on d.[PostID] = a.[PostID] inner join [PlatformAcctDb].dbo.[Users] as e on e.[UserID] = b.[game_account_id] inner join [PlatformAcctDb].dbo.[Users] as f on f.[UserID] = c.[game_account_id] where 1 = 1";
            string sql_str = string.Empty;
            if (textBox1.Text.Trim() != "")
            {
                sql += $"and e.[LoginName] = '{textBox1.Text.Trim()}'";
                sql_str += $"and b.[LoginName] = '{textBox1.Text.Trim()}@ncsoft.com'";
            }
            if (textBox2.Text.Trim() != "")
            {
                sql += $"and a.[SenderName] like '%{textBox2.Text.Trim()}'%";
            }
            if (textBox3.Text.Trim() != "")
            {
                sql += $"and d.[dataId] in ({textBox3.Text.Trim()})";
                sql_str += $"and a.[道具ID] in ({textBox3.Text.Trim()})";
            }
            sql += " order by a.[ReceiptTime] desc ";

            Task.Run(() => 
            {
                
                List<PostEntity> da = new List<PostEntity>();
                using (SqlDataReader sdr = db._SQLRead(sql))
                {
                    while (sdr.Read())
                    {
                        da.Add(new PostEntity()
                        {
                            PostID = sdr["PostID"].ToString(),
                            Amount = sdr["Amount"].ToString(),
                            dataID = sdr["dataID"] == DBNull.Value ? "无" : sdr["dataID"].ToString(),
                            ReceiptTime = sdr["ReceiptTime"].ToString(),
                            RecipientName = sdr["RecipientName"].ToString(),
                            RecipientAcction = sdr["RecipientAcction"].ToString().Replace("@ncsoft.com", ""),
                            SenderName = sdr["SenderName"].ToString(),
                            SendAcction = sdr["SendAcction"].ToString().Replace("@ncsoft.com","")
                        });
                    }
                }

                string PostID = "";
                DataMode T = null;
                foreach (var item in da)
                {
                    if (PostID != item.PostID)
                    {
                        if (T != null)
                        {
                            list.Add(T);
                        }
                        PostID = item.PostID;
                        T = new DataMode();
                        T.PostID = item.PostID;
                        T.SendName = item.SenderName;
                        T.SendAcction = item.SendAcction;
                        T.RecipientName = item.RecipientName;
                        T.RecipientAcction = item.RecipientAcction;
                        T.Timer = item.ReceiptTime;
                        T.jb = "0";
                    }
                    if (item.dataID != "无")
                    {
                        List<dj_item> d = Data.Where(it => it.ID == Convert.ToInt32(item.dataID)).ToList();
                        if (d.Count > 0)
                        {
                            T.Data += $"[{d[0].Name}]x{item.Amount};";
                        }
                        else
                        {
                            T.Data += $"{item.dataID}x{item.Amount};";
                        }

                    }
                    else
                    {
                        if (item.Amount != "")
                        {
                            T.jb = $"{Convert.ToDecimal(item.Amount) / 10000}";
                        }
                    }
                }

                List<ListViewItem> lvi_l = new List<ListViewItem>();
                foreach (var item in list)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = (lvi_l.Count + 1).ToString();
                    lvi.SubItems.Add(item.Data);
                    lvi.SubItems.Add(item.jb);
                    lvi.SubItems.Add(item.SendAcction);
                    lvi.SubItems.Add(item.RecipientAcction);
                    lvi.SubItems.Add(item.SendName);
                    lvi.SubItems.Add(item.RecipientName);
                    lvi.SubItems.Add(item.Timer);
                    lvi_l.Add(lvi);
                }

                this.Invoke(new Action(delegate
                {
                    listView1.Items.Clear();
                    listView1.Items.AddRange(lvi_l.ToArray());
                }));

            });

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = "连接中...";
            db.Connection(textBox8.Text, textBox7.Text, textBox6.Text);
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            textBox8.Text = GetAppConfig("ServerIP") == null ? textBox1.Text : GetAppConfig("ServerIP");
            textBox7.Text = GetAppConfig("Acction") == null ? textBox2.Text : GetAppConfig("Acction");
            textBox6.Text = GetAppConfig("Password") == null ? textBox3.Text : GetAppConfig("Password");

            db = new DataChanged(this);
            db.ConnStatus += ConnectionStatus;


            Data = new List<dj_item>();

            if (File.Exists(Application.StartupPath + "\\item.txt"))
            {
                Task.Run(() =>
                {
                    Read(Application.StartupPath + "\\item.txt");
                });

            }
            else
            {
                MessageBox.Show("未找到item.txt物品信息文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void ConnectionStatus(bool Status, string IP, string Acction, string Password)
        {
            if (Status)
            {
                this.Invoke(new Action(delegate
                {
                    button3.Text = "断开连接";
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

        public void Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Data.Add(new dj_item()
                {
                    ID = Convert.ToInt32(line.Substring(0, line.IndexOf("="))),
                    Name = line.Substring(line.IndexOf("=") + 1)
                });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() => 
            {
                string sql = $"select b.[LoginName] as '账号',[道具ID],[数量],[时间] from (select [OwnerAccountID] as 'UserID',[ItemDataID] as '道具ID',[ItemAmount] as '数量',[RegistrationTime] as '时间' from [GamewarehouseDB].dbo.[WarehouseItemArchive] union select [OwnerAccountID] as 'UserID',[ItemDataID] as '道具ID',[ItemAmount] as '数量',[RegistrationTime] as '时间' from  [GamewarehouseDB].dbo.[WarehouseItem]) as a left join [PlatformAcctDb].dbo.[Users] as b on a.[UserID] = b.[UserId] where 1 = 1";
                string str = string.Empty;
                int i = 0;
                using (SqlDataReader sdr = db._SQLRead(sql))
                {
                    while (sdr.Read())
                    {
                        List<dj_item> d = Data.Where(it => it.ID == Convert.ToInt32(sdr["道具ID"])).ToList();
                        if (d.Count > 0 && !str.Contains($"{d[0].ID}={d[0].Name}\r\n"))
                        {
                            str += $"{d[0].ID}={d[0].Name}\r\n";
                        }
                        i++;
                    }
                }
                Console.WriteLine(str);
            });
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string sql = $@"select b.[LoginName] as '账号',[道具ID],[数量],[时间] from (select [OwnerAccountID] as 'UserID',[ItemDataID] as '道具ID',[ItemAmount] as '数量',[RegistrationTime] as '时间' from [GamewarehouseDB].dbo.[WarehouseItemArchive] union select [OwnerAccountID] as 'UserID',[ItemDataID] as '道具ID',[ItemAmount] as '数量',[RegistrationTime] as '时间' from  [GamewarehouseDB].dbo.[WarehouseItem]) as a left join [PlatformAcctDb].dbo.[Users] as b on a.[UserID] = b.[UserId] where 1 = 1 ";
            if (textBox1.Text.Trim() != "")
            {
                sql += $"and b.[LoginName] = '{textBox1.Text.Trim()}@ncsoft.com'";
            }
            if (textBox3.Text.Trim() != "")
            {
                sql += $"and a.[道具ID] in ({textBox3.Text.Trim()})";
            }
            sql += "order by a.[时间] desc ";
            Task.Run(() => 
            {
                List<ListViewItem> lvi_l_1 = new List<ListViewItem>();
                using (SqlDataReader sdr = db._SQLRead(sql))
                {
                    while (sdr.Read())
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = (lvi_l_1.Count + 1).ToString();
                        lvi.SubItems.Add(sdr["道具ID"].ToString());
                        List<dj_item> d = Data.Where(it => it.ID == Convert.ToInt32(sdr["道具ID"])).ToList();
                        if (d.Count > 0)
                        {
                            lvi.SubItems.Add(d[0].Name);
                        }
                        else
                        {
                            lvi.SubItems.Add("");
                        }

                        lvi.SubItems.Add(sdr["数量"].ToString());
                        lvi.SubItems.Add(sdr["账号"].ToString().Replace("@ncsoft.com", ""));
                        lvi.SubItems.Add(sdr["时间"].ToString());
                        lvi_l_1.Add(lvi);
                    }
                }

                this.Invoke(new Action(delegate
                {
                    listView2.Items.Clear();
                    listView2.Items.AddRange(lvi_l_1.ToArray());
                }));
            });
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView com = sender as ListView;

            // Determine what the last sort order was and change it.
            if (com.Sorting == System.Windows.Forms.SortOrder.Ascending)
            {
                com.Sorting = System.Windows.Forms.SortOrder.Descending;
            }
            else
            {
                com.Sorting = System.Windows.Forms.SortOrder.Ascending;
            }
            com.Sort();
            com.ListViewItemSorter = new ListViewItemComparer(e.Column, com.Sorting);
        }
    }
}
