using BNS_Tools_ALL.Entity;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace BNS_Tools_ALL
{
    public partial class Form1 : Form
    {
        bool LockProjcet = false;
        List<dj_item> Data;
        List<UserEntity> u_data;
        bool IsLoadKey = false;
        List<ListViewItem> mList;

        DataChanged db;

        List<data_code> data_ids;
        List<Promotions_Entity> Promotions;
        List<RewardPolicies_Entity> RewardPolicies;

        bool _APIMode = false;

        public bool APIMode
        {
            get
            {
                return _APIMode;
            }
            set
            {
                APIChanged(value);
                _APIMode = value;
            }
        }

        string u_ID;

        bool Lock_Check;
        int Check_PCID = 0;
        string CheckAcction = "";
        string JobStr = "";
        public string msg
        {
            set
            {
                this.Invoke(new Action(delegate
                {
                    label8.Text = $"{value}";
                }));
                Thread.Sleep(4000);
                this.Invoke(new Action(delegate
                {
                    label8.Text = "等候操作";
                }));
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// API模式切换
        /// </summary>
        /// <param name="status"></param>
        public void APIChanged (bool status)
        {
            if (status)
            {
                this.Invoke(new Action(delegate
                {
                    groupBox6.Text = "点券[实时API]";
                    groupBox2.Text = "礼品箱物品[实时API]";
                    button42.Enabled = true;
                    this.Text = "BNS工具集 - [API+DB-MODE]";
                }));
            }
            else
            {
                this.Invoke(new Action(delegate
                {
                    groupBox6.Text = "点券[DB]";
                    groupBox2.Text = "礼品箱物品[DB]";
                    button42.Enabled = false;
                    button42.Text = "需要API支持";
                    this.Text = "BNS工具集 - [DB-MODE]";
                }));
                
            }
            
            
        }

        private void Sql_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void ConnectionStatus(bool Status, string IP, string Acction, string Password)
        {
            if (Status)
            {

                this.Invoke(new Action(delegate
                {
                    SQL_STATUS.Text = "连接成功";
                    SQL_STATUS.ForeColor = Color.SeaGreen;
                    button27_Click(null, null);
                }));

                Tools.UpdateAppConfig("ServerIP", IP);
                Tools.UpdateAppConfig("Acction", Acction);
                Tools.UpdateAppConfig("Password", Password);

                if (Tools.GetAppConfig("UserAcction") != null)
                {
                    Thread.Sleep(2000);
                    this.Invoke(new Action(delegate
                    {
                        UNAME_TB.Text = Tools.GetAppConfig("UserAcction");
                        button10_Click(null, null);
                    }));
                    
                }

                try
                {
                    string PingAPI = Tools.Get($"http://{db.ServerIP}:8012/Ping.aspx", new Dictionary<string, string>());

                    if (PingAPI.Contains("Success"))
                        APIMode = true;
                    else
                        APIMode = false;
                }
                catch
                {
                    APIMode = false;
                }

                

            }
            else
            {
                //连接失败
                this.Invoke(new Action(delegate
                {
                    SQL_STATUS.Text = "连接失败";
                    SQL_STATUS.ForeColor = Color.Red;
                }));
            }
        }
        public string getip()
        {
            try
            {
                string ipc = "";
                string AddressIP = string.Empty;
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        AddressIP = _IPAddress.ToString();
                    }
                }
                ipc = AddressIP;
                Console.WriteLine(ipc);
                return ipc;
            }
            catch
            {
                return "IP认证出现错误";
            }
        }
        private void IDU_Status(int Count)
        {

        }
        private void ReadData(SqlDataReader sdr)
        {

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

        private void Form1_Shown(object sender, EventArgs e)
        {
            Tools.Mac = getMacAddr_Local();
            u_ID = "";
            button10.Visible = true;
            button20.Visible = true;
                button28.Visible = true;
                button29.Visible = true;
                button17.Visible = true;
            
            db = new DataChanged(this);
            db.ConnStatus += ConnectionStatus;

            if (Tools.GetAppConfig("ServerIP") == null || Tools.GetAppConfig("Acction") == null || Tools.GetAppConfig("Password") == null)
            {
                new Form2(this, db).ShowDialog();
            }
            else
            {
                db.Connection(Tools.GetAppConfig("ServerIP"), Tools.GetAppConfig("Acction"), Tools.GetAppConfig("Password"));
            }



            listView1.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(MyList_RetrieveVirtualItem);

            Data = new List<dj_item>();

            if (File.Exists(Application.StartupPath + "\\item.txt"))
            {
                Task.Run(() =>
                {
                    Read(Application.StartupPath + "\\item.txt");

                    LoadData("", "");
                });

            }
            else
            {
                MessageBox.Show("未找到item.txt物品信息文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            DataTable dt = new DataTable();
            dt.TableName = "dt";
            dt.Columns.Add("Code");
            dt.Columns.Add("Name");

            DataRow dr1 = dt.NewRow();
            dr1["Code"] = "1420";
            dr1["Name"] = "前往白青山脉";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Code"] = "1510";
            dr1["Name"] = "第七章第二幕-帮手";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Code"] = "1668";
            dr1["Name"] = "主线完成";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Code"] = "1667";
            dr1["Name"] = "超越轻功的极限";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Code"] = "1617";
            dr1["Name"] = "昆仑绝壁";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Code"] = "966";
            dr1["Name"] = "天下双势的召唤";
            dt.Rows.Add(dr1);

            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "Code";
            this.comboBox1.DataSource = dt;

            data_ids = new List<data_code>();
            data_ids.Add(new data_code()
            {
                data_id = 1519,
                current_mission_step = 15
            });//开启苍穹盆地\法器研究所\南天圣地书信
            data_ids.Add(new data_code()
            {
                data_id = 1520,
                current_mission_step = 16
            });//开启暴徒之岛
            data_ids.Add(new data_code()
            {
                data_id = 1611,
                current_mission_step = 16
            });//启永劫圣所\霸主密室\庶子安息处书信和寺院前置任务
            data_ids.Add(new data_code()
            {
                data_id = 1638,
                current_mission_step = 16
            });//开启书信超越轻功的极限
            data_ids.Add(new data_code()
            {
                data_id = 1641,
                current_mission_step = 14
            });//开启武神塔的支线任务
            data_ids.Add(new data_code()
            {
                data_id = 1657,
                current_mission_step = 16
            });//开启狼人  树人副本任务


            Task.Run(() =>
            {
                Thread.Sleep(1500);
                string msg = Tools.Get("http://101.37.76.151:8121/api/GetMsg", new Dictionary<string, string>());
                msg = msg.Substring(1, msg.Length - 2);
                if (msg != "Not_Msg")
                {
                    string title = msg.Substring(0, msg.IndexOf("||"));
                    string txt = msg.Substring(title.Length + 2);

                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show(txt, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));

                }
            });

            Task.Run(()=> 
            {
                Thread.Sleep(5000);
                this.Invoke(new Action(delegate 
                {
                    tabControl1.Enabled = true;
                    label42.Visible = false;
                    this.Controls.Remove(label42);
                }));
                
            });
            
        }
        ///<summary>
        /// 虚拟模式事件
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (this.mList == null || this.mList.Count == 0)
                {
                    return;
                }
                e.Item = this.mList[e.ItemIndex];
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"数据加载异常 错误内容:{ex.Message}");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                ID_TB.Text = mList[listView1.SelectedIndices[0]].Tag.ToString();
                if (COUNT_TB.Text.Trim().Equals(""))
                {
                    COUNT_TB.Text = "1";
                }
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

        private void LoadData(string ID, string str)
        {
            this.Invoke(new Action(delegate
            {
                mList = new List<ListViewItem>();
                listView1.VirtualListSize = 0;

            }));

            List<dj_item> _data = new List<dj_item>();
            List<ListViewItem> lvi_list = new List<ListViewItem>();
            foreach (var item in Data)
            {

                if (str.Trim() == "" && ID.Trim() == "")
                {
                    _data.Add(new dj_item()
                    {
                        Name = item.Name,
                        ID = item.ID
                    });

                }
                else
                {
                    if (ID.Trim() != "")
                    {
                        if (item.ID == Convert.ToInt32(ID))
                        {
                            _data.Add(new dj_item()
                            {
                                Name = item.Name,
                                ID = item.ID
                            });
                        }
                    }
                    else
                    {
                        if (item.Name.Trim().Contains(str.Trim()))
                        {
                            _data.Add(new dj_item()
                            {
                                Name = item.Name,
                                ID = item.ID
                            });
                        }
                    }


                }

            }

            _data = _data.OrderBy(p => p.Name).ToList();
            int Index = 1;
            foreach (var item in _data)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = item.ID;
                lvi.Text = Index.ToString();
                lvi.SubItems.Add(item.ID.ToString());
                lvi.SubItems.Add(item.Name.Trim());
                lvi_list.Add(lvi);

                Index++;
            }

            this.Invoke(new Action(delegate
            {
                mList = lvi_list;
                listView1.VirtualListSize = lvi_list.Count;

            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData(ItemID_TB.Text, ItemName_TB.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (ID_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品ID不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (COUNT_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品数量不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
update [GamewarehouseDB].dbo.[warehouseitem] set [ItemDataID] = {ID_TB.Text.Trim()},[ItemAmount] = '{COUNT_TB.Text.Trim()}' where [ItemInstanceID] = 
(select top 1 a.[ItemInstanceID] from [GameWarehouseDB].dbo.[warehouseitem] as a inner join [PlatformAcctDb].dbo.[Users] as b on a.[OwnerAccountID] = b.[UserId] where 1 = 1 and b.[UserId] = '{u_ID}' order by [RegistrationTime] desc)
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (ID_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品ID不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string sqlstr = $@"
            begin tran
begin try
delete [GameWarehouseDB].dbo.[warehouseitem] where [ItemInstanceID] = '{ID_TB.Text.Trim()}' and [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseGoods] where [ItemDataID] = '{ID_TB.Text.Trim()}' and  [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseStatistics];
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "清理成功！";
                        }
                        else
                        {
                            msg = "清理失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
            begin tran
begin try
delete [GameWarehouseDB].dbo.[warehouseitem] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseGoods] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseStatistics];
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "清理成功！";
                        }
                        else
                        {
                            msg = "清理失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
            Tips: 此程序修改游戏内[礼品盒]最新的一个物品
                    1.在游戏商品内购买一个物品，不要领取！
                    2.连接数据库后(自动保存记录)
                    3.输入账号后选择想要改的道具以及数量点击[确认提交]按钮
                    4.在游戏内领取礼物即可！(第一次失败,第二次成功)
            ----------------物品清理-------------------
                    1.ID清理需要选择 或输入物品的ID及[登录账号]--删除礼品箱属于此ID的物品
                    2.全部清理需要输入[登录账号]--删除该账号礼品箱的全部物品
                    3.删除后需要重新选择一次角色
", "方法说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{Check_PCID}'";

            try
            {
                if (db.IsConnection)
                {
                    Task.Run(() =>
                    {
                        db._SQL_IDU(sqlstr);
                        //操作成功
                        msg = "操作成功！";
                    });
                    listView2.Items.Clear();
                }
                else
                {
                    MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
insert into [BlGame01].dbo.[QuestProperty] 
([pcid],[data_id],[reward_multiplication_factor],[current_mission_variation],
[current_mission_step],[mission_completion_1],
[mission_achievement_value_1],[mission_completion_2],
[mission_achievement_value_2],[mission_completion_3],
[mission_achievement_value_3],[mission_completion_4],
[mission_achievement_value_4],[mission_completion_5],
[mission_achievement_value_5],[mission_completion_6],
[mission_achievement_value_6],[mission_completion_7],
[mission_achievement_value_7],[mission_completion_8],
[mission_achievement_value_8],[mission_completion_9],
[mission_achievement_value_9],[mission_completion_10],
[mission_achievement_value_10],[mission_completion_11],
[mission_achievement_value_11],[mission_completion_12],
[mission_achievement_value_12],[mission_completion_13],
[mission_achievement_value_13],[mission_completion_14],
[mission_achievement_value_14],[mission_completion_15],
[mission_achievement_value_15],[mission_completion_16],
[mission_achievement_value_16])
values(
'{Check_PCID}','{textBox8.Text.Trim()}','1','1','1'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0')

";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (db.IsConnection)
            {
                try
                {
                    string sql = $"select [UserId] from [PlatformAcctDb].dbo.[Users] where [LoginName] = N'{UNAME_TB.Text.Trim()}'+ N'@ncsoft.com'";
                    u_ID = db._SQLScalar(sql).ToString();
                    label22.Text = $"UID:{u_ID}";
                    UpdateAppConfig("UserAcction", UNAME_TB.Text.Trim());
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"兄弟 找不到这个账号啊！\r\n 错误给你看:{ex.Message}", "啊这..！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请等待数据库连接完成！","无法继续！",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

            
        }

        private void 完成度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listView2.SelectedItems.Count == 0)
            {

            }
            else
            {
                int Data_ID = Convert.ToInt32(listView2.SelectedItems[0].SubItems[1].Text);
                string sqlstr = $@"
update [BlGame01].dbo.[QuestProperty] set [current_mission_step] = [current_mission_step]+1  where [pcid] = '{Check_PCID}' and [data_id] = '{Data_ID}'";
                Task.Run(() =>
                {
                    try
                    {
                        if (db.IsConnection)
                        {
                            if (db._SQL_IDU(sqlstr) > 0)
                            {
                                this.Invoke(new Action(delegate
                                {
                                    listView2.SelectedItems[0].SubItems[2].Text = (Convert.ToInt32(listView2.SelectedItems[0].SubItems[2].Text) + 1).ToString();
                                }));
                                //操作成功
                                msg = "操作成功！";
                            }
                            else
                            {
                                msg = "操作失败！";
                            }

                        }
                        else
                        {
                            MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                });
            }
        }

        private void 完成度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listView2.SelectedItems.Count == 0)
            {

            }
            else
            {
                int Data_ID = Convert.ToInt32(listView2.SelectedItems[0].SubItems[1].Text);
                string sqlstr = $@"
update [BlGame01].dbo.[QuestProperty] set [current_mission_step] = [current_mission_step]-1  where [pcid] = '{Check_PCID}' and [data_id] = '{Data_ID}'";
                Task.Run(() =>
                {
                    try
                    {
                        if (db.IsConnection)
                        {
                            if (db._SQL_IDU(sqlstr) > 0)
                            {
                                this.Invoke(new Action(delegate
                                {
                                    listView2.SelectedItems[0].SubItems[2].Text = (Convert.ToInt32(listView2.SelectedItems[0].SubItems[2].Text) - 1).ToString();
                                }));
                                //操作成功
                                msg = "操作成功！";
                                
                            }
                            else
                            {
                                msg = "操作失败！";
                            }

                        }
                        else
                        {
                            MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                });
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listView2.SelectedItems.Count == 0)
            {

            }
            else
            {
                int Data_ID = Convert.ToInt32(listView2.SelectedItems[0].SubItems[1].Text);
                string sqlstr = $@"
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{Check_PCID}' and [data_id] = '{Data_ID}'";

                try
                {
                    if (db.IsConnection)
                    {
                        Task.Run(() =>
                        {
                            if (db._SQL_IDU(sqlstr) > 0)
                            {
                                //操作成功
                                msg = "操作成功！";
                                this.Invoke(new Action(delegate 
                                {
                                    listView2.Items.Remove(listView2.SelectedItems[0]);
                                }));
                            }
                            else
                            {
                                msg = "操作失败！";
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (LEVEL_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写等级！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (M_LEVEL_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写洪门等级！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"update [BlGame01].dbo.[CreatureProperty] set [level] = '{LEVEL_TB.Text.Trim()}',[mastery_level] = '{M_LEVEL_TB.Text.Trim()}' where [pcid] = '{Check_PCID}'
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox9.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写点券数量！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!APIMode)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (u_ID.Equals(""))
                {
                    MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int money = Convert.ToInt32(textBox9.Text.Trim());


                string Timer = db._SQLScalar("select top 1 [DepositRequestKey] from [VirtualCurrencyDb].dbo.[Deposits] order by [DepositId] desc").ToString();

                string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");

                string sqlstr = $@"exec [VirtualCurrencyDb].dbo.[p_AddDeposit] '{u_ID}','51','5','{textBox9.Text.Trim()}','{textBox9.Text.Trim()}','0','1900-01-01T00:00:00+00:00','2999-12-30 03:30:30.000 +09:00','99','{MD5Encrypt(u_ID + tim)}',N'GM工具充值','0','0'";
                Task.Run(() =>
                {
                    try
                    {
                        if (db.IsConnection)
                        {
                            db._SQL_IDU(sqlstr);
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                });
            }
            else if (APIMode)
            {
                if (UNAME_TB.Text.Trim() == "")
                {
                    MessageBox.Show("请填写账号！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Acction", UNAME_TB.Text.Trim());
                dic.Add("dq", textBox9.Text.Trim());
                Task.Run(() => 
                {
                    string rest = Tools.Get($"http://{db.ServerIP}:8012/Debug.aspx", dic);
                    if (rest == "success")
                    {
                        msg = "操作成功!";
                    }
                    else
                    {
                        msg = "操作失败!";
                    }
                });
                
            }

            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox6.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写金钱数量！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Convert.ToDouble(textBox6.Text.Trim()) > 19000000)
            {
                textBox6.Text = "19000000";
            }

            double money = Convert.ToDouble(textBox6.Text.Trim()) * 10000;

            List<string> list = new List<string>();
            list.Add($"[money] = '{money}'");
            Project(list, Check_PCID);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写需要增加的经验！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> list = new List<string>();
            list.Add($"[exp] = [exp] + '{textBox1.Text.Trim()}'");
            Project(list, Check_PCID);
        }

        private void Project(List<string> lst,int _PCID) 
        {
            string set_str = string.Empty;
            for (int i = 0; i < lst.Count; i++)
            {
                set_str += $"{lst[i]}";
                if (i < lst.Count - 1)
                {
                    set_str += ",";
                }
            }
            string sqlstr = $@"update [BlGame01].dbo.[CreatureProperty] set {set_str} where [pcid] = '{_PCID}'";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }
        private void SummProject(List<string> lst, int _PCID,string tp)
        {
            string set_str = string.Empty;
            for (int i = 0; i < lst.Count; i++)
            {
                set_str += $"{lst[i]}";
                if (i < lst.Count - 1)
                {
                    set_str += ",";
                }
            }
            string sqlstr = string.Empty;

            switch (tp)
            {
                case "Insert":
                    sqlstr = $@"Insert into [BlGame01].dbo.[SummonedProperty]([pcid],[slot],[race],[name],[appearance1],[appearance2],[appearance3],[appearance4],[appearance5],[appearance6],[appearance7]) values('{_PCID}',1,10,N'{lst[0]}',5,1,10,2,10,11,8)";
                    break;
                case "Update":
                    sqlstr = $@"update [BlGame01].dbo.[SummonedProperty] set {set_str} where [pcid] = '{_PCID}'";
                    break;
                case "Delete":
                    sqlstr = $@"Delete [BlGame01].dbo.[SummonedProperty] where [pcid] = '{_PCID}'";
                    break;
            }

            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            this.Invoke(new Action(delegate 
                            {
                                switch (tp)
                                {
                                    case "Insert":
                                        listView5.SelectedItems[0].SubItems[11].Text = $@"{lst[0]}";
                                        break;
                                    case "Update":
                                        listView5.SelectedItems[0].SubItems[11].Text = $@"{lst[0].Substring(11, lst[0].Length - 12)}";
                                        break;
                                    case "Delete":
                                        listView5.SelectedItems[0].SubItems[11].Text = $@"无";
                                        break;
                                }
                            }));
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写需要修改的名称！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string sqlstr = $@"update [LobbyDB].dbo.[Character] set [CharacterName] = N'{textBox2.Text.Trim()}',[PureCharacterName] = N'{textBox2.Text.Trim()}' where [pcid] = '{Check_PCID}'";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        db._SQL_IDU(sqlstr);
                        Thread.Sleep(500);
                        this.Invoke(new Action(delegate 
                        {
                            List<string> list = new List<string>();
                            list.Add($"[Name] = N'{textBox2.Text.Trim()}'");
                            Project(list, Check_PCID);
                        }));
                        
                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
            



        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
begin tran
begin try  
delete [VirtualCurrencyDb].dbo.[WithdrawalPerDeposit] where [UserID] = '{u_ID}';
delete [VirtualCurrencyDb].dbo.[Deposits] where [UserID] = '{u_ID}';
end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
            List<ProjectData> us_data = new List<ProjectData>();
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }
        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void _KeyPress1(object sender, KeyPressEventArgs e)
        {
            TextBox tbx = (sender) as TextBox;

            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)

                e.Handled = true;


            //小数点的处理。

            if ((int)e.KeyChar == 46)                           //小数点

            {

                if (tbx.Text.Length <= 0)

                    e.Handled = true;   //小数点不能在第一位

                else

                {

                    float f;

                    float oldf;

                    bool b1 = false, b2 = false;

                    b1 = float.TryParse(tbx.Text, out oldf);

                    b2 = float.TryParse(tbx.Text + e.KeyChar.ToString(), out f);

                    if (b2 == false)

                    {

                        if (b1 == true)

                            e.Handled = true;

                        else

                            e.Handled = false;

                    }

                }

            }
        }

        private void LEVEL_TB_TextChanged(object sender, EventArgs e)
        {
            if (LEVEL_TB.Text == "" || Convert.ToInt32(LEVEL_TB.Text) < 55)
            {
                M_LEVEL_TB.Text = "0";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                textBox8.Text = this.comboBox1.SelectedValue.ToString();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写需要增加的经验！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> list = new List<string>();
            list.Add($"[mastery_exp] = [mastery_exp] + '{textBox3.Text.Trim()}'");
            Project(list, Check_PCID);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox4.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写需要增加的经验！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> list = new List<string>();
            list.Add($"[faction_reputation] = [faction_reputation] + '{textBox4.Text.Trim()}'");
            Project(list, Check_PCID);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string dat = string.Empty;
            switch (JobStr)
            {
                case "剑士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0070004000000000000F0FFFF010000000000000000000040040140000000000000FCF1FFFFFFFF3B0000000000000000000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFFFC434E0020020000240000000004000000000000000000000300000080500000000000000000090C000000182000F00700000020000000001032000000F8FFFFFFFF080002008000E0FF00000000000000006000000400000000000020";
                    break;
                case "召唤师":
                    dat = "0x0000000000000000000000000000000000000000000000000000007004003C000000000080F0FFFF010000000000000000000040040140000000000000FCF1FFFFFFFF3B0000000000000000000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFF0440400020020000000000000004000080030000000000000300000080000000000000000000080C00000018000000040000803F000000001000000000F8FFFFFFFF080000000000E0C04000000000000000000000942F40000100515001";
                    break;
                case "气功师":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040604000000000000F0FFFF010000000000000000000040040140000000000000FCF1FFFFFFFF3B0000000000000000000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFF0440400020020000000000000004000000000000000000000300000080000000000000000000080C0000001800000004007E0020000000001000000000F8FFFFFFFF08000000000000C0000000000000000000000004000020000001E0";
                    break;
                case "力士":
                    dat = "0x000000000000200000000000000000000000000000000000000000F0C40004000000000080FFFFFF010000000000000000000040040140002000400004FCF1FFFFFFFF3B0000000000000800000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFFFC437E0020020000260000000004002080030000000000000300000080500000000000040000090C0800001800000004FC00002000000000B0FF0E0000F8FFFFFFFF080000000000E0FFC000000000000000200000FC6F40E0FFFE59E0078006";
                    break;
                case "灵剑士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040004180000000000F0FFFF010000000000000000000040040140000000000000FCF1FFFFFFFF3B0000000000000000000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFF0440400020020000000000000004000080030000000000000300000080000000000000000000080C0000001800000004000000A01F0000001000000000F8FFFFFFFF08000000000000C0C000000000000000000000144F0020010001E007";
                    break;
                case "拳师":
                    dat = "0x000000000000200000000000000000000000000000000000000000F01C0004000000000000F0FFFF010000000000000000000040040140000000000000FCF1FFFFFFFF3B0000000000000000000000C00100000000000000080000000000000000000000000000000000000000000000F0FF7FFFFC434E0020020000260000000004000000010000000000000300000080500000000000000000090C08000018000000FC01000020000000001000000000F8FFFFFFFF080000000000E0DF00000000000000000000000400002001000120";
                    break;
                default:
                    dat = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                    break;
            }
            string sqlstr = $@"
begin tran
begin try  
UPDATE [BlGame01].dbo.[QuestList] SET [flags] = {dat} WHERE [pcid] = '{Check_PCID}';
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{Check_PCID}';
insert into [BlGame01].dbo.[QuestProperty] 
([pcid],[data_id],[reward_multiplication_factor],[current_mission_variation],
[current_mission_step],[mission_completion_1],
[mission_achievement_value_1],[mission_completion_2],
[mission_achievement_value_2],[mission_completion_3],
[mission_achievement_value_3],[mission_completion_4],
[mission_achievement_value_4],[mission_completion_5],
[mission_achievement_value_5],[mission_completion_6],
[mission_achievement_value_6],[mission_completion_7],
[mission_achievement_value_7],[mission_completion_8],
[mission_achievement_value_8],[mission_completion_9],
[mission_achievement_value_9],[mission_completion_10],
[mission_achievement_value_10],[mission_completion_11],
[mission_achievement_value_11],[mission_completion_12],
[mission_achievement_value_12],[mission_completion_13],
[mission_achievement_value_13],[mission_completion_14],
[mission_achievement_value_14],[mission_completion_15],
[mission_achievement_value_15],[mission_completion_16],
[mission_achievement_value_16])
values(
'{Check_PCID}','{1668}','1','1','1'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0');
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {

                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (ID_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品ID不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (COUNT_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品数量不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string Acction = UNAME_TB.Text.Trim();
            string count = COUNT_TB.Text.Trim();
            string SenditemID = ID_TB.Text.Trim();
            
            Task.Run(() =>
            {
                try
                {
                    if (APIMode)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("Acction", Acction);
                        dic.Add("ItemID", SenditemID);
                        dic.Add("Count", count);
                        dic.Add("Pwd", "BNSServerP");
                        string SendAPI = Tools.Get($"http://{db.ServerIP}:8012/SendItem.aspx", dic);
                        if (SendAPI.Contains("Success"))
                        {
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }
                    }
                    else
                    {

                        if (db.IsConnection)
                        {
                            string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string sql_str = "select MAX(goodsid) from [GameWarehouseDB].dbo.[WarehouseGoods]";
                            int gd_id = 0;

                            try
                            {
                                gd_id = Convert.ToInt32(db._SQLScalar(sql_str)) + 1;
                            }
                            catch
                            {
                                if (DialogResult.No == MessageBox.Show("检测礼物记录为空 是否坚持发送？ 可能会失败！\r\n请确认是否继续执行", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                                {
                                    return;
                                }
                            }

                            sql_str = $@"
begin tran
begin try  
insert [GameWarehouseDB].dbo.[WarehouseGoods] values('{u_ID}',null,'{gd_id}',80169,null,null,'{tim}',2,'{tim}','{tim}',1);
update [GameWarehouseDB].dbo.[WarehouseStatistics] set [MaxLabelID]=(select IDENT_CURRENT('[GameWarehouseDB].dbo.[WarehouseGoods]'))  where [OwnerAccountID] = '{u_ID}';
insert  [GameWarehouseDB].dbo.[WarehouseItem] values((select IDENT_CURRENT('[GameWarehouseDB].dbo.[WarehouseGoods]')),'{u_ID}',null,null,'80248','{SenditemID}','{count}',null,1,null,'{tim}','{tim}');
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";


                            if (db._SQL_IDU(sql_str) > 0)
                            {
                                //操作成功
                                msg = "操作成功！";
                            }
                            else
                            {
                                msg = "操作失败！";
                            }
                        }
                        else
                        {
                            MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });

        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (ID_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品ID不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (COUNT_TB.Text.Trim().Equals(""))
            {
                MessageBox.Show("物品数量不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Convert.ToInt32(textBox10.Text.Trim() == "" ? "50" : textBox10.Text.Trim()) < 50)
            {
                if (DialogResult.No == MessageBox.Show("延迟太低可能导致服务器 服务崩溃！\r\n请确认是否继续执行", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    return;
                }
            }

            if (DialogResult.No == MessageBox.Show("确定全员发送？ 可能需要一点处理的时间！","执行准备",MessageBoxButtons.YesNo,MessageBoxIcon.Question))
            {
                return;
            }
            string count = COUNT_TB.Text.Trim();
            string id = ID_TB.Text.Trim();
            button24.Enabled = false;
            int ys = Convert.ToInt32(textBox10.Text.Trim() == "" ? "50" : textBox10.Text.Trim());
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        string sql_str = "select [UserId],[UserName] from [PlatformAcctDb].dbo.[Users]";
                        Dictionary<string,string> ALLUser_PID = new Dictionary<string,string>();
                        using (SqlDataReader sdr = db._SQLRead(sql_str))
                        {
                            while (sdr.Read())
                            {
                                ALLUser_PID.Add(sdr["UserId"].ToString(),sdr["UserName"].ToString());
                            }
                        }
                        this.Invoke(new Action(delegate
                        {
                            button24.Text = $"准备发送礼品:{ALLUser_PID.Count}";
                            label8.Text = "全员发送礼物准备中！";
                        }));

                        Dictionary<string, string> Error_User = new Dictionary<string, string>();
                        int Index = 0;
                        DateTime tims = DateTime.Now.AddSeconds((ALLUser_PID.Count * -1));
                        foreach (var item in ALLUser_PID)
                        {
                            tims = tims.AddSeconds(1);
                            string tim = tims.ToString("yyyy-MM-dd HH:mm:ss");
                            sql_str = "select MAX(goodsid) from [GameWarehouseDB].dbo.[WarehouseGoods]";
                            int gd_id = 0;
                            try
                            {
                                gd_id = Convert.ToInt32(db._SQLScalar(sql_str)) + 1;
                            }
                            catch
                            {
                                if (DialogResult.No == MessageBox.Show("检测礼物记录为空 是否坚持发送？ 可能会失败！\r\n请确认是否继续执行", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                                {
                                    return;
                                }
                            }
                            

                            sql_str = $@"
begin tran
begin try  
insert [GameWarehouseDB].dbo.[WarehouseGoods] values('{item.Key}',null,'{gd_id}',80169,null,null,'{tim}',2,'{tim}','{tim}',1);
update [GameWarehouseDB].dbo.[WarehouseStatistics] set [MaxLabelID]=(select IDENT_CURRENT('[GameWarehouseDB].dbo.[WarehouseGoods]'))  where [OwnerAccountID] = '{item.Key}';
insert  [GameWarehouseDB].dbo.[WarehouseItem] values((select IDENT_CURRENT('[GameWarehouseDB].dbo.[WarehouseGoods]')),'{item.Key}',null,null,'80248','{id}','{count}',null,1,null,'{tim}','{tim}');
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";
                            if (db._SQL_IDU(sql_str) <= 0)
                            {
                                Error_User.Add(item.Key, item.Value);
                            }
                            
                            this.Invoke(new Action(delegate
                            {
                                button24.Text = $"正在发送:{(Index + 1)}/{ALLUser_PID.Count}";
                                label8.Text = "正在处理请稍后！";
                            }));
                            Index++;
                            Thread.Sleep(ys);
                        }
                        msg = $"执行完成！ 共发送{ALLUser_PID.Count}件礼物. 成功{ALLUser_PID.Count - Error_User.Count}";
                        this.Invoke(new Action(delegate
                        {
                            button24.Enabled = true;

                            if (Error_User.Count > 0)
                            {
                                new ErrorMsg(Error_User).Show();
                            }

                        }));
                        
                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (textBox9.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写点券数量！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Convert.ToInt32(textBox10.Text.Trim() == "" ? "50" : textBox10.Text.Trim()) < 50)
            {
                if (DialogResult.No == MessageBox.Show("延迟太低可能导致服务器 服务崩溃！\r\n请确认是否继续执行", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    return;
                }
            }

            if (DialogResult.No == MessageBox.Show("确定全员发送？ 可能需要一点处理的时间！", "执行准备", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }
            int money = Convert.ToInt32(textBox9.Text.Trim());
            button25.Enabled = false;
            int ys = Convert.ToInt32(textBox10.Text.Trim() == "" ? "50" : textBox10.Text.Trim());
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        string sql_str = "select [UserId],[UserName] from [PlatformAcctDb].dbo.[Users]";
                        Dictionary<string, string> ALLUser_PID = new Dictionary<string, string>();
                        string Timer = db._SQLScalar("select top 1 [DepositRequestKey] from [VirtualCurrencyDb].dbo.[Deposits] order by [DepositId] desc").ToString();
                        string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");

                        using (SqlDataReader sdr = db._SQLRead(sql_str))
                        {
                            while (sdr.Read())
                            {
                                ALLUser_PID.Add(sdr["UserId"].ToString(), sdr["UserName"].ToString());
                            }
                        }
                        this.Invoke(new Action(delegate
                        {
                            label8.Text = $"准备发送点券:{ALLUser_PID.Count}";
                        }));

                        Dictionary<string, string> Error_User = new Dictionary<string, string>();
                        int Index = 0;
                        
                        foreach (var item in ALLUser_PID)
                        {
                            sql_str = $@"exec [VirtualCurrencyDb].dbo.[p_AddDeposit] '{item.Key}','51','5','{money}','{money}','0','1900-01-01T00:00:00+00:00','2999-12-30 03:30:30.000 +09:00','99','{MD5Encrypt(item.Key + tim)}',N'GM工具充值','0','0';";
                            db._SQL_IDU(sql_str);

                            this.Invoke(new Action(delegate
                            {
                                label8.Text = $"正在发送:{(Index + 1)}/{ALLUser_PID.Count}";
                            }));
                            Index++;
                            Thread.Sleep(ys);
                        }
                        

                        msg = $"执行完成！ 共发送{ALLUser_PID.Count}份点券. 成功{ALLUser_PID.Count - Error_User.Count}";
                        this.Invoke(new Action(delegate
                        {
                            button25.Enabled = true;

                            if (Error_User.Count > 0)
                            {
                                new ErrorMsg(Error_User).Show();
                            }

                        }));

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strText">待加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] targetData = md5.ComputeHash(Encoding.UTF8.GetBytes(strText));

            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String.ToUpper();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (DialogResult.Yes == MessageBox.Show("清理前请确认服务端已经关闭！！\r\n是否确认清除[礼品盒数据库]内的数据？ \r\n 会导致礼品盒所有数据全部消失！若有需要请提前备份！","确认重置",MessageBoxButtons.YesNo,MessageBoxIcon.Question))
            {
                string sqlstr = $@"
begin tran
            begin try

delete [GameWarehouseDB].dbo.[warehouseitem];
delete [GameWarehouseDB].dbo.[WarehouseGoods];
delete [GameWarehouseDB].dbo.[WarehouseStatistics];
delete [GameWarehouseDB].dbo.[ErrorLog];

            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran

";
                Task.Run(() =>
                {
                    try
                    {
                        if (db.IsConnection)
                        {
                            db._SQL_IDU(sqlstr);
                            msg = "已完成操作";
                        }
                        else
                        {
                            MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                });
            }

            
        }
        /// <summary>
        /// 获取本地连接网卡物理地址
        /// </summary>
        /// <returns></returns>
        public static List<string> getMacAddr_Local()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            List<string> Mac_List = new List<string>();
            string macAddress = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        string fRegistryKey = key + adapter.Id + "\\Connection";
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                            int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 &&
                                fPnpInstanceID.Substring(0, 3) == "PCI")
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                //for (int i = 1; i < 6; i++)
                                //{
                                //    macAddress = macAddress.Insert(3 * i - 1, "");
                                //}
                                if (!macAddress.Trim().Equals(""))
                                {
                                    Mac_List.Add(macAddress);
                                }
                                break;
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"本地网卡MAC地址获取异常:{ex.Message}");
                //这里写异常的处理
            }
            return Mac_List;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new Form2(this, db).ShowDialog();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (db.IsConnection)
            {
                try
                {
                    string sql = $"select [Game_Account_id] from [BlGame01].dbo.[CreatureProperty] where [name] = N'{textBox5.Text.Trim()}'";
                    u_ID = db._SQLScalar(sql).ToString();
                    label22.Text = $"UID:{u_ID}";
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"兄弟 找不到这个账号啊！\r\n 错误给你看:{ex.Message}", "啊这..！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            CheckData_User(User_Name_CheckTxT.Text.Trim(),textBox11.Text.Trim());
        }

        private void CheckData_User(string _UserName,string UserAcction)
        {
            string sqlstr = $@"select a.[Job],a.[pcid],a.[Name],a.[money],a.[level],a.[mastery_level],a.[exp],a.[faction],a.[duel_point],a.[mastery_exp],a.[faction_reputation],c.[LoginName],c.[UserId],
(select sum(Balance) from [VirtualCurrencyDb].dbo.[Deposits] as d where d.[UserID] = c.[UserId]) as [Balance]
,d.[name] as [summName]
from [BlGame01].dbo.[CreatureProperty] as a
inner join [LobbyDB].dbo.[Character] as b on a.[pcid] = b.[pcid]
inner join [PlatformAcctDb].dbo.[Users] as c on b.[GameAccountID] = c.[UserId]
left join [BlGame01].dbo.[SummonedProperty] as d on a.[pcid] = d.[pcid]
where b.[CharacterState] = '2'";

            if (!_UserName.Trim().Equals(""))
            {
                sqlstr += $@" and a.[name] like N'%{_UserName}%'";
            }
            if (!UserAcction.Trim().Equals(""))
            {
                sqlstr += $@" and c.[LoginName] = N'{UserAcction}'+N'@ncsoft.com'";
            }
            sqlstr += " ORDER BY c.[LoginName]";
            List<Check_UserData> us_data = new List<Check_UserData>();
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Check_UserData T = new Check_UserData()
                                {
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["exp"]),
                                    faction_reputation = sdr["faction_reputation"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["faction_reputation"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    mastery_exp = sdr["mastery_exp"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["mastery_exp"]),
                                    mastery_level = sdr["mastery_level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["mastery_level"]),
                                    money = sdr["money"] == DBNull.Value ? 0 : Convert.ToDecimal(sdr["money"]),
                                    Name = sdr["Name"].ToString(),
                                    LoginName = sdr["LoginName"].ToString(),
                                    UserID = sdr["UserID"].ToString(),
                                    PCID = sdr["PCID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["PCID"]),
                                    Balance = sdr["Balance"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["Balance"]),
                                    SummName = sdr["SummName"] == DBNull.Value ? "无" : sdr["SummName"].ToString(),
                                    duel = sdr["duel_point"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["duel_point"]),
                                    faction = sdr["faction"] == DBNull.Value ? "无" : sdr["faction"].ToString() == "1" ? "武林盟" : sdr["faction"].ToString() == "2" ? "浑天教" : "无",
                                    Job = Convert.ToInt32(sdr["Job"])
                                };
                                us_data.Add(T);
                            }
                        }
                        this.Invoke(new Action(() =>
                        {
                            listView5.Items.Clear();
                            for (int i = 0; i < us_data.Count; i++)
                            {
                                ListViewItem lvi = new ListViewItem();
                                lvi.Tag = us_data[i].PCID.ToString();
                                lvi.Text = (i + 1).ToString();
                                lvi.SubItems.Add(us_data[i].Name.ToString());
                                lvi.SubItems.Add(Convert.ToInt64(us_data[i].money / 10000).ToString());
                                lvi.SubItems.Add(us_data[i].Balance.ToString());
                                lvi.SubItems.Add(us_data[i].duel.ToString());
                                lvi.SubItems.Add(us_data[i].level.ToString());
                                lvi.SubItems.Add(us_data[i].mastery_level.ToString());
                                lvi.SubItems.Add(us_data[i].exp.ToString());
                                lvi.SubItems.Add(us_data[i].mastery_exp.ToString());
                                lvi.SubItems.Add(us_data[i].faction_reputation.ToString());
                                lvi.SubItems.Add(us_data[i].LoginName.Replace("@ncsoft.com", ""));
                                lvi.SubItems.Add(us_data[i].SummName);
                                lvi.SubItems.Add(us_data[i].faction);

                                string jobstring = string.Empty;
                                switch (us_data[i].Job)
                                {
                                    case 1:
                                        jobstring = "剑士";
                                        break;
                                    case 2:
                                        jobstring = "拳师";
                                        break;
                                    case 3:
                                        jobstring = "气功师";
                                        break;
                                    case 5:
                                        jobstring = "力士";
                                        break;
                                    case 6:
                                        jobstring = "召唤师";
                                        break;
                                    case 7:
                                        jobstring = "刺客";
                                        break;
                                    case 8:
                                        jobstring = "灵剑士";
                                        break;
                                    case 9:
                                        jobstring = "咒术师";
                                        break;
                                    case 10:
                                        jobstring = "气宗师";
                                        break;
                                    default:
                                        jobstring = "无";
                                        break;
                                }
                                lvi.SubItems.Add(jobstring);
                                listView5.Items.Add(lvi);
                            }

                        }));

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void Check_Btn_Click(object sender, EventArgs e)
        {
            if (!Check_Data_id_TXT.Text.Trim().Equals(""))
            {
                Check_User_Data_id_Count(Check_Data_id_TXT.Text.Trim());
            }
        }

        private void Check_User_Data_id_Count(string Data_id)
        {
            Task.Run(() =>
            {
                if (db.IsConnection)
                {
                    List<string> PCID_List = new List<string>();

                    try
                    {
                        string sqlstr = string.Empty;
                        List<string> Data;


                        Data = new List<string>();
                        sqlstr = $@"select [PCID] from [BlGame01].dbo.[AccessoryProperty] where [data_id] in ({Data_id})";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data.Add(sdr["PCID"].ToString());
                            }
                        }
                        PCID_List = PCID_List.Union(Data).ToList<string>();

                        Data = new List<string>();
                        sqlstr = $@"select [PCID] from [BlGame01].dbo.[GroceryProperty] where [data_id] in ({Data_id})";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data.Add(sdr["PCID"].ToString());
                            }
                        }
                        PCID_List = PCID_List.Union(Data).ToList<string>();

                        Data = new List<string>();
                        sqlstr = $@"select PCID from [BlGame01].dbo.[WeaponProperty] where [data_id] in ({Data_id})  or [appearance_item_data_id] in ({Data_id})";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data.Add(sdr["PCID"].ToString());
                            }
                        }
                        PCID_List = PCID_List.Union(Data).ToList<string>();

                        this.Invoke(new Action(() =>
                        {
                            listView5.Items.Clear();
                        }));

                        if (PCID_List.Count > 0)
                        {
                            //账号查找
                            string PID = string.Empty;
                            foreach (var item in PCID_List)
                            {
                                PID += $"'{item.ToString()}'";
                                if (item != PCID_List[PCID_List.Count - 1])
                                    PID += ",";
                            }
                            sqlstr = $@"select a.[Job],a.[pcid],a.[Name],a.[money],a.[level],a.[mastery_level],a.[exp],a.[faction],a.[duel_point],a.[mastery_exp],a.[faction_reputation],c.[LoginName],c.[UserId],
(select sum(Balance) from [VirtualCurrencyDb].dbo.[Deposits] as d where d.[UserID] = c.[UserId]) as [Balance]
,d.[name] as [summName]
from [BlGame01].dbo.[CreatureProperty] as a
inner join [LobbyDB].dbo.[Character] as b on a.[pcid] = b.[pcid]
inner join [PlatformAcctDb].dbo.[Users] as c on b.[GameAccountID] = c.[UserId]
left join [BlGame01].dbo.[SummonedProperty] as d on a.[pcid] = d.[pcid]
where b.[CharacterState] = '2' and a.[pcid] in ({PID})";

                            List<Check_UserData> us_data = new List<Check_UserData>();
                            using (SqlDataReader sdr = db._SQLRead(sqlstr))
                            {
                                while (sdr.Read())
                                {
                                    Check_UserData T = new Check_UserData()
                                    {
                                        exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["exp"]),
                                        faction_reputation = sdr["faction_reputation"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["faction_reputation"]),
                                        level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                        mastery_exp = sdr["mastery_exp"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["mastery_exp"]),
                                        mastery_level = sdr["mastery_level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["mastery_level"]),
                                        money = sdr["money"] == DBNull.Value ? 0 : Convert.ToDecimal(sdr["money"]),
                                        Name = sdr["Name"].ToString(),
                                        LoginName = sdr["LoginName"].ToString(),
                                        UserID = sdr["UserID"].ToString(),
                                        PCID = sdr["PCID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["PCID"]),
                                        Balance = sdr["Balance"] == DBNull.Value ? 0 : Convert.ToInt64(sdr["Balance"]),
                                        SummName = sdr["SummName"] == DBNull.Value ? "无" : sdr["SummName"].ToString(),
                                        duel = sdr["duel_point"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["duel_point"]),
                                        faction = sdr["faction"] == DBNull.Value ? "无" : sdr["faction"].ToString() == "1" ? "武林盟" : sdr["faction"].ToString() == "2" ? "浑天教" : "无",
                                        Job = Convert.ToInt32(sdr["Job"])
                                    };
                                    us_data.Add(T);
                                }
                            }
                            this.Invoke(new Action(() =>
                            {
                                listView5.Items.Clear();
                                for (int i = 0; i < us_data.Count; i++)
                                {
                                    ListViewItem lvi = new ListViewItem();
                                    lvi.Tag = us_data[i].PCID.ToString();
                                    lvi.Text = (i + 1).ToString();
                                    lvi.SubItems.Add(us_data[i].Name.ToString());
                                    lvi.SubItems.Add(Convert.ToInt64(us_data[i].money / 10000).ToString());
                                    lvi.SubItems.Add(us_data[i].Balance.ToString());
                                    lvi.SubItems.Add(us_data[i].duel.ToString());
                                    lvi.SubItems.Add(us_data[i].level.ToString());
                                    lvi.SubItems.Add(us_data[i].mastery_level.ToString());
                                    lvi.SubItems.Add(us_data[i].exp.ToString());
                                    lvi.SubItems.Add(us_data[i].mastery_exp.ToString());
                                    lvi.SubItems.Add(us_data[i].faction_reputation.ToString());
                                    lvi.SubItems.Add(us_data[i].LoginName.Replace("@ncsoft.com", ""));
                                    lvi.SubItems.Add(us_data[i].SummName);
                                    lvi.SubItems.Add(us_data[i].faction);
                                    string jobstring = string.Empty;
                                    switch (us_data[i].Job)
                                    {
                                        case 1:
                                            jobstring = "剑士";
                                            break;
                                        case 2:
                                            jobstring = "拳师";
                                            break;
                                        case 3:
                                            jobstring = "气功师";
                                            break;
                                        case 5:
                                            jobstring = "力士";
                                            break;
                                        case 6:
                                            jobstring = "召唤师";
                                            break;
                                        case 7:
                                            jobstring = "刺客";
                                            break;
                                        case 8:
                                            jobstring = "灵剑士";
                                            break;
                                        case 9:
                                            jobstring = "咒术师";
                                            break;
                                        case 10:
                                            jobstring = "气宗师";
                                            break;
                                        default:
                                            jobstring = "无";
                                            break;
                                    }
                                    lvi.SubItems.Add(jobstring);
                                    listView5.Items.Add(lvi);
                                }

                            }));
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show($"警告！ 出现异常错误！ 无法继续执行 但不影响数据！\r\n错误消息:\r\n{ex.Message}", "无法继续", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            });

        }

        private void 批量删除再次指定IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> dat = new Dictionary<int, string>();
            foreach (ListViewItem item in listView5.CheckedItems)
            {
                dat.Add(Convert.ToInt32(item.Tag), item.SubItems[1].Text);
            }
            if (dat.Count > 0)
            {
                new Del_data_id(dat, db, Check_Data_id_TXT.Text.Trim()).ShowDialog();
            }
        }

        private void 批量删除彻底删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dat = new Dictionary<string, string>();
            foreach (ListViewItem item in listView4.CheckedItems)
            {
                dat.Add(item.Tag.ToString(), item.SubItems[1].Text);
            }
            if (dat.Count > 0)
            {
                string tips = string.Empty;
                string data_str = string.Empty;
                foreach (var item in dat)
                {
                    tips += $"[{item.Value}]\r\n";
                    data_str += $"'{item.Key}',";
                }
                if (data_str.Substring(data_str.Length - 1, 1) == ",")
                {
                    data_str = data_str.Substring(0, data_str.Length - 1);
                }
                DialogResult di = MessageBox.Show($"请确认删除该角色PCID:{Check_PCID}的下列道具:\r\n{tips}", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (di == DialogResult.Yes)
                {
                    Task.Run(() =>
                    {
                        string sqlstr = $@"
begin tran
begin try  
delete [BlGame01].dbo.[AccessoryProperty] where [pcid] = '{Check_PCID}' and [id] in ({data_str})
delete [BlGame01].dbo.[GroceryProperty] where [pcid] = '{Check_PCID}' and [id] in ({data_str})
delete [BlGame01].dbo.[WeaponProperty] where [pcid] = '{Check_PCID}' and [id] in ({data_str})
delete [BlGame01].dbo.[CostumeProperty] where [pcid] = '{Check_PCID}' and [id] in ({data_str})
delete [BlGame01].dbo.[Closet] where [pcid] = '{Check_PCID}' and [ItemInstanceID] in ({data_str})
delete [BlGame01].dbo.[GemProperty] where [pcid] = '{Check_PCID}' and [id] in ({data_str})
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                        if (db.IsConnection)
                        {
                            int count = db._SQL_IDU(sqlstr);
                            this.Invoke(new Action(delegate
                            {
                                foreach (ListViewItem item in listView5.CheckedItems)
                                {
                                    listView4.Items.Remove(item);
                                }
                                MessageBox.Show($"已完成操作！受影响数据共:{count}条！", "执行完毕");
                            }));
                        }
                    });
                }

            }
        }

        private void listView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 0)
            {
                groupBox12.Text = $"角色名:未选择角色";
                groupBox12.ForeColor = Color.Red;
            }
            else if (listView5.SelectedItems.Count == 1)
            {
                groupBox12.Text = $"角色名:{listView5.SelectedItems[0].SubItems[1].Text}";
                Check_PCID = Convert.ToInt32(listView5.SelectedItems[0].Tag);
                JobStr = listView5.SelectedItems[0].SubItems[13].Text;
                groupBox12.ForeColor = Color.DodgerBlue;

                if (!LockProjcet)
                {
                    textBox2.Text = listView5.SelectedItems[0].SubItems[1].Text;
                    textBox6.Text = listView5.SelectedItems[0].SubItems[2].Text;
                    textBox22.Text = listView5.SelectedItems[0].SubItems[4].Text;
                    LEVEL_TB.Text = listView5.SelectedItems[0].SubItems[5].Text;
                    M_LEVEL_TB.Text = listView5.SelectedItems[0].SubItems[6].Text;
                    textBox1.Text = listView5.SelectedItems[0].SubItems[7].Text;
                    textBox3.Text = listView5.SelectedItems[0].SubItems[8].Text;
                    textBox4.Text = listView5.SelectedItems[0].SubItems[9].Text;
                    CheckAcction = listView5.SelectedItems[0].SubItems[10].Text;
                    Check_Data_ID(Check_PCID, textBox13.Text.Trim(), textBox12.Text.Trim());
                }

            }
        }
        private void Check_Data_ID(int PCID,string item_id,string item_name)
        {
            if (!Lock_Check)
            {
                bool check_id = checkBox1.Checked;
                bool check_name = checkBox2.Checked;
                Task.Run((Action)(() =>
                {
                    try
                    {
                        Lock_Check = true;
                        List<Data_id_CountEntity> list = new List<Data_id_CountEntity>();
                        string sqlstr = $@"select [id],[data_id],[exp],[level] from [BlGame01].dbo.[AccessoryProperty] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["id"].ToString(),
                                    count = 1,
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Data_Name = "",
                                    types = "首饰",
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["exp"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    app_item_data_Name = "无",
                                    gem_1 = "无",
                                    gem_2 = "无",
                                    gem_3 = "无",
                                    gem_4 = "无",
                                    gem_5 = "无",
                                    gem_6 = "无"
                                };
                                list.Add(entity);
                            }
                        }
                        sqlstr = $@"select [id],[count],[data_id],[level],[exp] from [BlGame01].dbo.[GroceryProperty] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["id"].ToString(),
                                    count = sdr["count"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["count"]),
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Data_Name = "",
                                    types = "材料/消耗品",
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["exp"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    app_item_data_Name = "无",
                                    gem_1 = "无",
                                    gem_2 = "无",
                                    gem_3 = "无",
                                    gem_4 = "无",
                                    gem_5 = "无",
                                    gem_6 = "无"
                                };
                                list.Add(entity);
                            }
                        }
                        sqlstr = $@"select [id],[data_id],[level],[exp],[appearance_item_data_id],[gem_1],[gem_2],[gem_3],[gem_4],[gem_5],[gem_6] from [BlGame01].dbo.[WeaponProperty] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["id"].ToString(),
                                    count = 1,
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Data_Name = "",
                                    types = "武器",
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["exp"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    app_item_data_ID = sdr["appearance_item_data_id"].ToString() == "0" ? "" : sdr["appearance_item_data_id"].ToString(),
                                    app_item_data_Name = sdr["appearance_item_data_id"].ToString() == "0" ? "无" : sdr["appearance_item_data_id"].ToString(),
                                    gem_1 = sdr["gem_1"].ToString() == "0" ? "无" : sdr["gem_1"].ToString() == "65011" ? "无" : sdr["gem_1"].ToString(),
                                    gem_2 = sdr["gem_2"].ToString() == "0" ? "无" : sdr["gem_2"].ToString() == "65011" ? "无" : sdr["gem_2"].ToString(),
                                    gem_3 = sdr["gem_3"].ToString() == "0" ? "无" : sdr["gem_3"].ToString() == "65011" ? "无" : sdr["gem_3"].ToString(),
                                    gem_4 = sdr["gem_4"].ToString() == "0" ? "无" : sdr["gem_4"].ToString() == "65011" ? "无" : sdr["gem_4"].ToString(),
                                    gem_5 = sdr["gem_5"].ToString() == "0" ? "无" : sdr["gem_5"].ToString() == "65011" ? "无" : sdr["gem_5"].ToString(),
                                    gem_6 = sdr["gem_6"].ToString() == "0" ? "无" : sdr["gem_6"].ToString() == "65011" ? "无" : sdr["gem_6"].ToString(),
                                };
                                list.Add(entity);
                            }
                        }

                        sqlstr = $@"select [id],[data_id],[level],[exp] from [BlGame01].dbo.[GemProperty] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["id"].ToString(),
                                    count = 1,
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Data_Name = "",
                                    types = "八卦",
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["exp"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    app_item_data_Name = "无",
                                    gem_1 = "无",
                                    gem_2 = "无",
                                    gem_3 = "无",
                                    gem_4 = "无",
                                    gem_5 = "无",
                                    gem_6 = "无"
                                };
                                list.Add(entity);
                            }
                        }

                        sqlstr = $@"select [id],[data_id],[level],[exp] from [BlGame01].dbo.[CostumeProperty] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["id"].ToString(),
                                    count = 1,
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Data_Name = "",
                                    types = "衣服",
                                    exp = sdr["exp"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["exp"]),
                                    level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    app_item_data_Name = "无",
                                    gem_1 = "无",
                                    gem_2 = "无",
                                    gem_3 = "无",
                                    gem_4 = "无",
                                    gem_5 = "无",
                                    gem_6 = "无"
                                };
                                list.Add(entity);
                            }
                        }

                        sqlstr = $@"select [ItemInstanceID],[ItemDataID] from [BlGame01].dbo.[Closet] where [PCID] = '{PCID}'";
                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                Data_id_CountEntity entity = new Data_id_CountEntity()
                                {
                                    id = sdr["ItemInstanceID"].ToString(),
                                    count = 1,
                                    data_id = sdr["ItemDataID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ItemDataID"]),
                                    Data_Name = "",
                                    types = "衣柜",
                                    exp = 0,
                                    level = 0,
                                    app_item_data_Name = "无",
                                    gem_1 = "无",
                                    gem_2 = "无",
                                    gem_3 = "无",
                                    gem_4 = "无",
                                    gem_5 = "无",
                                    gem_6 = "无"
                                };
                                list.Add(entity);
                            }
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            List<dj_item> dj = Data.Where(it => it.ID == list[i].data_id).ToList();
                            if (dj.Count > 0)
                            {
                                list[i].Data_Name = dj[0].Name;
                            }
                            if (list[i].app_item_data_Name !="无")
                            {
                                dj = Data.Where((Func<dj_item, bool>)(it => it.ID == Convert.ToInt64((string)list[i].app_item_data_Name))).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].app_item_data_Name = dj[0].Name;
                                }
                                
                            }
                            if (list[i].gem_1 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_1)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_1 = dj[0].Name;
                                }

                            }
                            if (list[i].gem_2 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_2)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_2 = dj[0].Name;
                                }

                            }
                            if (list[i].gem_3 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_3)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_3 = dj[0].Name;
                                }

                            }
                            if (list[i].gem_4 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_4)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_4 = dj[0].Name;
                                }

                            }
                            if (list[i].gem_5 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_5)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_5 = dj[0].Name;
                                }

                            }
                            if (list[i].gem_6 != "无")
                            {
                                dj = Data.Where(it => it.ID == Convert.ToInt64(list[i].gem_6)).ToList();
                                if (dj.Count > 0)
                                {
                                    list[i].gem_6 = dj[0].Name;
                                }

                            }
                        }

#region 过滤
                        if (check_id)
                        {
                            if (item_id != "")
                            {
                                int forcount = item_id.Length - item_id.Replace(",", "").Length == 0 ? 1 : item_id.Length - item_id.Replace(",", "").Length + 1;
                                List<int> gl_item = new List<int>();
                                for (int i = 0; i < forcount; i++)
                                {
                                    string str = string.Empty;

                                    if (i == forcount - 1)
                                    {
                                        str = item_id.Substring(0);
                                        item_id = item_id.Substring(str.Length);
                                    }
                                    else
                                    {
                                        str = item_id.Substring(0, item_id.IndexOf(","));
                                        item_id = item_id.Substring(str.Length + 1);
                                    }

                                    gl_item.Add(Convert.ToInt32(str));
                                }
                                List<Data_id_CountEntity> _Visit_item = new List<Data_id_CountEntity>();
                                for (int i = 0; i < gl_item.Count; i++)
                                {
                                    _Visit_item.AddRange(list.Where(it => it.data_id == gl_item[i]));
                                }
                                list = _Visit_item;
                            }
                        }
                        if (check_name)
                        {
                            if (item_name != "")
                            {
                                int forcount = item_name.Length - item_name.Replace(",", "").Length == 0 ? 1 : item_name.Length - item_name.Replace(",", "").Length + 1;
                                List<string> gl_item = new List<string>();
                                for (int i = 0; i < forcount; i++)
                                {
                                    string str = string.Empty;

                                    if (i == forcount - 1)
                                    {
                                        str = item_name.Substring(0);
                                        item_name = item_name.Substring(str.Length);
                                    }
                                    else
                                    {
                                        str = item_name.Substring(0, item_name.IndexOf(","));
                                        item_name = item_name.Substring(str.Length + 1);
                                    }

                                    gl_item.Add(str);
                                }
                                List<Data_id_CountEntity> _Visit_item = new List<Data_id_CountEntity>();
                                for (int i = 0; i < gl_item.Count; i++)
                                {
                                    _Visit_item.AddRange(list.Where(it => it.Data_Name.Contains(gl_item[i])));
                                }
                                list = _Visit_item;
                            }
                        }
#endregion

                        this.Invoke(new Action(() =>
                        {
                            listView4.Items.Clear();
                            for (int i = 0; i < list.Count; i++)
                            {
                                ListViewItem lvi = new ListViewItem();
                                lvi.Tag = list[i].id.ToString();
                                lvi.Text = (i + 1).ToString();
                                lvi.SubItems.Add(list[i].Data_Name == "" ? list[i].data_id.ToString() : list[i].Data_Name);
                                lvi.SubItems.Add(list[i].types);
                                lvi.SubItems.Add(list[i].data_id.ToString());
                                lvi.SubItems.Add(list[i].count.ToString());
                                lvi.SubItems.Add(list[i].exp.ToString());
                                lvi.SubItems.Add(list[i].app_item_data_Name);
                                lvi.SubItems[6].Tag = list[i].app_item_data_ID == null ? "" : list[i].app_item_data_ID;
                                lvi.SubItems.Add(list[i].gem_1);
                                lvi.SubItems.Add(list[i].gem_2);
                                lvi.SubItems.Add(list[i].gem_3);
                                lvi.SubItems.Add(list[i].gem_4);
                                lvi.SubItems.Add(list[i].gem_5);
                                lvi.SubItems.Add(list[i].gem_6);
                                listView4.Items.Add(lvi);
                            }

                        }));

                        sqlstr = $@"
select [data_id],[current_mission_step] from [BlGame01].dbo.[QuestProperty] where [pcid] = '{PCID}'";
                        List<UserData> us_data = new List<UserData>();

                        using (SqlDataReader sdr = db._SQLRead(sqlstr))
                        {
                            while (sdr.Read())
                            {
                                UserData T = new UserData()
                                {
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    current_mission_step = sdr["current_mission_step"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["current_mission_step"]),
                                };
                                us_data.Add(T);

                            }
                        }
                        this.Invoke(new Action(() =>
                        {
                            listView2.Items.Clear();
                            for (int i = 0; i < us_data.Count; i++)
                            {
                                ListViewItem lvi = new ListViewItem();
                                lvi.Tag = us_data[i].PCID;
                                lvi.Text = (i + 1).ToString();
                                lvi.SubItems.Add(us_data[i].data_id.ToString());
                                lvi.SubItems.Add(us_data[i].current_mission_step.ToString());
                                listView2.Items.Add(lvi);
                            }

                        }));

                        Lock_Check = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出错辣~~~ 错误消息:{ex.Message}", "无法执行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    finally 
                    {
                        Lock_Check = false;
                    }
                }));
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
UPDATE [BlGame01].dbo.[ActivatedTeleport] SET [flags] = 0xF79FFF7FFE1BFAFF1FDEFF777FFFFFFCFF3F  WHERE [pcid] = '{Check_PCID}'";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {

                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string dat = string.Empty;
            switch (JobStr)
            {
                case "剑士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0070004020000000008F0FFFF698D099307C470E23FE1BF7A9671777EC0FF060F20FCF1FFFFFFFF3BC0FEFEBFF60200FFDFFFFFFFFFFFFDFF7E0000000800000000C0FFDE0FFE9F371C00000000F7DCFFFF7F8F0BF0FF7FFF04EC4100A0FF030000FFFBFFFF97070000C0EFFBFFB7FF0083FEFECFB9AA00F0FFFF7F00000000D4F3FEFF3B0000F0070200002000000000B03B";
                    break;
                case "召唤师":
                    dat = "0x0000000000000000000000000000000000000000000000000000007000007C020000000008F0FFFF698D091305C4704239C1BF3A16417760420D040F00FCF1FFFFFFFF3BC0FFFE3F82000094FAFFFFFFFFFF7EFF6E0000000800000000C0FFDE0FFE9F371C00000000F7DCFB7F7F8C0BF0FF7FFF04A8010020FF030000FFFBFFFF97070000C0EDFBFF37FE0083FEFEDFF9AB00F0FFFF7F00000000D4B7FE7F38000000040200803F00000000B03F";
                    break;
                case "气功师":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040604020000000008F0FFFF698D099307E470F23FE1BF4AD671777EC2FE060F00FCF1FFFFFFFF3BC0FEFFBFDE0200BDDEFFFFFFFFFF7EBD5C0000000800000000C0FFDC0FFC9B351C00000000F7DCFBFF7F8C03F0FF7FFF04E8410020FF030000FFFBFF7F97070000C0EDFBFF37FE0083EE78CFF9AA00F0FFF77F00000000D4B3025F1900000004027E002000000000B03F";
                    break;
                case "力士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0C40004020000000008F0FFFF698D099307E470F23FE1BF4AD671677EC0FF060F20FCF1FFFFFFFF3BC0FFFFBFFE0300BDFEFFFFDFFFFF7EFF7E0000000800000000C0FFDE0FFC9D170800000000E7C4FE3F7B0C0BF0FF7FFF04E84100A0FF030000FFFBFF7F15000000C0EDFBFF37F600824CFB8011A800D0BF7F0300000000D4B2025F1B00000004FC00002060000000B03D";
                    break;
                case "灵剑士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F00400041A0000000008F0FFFF690400000420004203002A72044177120212020E00FCF1FFFFFFFF3BC07F91030A000084420100E0D9A66A09300000000800000000C0190800D0810018000000008094FF4B068C01F0FF7FFF04800100201F00000097A1FB7716000000C08DFB8337F60083FC5001B08A00F0FE9760000000001095021F1900000004020000A01F000000B03B";
                    break;
                case "拳师":
                    dat = "0x000000000000000000000000000000000000000000000000000000F01C0004020000000008F0FFFF698D099307E070F23FE1BF7AD671777EC0FF060F00FCF1FFFFFFFF3BC0FFFFBF8E000072FDFFFFFFFFFFFFFE7F0000000800000000C0FFDE0AFE97351C00000000A01C0103000300F0FF7FFF04FC410020FF030000FFFBFF7FB7070000C0C9FBFF37F80083EEF9D7F9AB00F0FFFF7F00000000D437FBE019000000FC0300002000000000B03B00000008";
                    break;
                case "刺客":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0043004020000000000F0FFFF08810092040000020160A20286415712C0FF020E00FCF1FFFFFFFF3B406211004A010042C18046E121C0C742210000000800000000C0094808D080001800000000C5D4F97A7F8309F0FF7FFF045C0100A0FF00000065A06B2185070000C0C2430930E0008302E886912B0050A3160000000000D473FFFF390000000402003F2000000000B03F";
                    break;
                case "咒术师":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040004020000000008F0FFFF68890993070440023B00B77A946147000208000E00FCF1FFFFFFFF3BC0283E001000007B7D9BF8C7FB7EC88C5000000008000000000060140ACC19140000000000C684FA3B020303F0FF7FFF0404000020020000009FFBFD8787070000C0EAF3FD87F100834A6A86912A00F0BF5E0700807F00D476FA401A000000040200002000000000B03F";
                    break;
                default:
                    dat = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                    break;
            }

            string sqlstr = $@"
begin tran
begin try  
UPDATE [BlGame01].dbo.[QuestList] SET [flags] = {dat} WHERE [pcid] = '{Check_PCID}';
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{Check_PCID}';
insert into [BlGame01].dbo.[QuestProperty] 
([pcid],[data_id],[reward_multiplication_factor],[current_mission_variation],
[current_mission_step],[mission_completion_1],
[mission_achievement_value_1],[mission_completion_2],
[mission_achievement_value_2],[mission_completion_3],
[mission_achievement_value_3],[mission_completion_4],
[mission_achievement_value_4],[mission_completion_5],
[mission_achievement_value_5],[mission_completion_6],
[mission_achievement_value_6],[mission_completion_7],
[mission_achievement_value_7],[mission_completion_8],
[mission_achievement_value_8],[mission_completion_9],
[mission_achievement_value_9],[mission_completion_10],
[mission_achievement_value_10],[mission_completion_11],
[mission_achievement_value_11],[mission_completion_12],
[mission_achievement_value_12],[mission_completion_13],
[mission_achievement_value_13],[mission_completion_14],
[mission_achievement_value_14],[mission_completion_15],
[mission_achievement_value_15],[mission_completion_16],
[mission_achievement_value_16])
values(
'{Check_PCID}','{1420}','1','1','1'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0');
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran

";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {

                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
UPDATE [BlGame01].dbo.[ActivatedTeleport] SET [flags] = 0xF7FFFFFFFFFBFBFF1F0E000040  WHERE [pcid] = '{Check_PCID}'";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {

                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void 修改数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                if (listView4.SelectedItems[0].SubItems[2].Text == "材料/消耗品")
                {
                    ChangedCount chang = new ChangedCount(db,this, Check_PCID, listView4.SelectedItems[0].Tag.ToString(), listView4.SelectedItems[0].SubItems[4].Text);
                    if (DialogResult.OK == chang.ShowDialog())
                    {
                        listView4.SelectedItems[0].SubItems[4].Text = chang.ReturnValue1.ToString();
                        Task.Run(()=> 
                        {
                            msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                        });
                    }
                }
                
            }
        }

        private void contextMenuStrip4_VisibleChanged(object sender, EventArgs e)
        {
            if (contextMenuStrip4.Visible)
            {
                if (listView4.SelectedItems.Count > 0)
                {
                    if (listView4.SelectedItems[0].SubItems[2].Text == "材料/消耗品")
                    {
                        修改数量ToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        修改数量ToolStripMenuItem.Enabled = false;
                    }
                    if (listView4.SelectedItems[0].SubItems[2].Text == "武器" || listView4.SelectedItems[0].SubItems[2].Text == "首饰")
                    {
                        修改为10段ToolStripMenuItem.Enabled = true;
                        修改幻化IDToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        修改为10段ToolStripMenuItem.Enabled = false;
                        修改幻化IDToolStripMenuItem.Enabled = true;
                    }
                    if (listView4.SelectedItems[0].SubItems[2].Text == "衣柜")
                    {
                        修改为10段ToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        修改为10段ToolStripMenuItem.Enabled = true;
                    }
                }

            }
            
        }

        private void 复制所属账号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string Acction = string.Empty;
                if (listView5.CheckedItems.Count > 0)
                {
                    foreach (ListViewItem item in listView5.CheckedItems)
                    {
                        Acction += item.SubItems[10].Text.Replace("@ncsoft.com", "");
                        Acction += "\r\n";
                    }
                }
                else
                {
                    if (listView5.SelectedItems.Count > 0)
                    {
                        Acction += listView5.SelectedItems[0].SubItems[10].Text.Replace("@ncsoft.com", "");
                    }
                    
                }
                
                
                Clipboard.Clear();//清空剪切板内容

                Clipboard.SetData(DataFormats.Text, Acction);//复制内容到剪切板
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制时出现了异常:\r\n{ex.Message}");
                //MessageBox.Show($"复制时出现了异常:\r\n{ex.Message}","执行失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void 复制角色名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string UserName = string.Empty;
                if (listView5.CheckedItems.Count > 0)
                {
                    foreach (ListViewItem item in listView5.CheckedItems)
                    {
                        UserName += item.SubItems[1].Text;
                        UserName += "\r\n";
                    }
                }
                else
                {
                    if (listView5.SelectedItems.Count > 0)
                    {
                        UserName += listView5.SelectedItems[0].SubItems[1].Text;
                    }

                }
                
                Clipboard.Clear();//清空剪切板内容

                Clipboard.SetData(DataFormats.Text, UserName);//复制内容到剪切板
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制时出现了异常:\r\n{ex.Message}");
                //MessageBox.Show($"复制时出现了异常:\r\n{ex.Message}", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 复制道具IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView4.SelectedItems.Count == 1)
                {
                    Clipboard.Clear();//清空剪切板内容

                    Clipboard.SetData(DataFormats.Text, listView4.SelectedItems[0].SubItems[3].Text);//复制内容到剪切板
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制时出现了异常:\r\n{ex.Message}");
                //MessageBox.Show($"复制时出现了异常:\r\n{ex.Message}","执行失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void 复制道具名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView4.SelectedItems.Count == 1)
                {
                    Clipboard.Clear();//清空剪切板内容

                    Clipboard.SetData(DataFormats.Text, listView4.SelectedItems[0].SubItems[1].Text);//复制内容到剪切板
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制时出现了异常:\r\n{ex.Message}");
                //MessageBox.Show($"复制时出现了异常:\r\n{ex.Message}","执行失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void 复制角色PCIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView5.SelectedItems.Count == 1)
                {
                    Clipboard.Clear();//清空剪切板内容

                    Clipboard.SetData(DataFormats.Text, listView5.SelectedItems[0].Tag);//复制内容到剪切板
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制时出现了异常:\r\n{ex.Message}");
                //MessageBox.Show($"复制时出现了异常:\r\n{ex.Message}","执行失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> list = new List<string>();
            list.Add($"[geo_zone] = '2310'");
            list.Add($"[x] = '4043'");
            list.Add($"[y] = '2441'");
            list.Add($"[z] = '20'");
            Project(list, Check_PCID);
        }

        private void 修改为10段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ChangedData chang = new ChangedData(db, this, Check_PCID, listView4.SelectedItems[0].Tag.ToString(), listView4.SelectedItems[0].SubItems[5].Text, "exp",Data);
                if (DialogResult.OK == chang.ShowDialog())
                {
                    listView4.SelectedItems[0].SubItems[5].Text = chang.ReturnValue1.ToString();
                    Task.Run(() =>
                    {
                        msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                    });
                }

            }
        }
        
        private void button34_Click(object sender, EventArgs e)
        {
            
            string sqlstr = string.Empty;
            sqlstr += $@"
begin tran
begin try
--财产转移
update [BlGame01].dbo.[GemProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 八卦
update [BlGame01].dbo.[AccessoryProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 首饰
update [BlGame01].dbo.[GroceryProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 材料/消耗品
update [BlGame01].dbo.[WeaponProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 武器
update [BlGame01].dbo.[CostumeProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 衣服
update [BlGame01].dbo.[SummonedProperty] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 召唤兽
update [BlGame01].dbo.[Closet] set [pcid] = '{textBox16.Text.Trim()}' where [pcid]='{textBox17.Text.Trim()}'; -- 衣柜

update [BlGame01].dbo.[CreatureProperty] set [depository_size]=(select [depository_size] from [BlGame01].dbo.[CreatureProperty] where [pcid] = '{textBox17.Text.Trim()}'),[wardrobe_size] = (select [wardrobe_size] from [BlGame01].dbo.[CreatureProperty] where [pcid] = '{textBox17.Text.Trim()}') where [pcid] = '{textBox16.Text.Trim()}'; -- 仓库解锁度

update [BlGame01].dbo.[RandomStore] set [slot_count] = (select [slot_count] from [BlGame01].dbo.[RandomStore] where [pcid]= '{textBox17.Text.Trim()}' and [random_store_number] = '1') where [pcid] = '{textBox16.Text.Trim()}' and [random_store_number] = '1'; --左侧聚灵阁

update [BlGame01].dbo.[RandomStore] set [slot_count] = (select [slot_count] from [BlGame01].dbo.[RandomStore] where [pcid]= '{textBox17.Text.Trim()}' and [random_store_number] = '2') where [pcid] = '{textBox16.Text.Trim()}' and [random_store_number] = '2'; --右侧聚灵阁
";

            if (DialogResult.Yes == MessageBox.Show("是否同步 [主线进度]、[遁地完成度]、[成就进度]", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                sqlstr += $@"update [BlGame01].dbo.[QuestList] set [flags] = (select top 1 [flags] from [BlGame01].dbo.[QuestList] where [pcid] = '{textBox17.Text.Trim()}') where [pcid] = '{textBox16.Text.Trim()}'; --任务进度
update [BlGame01].dbo.[ActivatedTeleport] set [flags] = (select top 1 [flags] from [BlGame01].dbo.[ActivatedTeleport] where [pcid] = '{textBox17.Text.Trim()}') where [pcid] = '{textBox16.Text.Trim()}'; --遁地解锁进度
delete [AchievementDB].dbo.[Register] where [pcid] = '{textBox16.Text.Trim()}';--删除原成就
update [AchievementDB].dbo.[Register] set [pcid] = '{textBox16.Text.Trim()}' where [pcid] = '{textBox17.Text.Trim()}';--解锁成就
delete [AchievementDB].dbo.[Completed] where [pcid] = '{textBox16.Text.Trim()}';--删除原成就奖励
update [AchievementDB].dbo.[Completed] set [pcid] = '{textBox16.Text.Trim()}' where [pcid] = '{textBox17.Text.Trim()}';
";
                
            }
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        sqlstr += $@"

     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
 ";
                        int count = db._SQL_IDU(sqlstr);

                        msg = $"已完成操作！受影响数据共:{count}条！";


                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void 修改势力为浑天教ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Check_PCID == 0)
                {
                    MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                List<string> list = new List<string>();
                list.Add($"[faction] = '{2}'");
                Project(list, Check_PCID);
            }
        }

        private void 修改势力为武林盟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Check_PCID == 0)
                {
                    MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                List<string> list = new List<string>();
                list.Add($"[faction] = '{1}'");
                Project(list, Check_PCID);
            }
            
        }

        private void 修改装备IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ChangedData chang = new ChangedData(db, this, Check_PCID, listView4.SelectedItems[0].Tag.ToString(), listView4.SelectedItems[0].SubItems[3].Text, "data_id", Data);
                if (DialogResult.OK == chang.ShowDialog())
                {
                    listView4.SelectedItems[0].SubItems[3].Text = chang.ReturnValue1.ToString();
                    Task.Run(() =>
                    {
                        msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                    });
                }

            }
        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (u_data == null)
            {
                MessageBox.Show("先加载账号哦");
                return;
            }
            string sqlstr = string.Empty;
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        sqlstr += $@"
begin tran
begin try 
";
                        for (int i = 0; i < u_data.Count; i++)
                        {
                            sqlstr += $@"
update [PlatformAcctDb].dbo.[Users] set [UserName] = N'{u_data[i].UserAcction.Replace("@ncsoft.com", "")}' where [LoginName] = N'{u_data[i].UserAcction}';
";
                        }
                        sqlstr += $@"
end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                        int count = db._SQL_IDU(sqlstr);

                        msg = $"已完成操作！受影响数据共:{count}条！";


                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void button36_Click(object sender, EventArgs e)
        {
            try
            {
               string sqlstr = $@"
begin tran
begin try  
update [BlGame01].dbo.[WeaponProperty] set [data_id] = '{textBox18.Text}' where [data_id] = '{textBox19.Text}';
update [BlGame01].dbo.[AccessoryProperty] set [data_id] = '{textBox18.Text}' where [data_id] = '{textBox19.Text}';
update [BlGame01].dbo.[GroceryProperty] set [data_id] = '{textBox18.Text}' where [data_id] = '{textBox19.Text}';
update [BlGame01].dbo.[CostumeProperty] set [data_id] = '{textBox18.Text}' where [data_id] = '{textBox19.Text}';
update [BlGame01].dbo.[GemProperty] set [data_id] = '{textBox18.Text}' where [data_id] = '{textBox19.Text}';
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                Task.Run(() =>
                {

                    if (db.IsConnection)
                    {
                        int count = db._SQL_IDU(sqlstr);
                        msg = $"已完成操作！受影响数据共:{count}条！";
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改时发生错误！已撤销修改。错误信息:{ex.Message}", "无法执行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox19.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label35.Text = $@"{name[0].Name}";
                }
                else
                {
                    label35.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox18.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label36.Text = $@"{name[0].Name}";
                }
                else
                {
                    label36.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }

        private void 添加召唤兽ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Check_PCID == 0)
                {
                    MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ChangedSummName summ = new ChangedSummName();
                if (DialogResult.OK == summ.ShowDialog())
                {
                    List<string> list = new List<string>();
                    list.Add($"{summ.ReturnValue}");
                    SummProject(list, Check_PCID,"Insert");
                }
                
            }
        }

        private void 删除召唤兽ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Check_PCID == 0)
                {
                    MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SummProject(new List<string>(), Check_PCID,"Delete");
            }
        }

        private void 修改召唤兽名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count == 1)
            {
                if (!db.IsConnection)
                {
                    MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Check_PCID == 0)
                {
                    MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ChangedSummName summ = new ChangedSummName(listView5.SelectedItems[0].SubItems[11].Text.Trim());
                if (DialogResult.OK == summ.ShowDialog())
                {
                    List<string> list = new List<string>();
                    list.Add($"[Name] = N'{summ.ReturnValue}'");
                    SummProject(list, Check_PCID, "Update");
                }

            }
        }

        private void 修改幻化IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ChangedData chang = new ChangedData(db, this, Check_PCID, listView4.SelectedItems[0].Tag.ToString(), listView4.SelectedItems[0].SubItems[6].Tag.ToString(), "hh", Data);
                if (DialogResult.OK == chang.ShowDialog())
                {
                    listView4.SelectedItems[0].SubItems[3].Text = chang.ReturnValue1.ToString();
                    Task.Run(() =>
                    {
                        msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                    });
                }

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            textBox21.Text = Tools.Get(textBox20.Text.Trim(),new Dictionary<string, string>());
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox22.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写金钱数量！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Convert.ToDouble(textBox22.Text.Trim()) > 60000)
            {
                textBox22.Text = "60000";
            }

            List<string> list = new List<string>();
            list.Add($"[duel_point] = '{textBox22.Text}'");
            Project(list, Check_PCID);
        }

        private void 转移到PCIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.CheckedItems.Count > 0)
            {
                List<data_Changed> dat = new List<data_Changed>();
                foreach (ListViewItem item in listView4.CheckedItems)
                {
                    dat.Add(new data_Changed()
                    {
                        id = item.Tag.ToString(),
                        data_id = item.SubItems[3].Text
                    }) ;
                }
                PCIDChanged chang = new PCIDChanged(db, Check_PCID, "ChangedPCID", dat);
                if (DialogResult.OK == chang.ShowDialog())
                {
                    foreach (ListViewItem item in listView4.CheckedItems)
                    {
                        listView4.Items.Remove(item);
                    }
                    Task.Run(() =>
                    {
                        msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                    });
                }
            }
            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView4.CheckedItems.Count > 0)
            {
                List<data_Changed> dat = new List<data_Changed>();
                foreach (ListViewItem item in listView4.CheckedItems)
                {
                    dat.Add(new data_Changed()
                    {
                        id = item.Tag.ToString(),
                        data_id = item.SubItems[3].Text
                    });
                }
                PCIDChanged chang = new PCIDChanged(db, Check_PCID, "NewPCID", dat);
                if (DialogResult.OK == chang.ShowDialog())
                {
                    Task.Run(() =>
                    {
                        msg = $"已完成操作！受影响数据共:{chang.ReturnValue}条！";
                    });
                }
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    string czbz = string.Empty;
                    string zcbz = string.Empty;
                    string _str_1 = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                    czbz = _str_1.Substring(_str_1.IndexOf("<AppName>VirtualCurrencySrv</AppName>"));
                    czbz = czbz.Substring(czbz.IndexOf("<Epoch>") + 7);
                    czbz = czbz.Substring(0, czbz.IndexOf("</Epoch>"));

                    zcbz = _str_1.Substring(_str_1.IndexOf("<AppName>AuthSrv</AppName>"));
                    zcbz = zcbz.Substring(zcbz.IndexOf("<Epoch>") + 7);
                    zcbz = zcbz.Substring(0, zcbz.IndexOf("</Epoch>"));
                    this.Invoke(new Action(delegate
                    {
                        textBox20.Text = zcbz;
                        textBox21.Text = czbz;
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

        }


        private void button42_Click(object sender, EventArgs e)
        {
            try
            {
                button42.Enabled = false;
                button42.Text = "操作中...";
                Task.Run(() =>
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("Acction", textBox25.Text.Trim());
                    dic.Add("NewPassword", textBox26.Text);
                    string str = Tools.Get($"http://{db.ServerIP}:8012/PasswordRest.aspx", dic);
                    this.Invoke(new Action(delegate
                    {
                        switch (str.Trim())
                        {
                            case "Success":
                                MessageBox.Show("密码修改成功辣quq", "T1温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case "ChangedError":
                                MessageBox.Show("密码修改失败了TAT 可恶 竟然失败了！再试试吧！", "T1温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                break;
                            default:
                                MessageBox.Show($"出现故障无法修改！请查阅数据！\r\n故障内容:\r\n{str}", "T1温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                        }
                        button42.Enabled = true;
                        button42.Text = "确认修改";
                    }));
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"可恶！这是什么鬼错误啊！快去截图给Sword！\r\n{ex.Message}", "T1温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox28.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label50.Text = $@"{name[0].Name}";
                }
                else
                {
                    label50.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox4.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写需要增加的经验！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            Task.Run(()=> 
            {
                this.Invoke(new Action(delegate
                {
                    label19.Text = "开始进行直升！";
                    button29_Click(null, null);
                }));
                
                Thread.Sleep(2000);
                this.Invoke(new Action(delegate
                {
                    label19.Text = "任务已完成 正在开启遁地！";
                    button28_Click(null, null);
                }));
                
                Thread.Sleep(2000);

                this.Invoke(new Action(delegate
                {
                    label19.Text = "遁地已开启 正在修改等级与资源！";
                    button17_Click(null, null);
                }));
                Thread.Sleep(2000);

                List<string> list = new List<string>();
                list.Add($"[faction] = '{1}'");
                list.Add($"[level] = '45'");
                list.Add($"[mastery_level] = '1'");
                list.Add($"[exp] = '1083090'");
                list.Add($"[money] = [money] + '50000'");
                list.Add($"[geo_zone] = '2310'");
                list.Add($"[x] = '4043'");
                list.Add($"[y] = '2441'");
                list.Add($"[z] = '20'");

                this.Invoke(new Action(delegate 
                {
                    Project(list, Check_PCID);
                }));
                

                this.Invoke(new Action(delegate
                {
                    label19.Text = "等级已直升！正在准备发送直升道具！";
                }));
                
                if (APIMode)
                {
                    Task.Run(() =>
                    {
                        List<Dictionary<string, string>> list_dic = new List<Dictionary<string, string>>();
                        Dictionary<string, string> dic = null;

                        for (int i = 0; i < 8; i++)
                        {
                            Dictionary<string, string> T = new Dictionary<string, string>();
                            T.Add("Acction", CheckAcction);
                            T.Add("Count", "1");
                            T.Add("ItemID", (818036 + i).ToString());
                            T.Add("Pwd", "BNSServerP");
                            list_dic.Add(T);
                        }

                        switch (JobStr)
                        {
                            case "剑士":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619733");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID", "618414");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "召唤师":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619743");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618424");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "气功师":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619748");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618429");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "力士":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619753");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618434");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "灵剑士":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619763");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618444");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "拳师":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","619738");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618419");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                            case "刺客":
                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","622549");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//恶女武器

                                dic = new Dictionary<string, string>();
                                dic.Add("Acction", CheckAcction);
                                dic.Add("Count", "1");
                                dic.Add("ItemID","618439");
                                dic.Add("Pwd", "BNSServerP");
                                list_dic.Add(dic);//极限妖魔武器
                                break;
                        }
                        dic = new Dictionary<string, string>();
                        dic.Add("Acction", CheckAcction);
                        dic["Count"] = "50";
                        dic.Add("ItemID", "827322");
                        dic.Add("Pwd", "BNSServerP");
                        list_dic.Add(dic);//解印符

                        dic = new Dictionary<string, string>();
                        dic.Add("Acction", CheckAcction);
                        dic["Count"] = "50";
                        dic.Add("ItemID","827320");
                        dic.Add("Pwd", "BNSServerP");
                        list_dic.Add(dic);//修理工具

                        dic = new Dictionary<string, string>();
                        dic.Add("Acction", CheckAcction);
                        dic["Count"] = "50";
                        dic.Add("ItemID","827319");
                        dic.Add("Pwd", "BNSServerP");
                        list_dic.Add(dic);//钥匙

                        dic = new Dictionary<string, string>();
                        dic.Add("Acction", CheckAcction);
                        dic["Count"] = "15";
                        dic.Add("ItemID", "823451");
                        dic.Add("Pwd", "BNSServerP");
                        list_dic.Add(dic);//火炮兰箱子

                        //
                        dic = null;//释放
                        int count = 0;
                        foreach (var item in list_dic)
                        {
                            if (Tools.Get($"http://{db.ServerIP}:8012/SendItem.aspx", item).Trim().Equals("Success"))
                            {
                                count++;
                            }
                            this.Invoke(new Action(delegate
                            {
                                label19.Text = $"正在发送第:{count}个道具...";
                            }));
                            Thread.Sleep(800);
                        }
                        if (count > 0)
                        {
                            this.Invoke(new Action(delegate
                            {
                                label19.Text = $"直升道具发送完毕 共成功:{count}个！";
                            }));
                            Thread.Sleep(3000);
                            this.Invoke(new Action(delegate
                            {
                                label19.Text = $"此页面功能 都需要在角色页面使用！";
                            }));
                        }
                    });

                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        label19.Text = "礼品发送无法完成 非API模式 请手动发送道具！";
                    }));
                    
                }
            });
            
            
        }
        private void contextMenuJobClick(object sender,EventArgs e) 
        {
            ToolStripMenuItem con = sender as ToolStripMenuItem;
            Keyjob(con.Text);
        }

        private void Keyjob(string _JobString) 
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Check_PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string dat = string.Empty;
            int job = 0;
            switch (_JobString)
            {
                case "剑士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0070004020000000008F0FFFF698D099307C470E23FE1BF7A9671777EC0FF060F20FCF1FFFFFFFF3BC0FEFEBFF60200FFDFFFFFFFFFFFFDFF7E0000000800000000C0FFDE0FFE9F371C00000000F7DCFFFF7F8F0BF0FF7FFF04EC4100A0FF030000FFFBFFFF97070000C0EFFBFFB7FF0083FEFECFB9AA00F0FFFF7F00000000D4F3FEFF3B0000F0070200002000000000B03B";
                    job = 1;
                    break;
                case "召唤":
                    dat = "0x0000000000000000000000000000000000000000000000000000007000007C020000000008F0FFFF698D091305C4704239C1BF3A16417760420D040F00FCF1FFFFFFFF3BC0FFFE3F82000094FAFFFFFFFFFF7EFF6E0000000800000000C0FFDE0FFE9F371C00000000F7DCFB7F7F8C0BF0FF7FFF04A8010020FF030000FFFBFFFF97070000C0EDFBFF37FE0083FEFEDFF9AB00F0FFFF7F00000000D4B7FE7F38000000040200803F00000000B03F";
                    job = 6;
                    break;
                case "气功":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040604020000000008F0FFFF698D099307E470F23FE1BF4AD671777EC2FE060F00FCF1FFFFFFFF3BC0FEFFBFDE0200BDDEFFFFFFFFFF7EBD5C0000000800000000C0FFDC0FFC9B351C00000000F7DCFBFF7F8C03F0FF7FFF04E8410020FF030000FFFBFF7F97070000C0EDFBFF37FE0083EE78CFF9AA00F0FFF77F00000000D4B3025F1900000004027E002000000000B03F";
                    job = 3;
                    break;
                case "力士":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0C40004020000000008F0FFFF698D099307E470F23FE1BF4AD671677EC0FF060F20FCF1FFFFFFFF3BC0FFFFBFFE0300BDFEFFFFDFFFFF7EFF7E0000000800000000C0FFDE0FFC9D170800000000E7C4FE3F7B0C0BF0FF7FFF04E84100A0FF030000FFFBFF7F15000000C0EDFBFF37F600824CFB8011A800D0BF7F0300000000D4B2025F1B00000004FC00002060000000B03D";
                    job = 5;
                    break;
                case "灵剑":
                    dat = "0x000000000000000000000000000000000000000000000000000000F00400041A0000000008F0FFFF690400000420004203002A72044177120212020E00FCF1FFFFFFFF3BC07F91030A000084420100E0D9A66A09300000000800000000C0190800D0810018000000008094FF4B068C01F0FF7FFF04800100201F00000097A1FB7716000000C08DFB8337F60083FC5001B08A00F0FE9760000000001095021F1900000004020000A01F000000B03B";
                    job = 8;
                    break;
                case "拳师":
                    dat = "0x000000000000000000000000000000000000000000000000000000F01C0004020000000008F0FFFF698D099307E070F23FE1BF7AD671777EC0FF060F00FCF1FFFFFFFF3BC0FFFFBF8E000072FDFFFFFFFFFFFFFC7D0000000800000000C0FFDE0AFE97351C00000000A01C0103000300F0FF7FFF04D4410020FF030000FFFBFF7F97070000C0C9FBFF37F80003EEF9D7E9A800F0FFFF7F00000000D437FAE008000000FC0300002000000000B03B00000008";
                    job = 2;
                    break;
                case "刺客":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0043004020000000000F0FFFF08810092040000020160A20286415712C0FF020E00FCF1FFFFFFFF3B406211004A010042C18046E121C0C742210000000800000000C0094808D080001800000000C5D4F97A7F8309F0FF7FFF045C0100A0FF00000065A06B2185070000C0C2430930E0008302E886912B0050A3160000000000D473FFFF390000000402003F2000000000B03F";
                    job = 7;
                    break;
                case "咒术":
                    dat = "0x000000000000000000000000000000000000000000000000000000F0040004020000000008F0FFFF68890993070440023B00B77A946147000208000E00FCF1FFFFFFFF3BC0283E001000007B7D9BF8C7FB7EC88C5000000008000000000060140ACC19140000000000C684FA3B020303F0FF7FFF0404000020020000009FFBFD8787070000C0EAF3FD87F100834A6A86912A00F0BF5E0700807F00D476FA401A000000040200002000000000B03F";
                    job = 9;
                    break;
                case "气宗":
                    dat = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                    job = 10;
                    break;
            }
            if (job == 0)
            {
                //无职业选择
                return;
            }
            string sqlstr = $@"
begin tran
begin try  
UPDATE [BlGame01].dbo.[QuestList] SET [flags] = {dat} WHERE [pcid] = '{Check_PCID}';
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{Check_PCID}';

insert into [BlGame01].dbo.[QuestProperty] 
([pcid],[data_id],[reward_multiplication_factor],[current_mission_variation],
[current_mission_step],[mission_completion_1],
[mission_achievement_value_1],[mission_completion_2],
[mission_achievement_value_2],[mission_completion_3],
[mission_achievement_value_3],[mission_completion_4],
[mission_achievement_value_4],[mission_completion_5],
[mission_achievement_value_5],[mission_completion_6],
[mission_achievement_value_6],[mission_completion_7],
[mission_achievement_value_7],[mission_completion_8],
[mission_achievement_value_8],[mission_completion_9],
[mission_achievement_value_9],[mission_completion_10],
[mission_achievement_value_10],[mission_completion_11],
[mission_achievement_value_11],[mission_completion_12],
[mission_achievement_value_12],[mission_completion_13],
[mission_achievement_value_13],[mission_completion_14],
[mission_achievement_value_14],[mission_completion_15],
[mission_achievement_value_15],[mission_completion_16],
[mission_achievement_value_16])
values(
'{Check_PCID}','{1420}','1','1','1'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0');
update [LobbyDB].dbo.[Character] set [job] = '{job}' where [pcid] = '{Check_PCID}';
update [BlGame01].dbo.[CreatureProperty] set [job] = '{job}' where [pcid] = '{Check_PCID}';
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {

                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "操作成功！";
                        }
                        else
                        {
                            msg = "操作失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }
        private void listView5_ColumnClick(object sender, ColumnClickEventArgs e)
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

        private string GetKey(string ItemID)
        {
            List<string> ls1 = new List<string>();
            ls1.Add("A");
            ls1.Add("B");
            ls1.Add("C");
            ls1.Add("D");
            ls1.Add("E");
            ls1.Add("F");
            ls1.Add("G");
            ls1.Add("H");
            ls1.Add("I");
            ls1.Add("J");
            ls1.Add("K");
            ls1.Add("L");
            ls1.Add("M");
            ls1.Add("N");
            ls1.Add("O");
            ls1.Add("P");
            ls1.Add("Q");
            ls1.Add("R");
            ls1.Add("S");
            ls1.Add("T");
            ls1.Add("U");
            ls1.Add("V");
            ls1.Add("W");
            ls1.Add("X");
            ls1.Add("Y");
            ls1.Add("Z");

            ls1.Add("a");
            ls1.Add("b");
            ls1.Add("c");
            ls1.Add("d");
            ls1.Add("e");
            ls1.Add("f");
            ls1.Add("g");
            ls1.Add("h");
            ls1.Add("i");
            ls1.Add("j");
            ls1.Add("k");
            ls1.Add("l");
            ls1.Add("m");
            ls1.Add("n");
            ls1.Add("o");
            ls1.Add("p");
            ls1.Add("q");
            ls1.Add("r");
            ls1.Add("s");
            ls1.Add("t");
            ls1.Add("u");
            ls1.Add("v");
            ls1.Add("w");
            ls1.Add("x");
            ls1.Add("y");
            ls1.Add("z");

            ls1.Add("0");
            ls1.Add("1");
            ls1.Add("2");
            ls1.Add("3");
            ls1.Add("4");
            ls1.Add("5");
            ls1.Add("6");
            ls1.Add("7");
            ls1.Add("8");
            ls1.Add("9");

            ls1.Add("+");
            ls1.Add("/");

            List<string> ends = new List<string>();
            int v = int.Parse(ItemID) * 16;
            while (v >= 64)
            {
                int idx = (int)((float)v % 64);
                ends.Add(ls1[idx]);
                v = (int)(Math.Floor((float)v / 64f));
            }
            ends.Add(ls1[v]);
            string _mids = string.Empty;
            for (int i = ends.Count - 1; i >= 0; i--)
            {
                _mids += ends[i];
            }
            string _Code = "AA" + _mids + "==";
            return _Code;
        }

        private void 删除此奖励ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            if (listView8.CheckedItems.Count > 0)
            {
                if (DialogResult.Yes == MessageBox.Show($@"确认删除选中的{listView8.CheckedItems.Count}项？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    string byid = "";
                    string GoodsID = "";
                    foreach (ListViewItem item in listView8.CheckedItems)
                    {
                        byid += item.Tag;
                        GoodsID += item.SubItems[1].Text;
                        if (item != listView8.CheckedItems[listView8.CheckedItems.Count -1])
                        {
                            byid += ",";
                            GoodsID += ",";
                        }
                    }
                    string sqlstr = $@"
begin tran
begin try  
delete [PromotionStampDb].dbo.[UserPromotionRewards] where [RewardPolicyId] in ({byid});
delete [PromotionStampDb].dbo.[RewardPolicies] where [RewardPolicyId] in ({byid});
delete [GoodsDB].dbo.[GoodsCategories] where [GoodsId] in ({GoodsID}) and [CategoryId] = '71' and [DisplayOrder] = '1'
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                    Task.Run(() =>
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            this.Invoke(new Action(delegate 
                            {
                                foreach (ListViewItem item in listView8.CheckedItems)
                                {
                                    listView8.Items.Remove(item);
                                }
                            }));
                            
                            msg = "提交成功！";
                        }
                        else
                        {
                            msg = "提交失败！";
                        }
                    });
                }
                
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            string ItemKey = GetKey(textBox30.Text.Trim());
            string Count = textBox34.Text.Trim();
            button44.Enabled = false;
            Task.Run(() => 
            {
                string sqlstr = $@"
select DISTINCT a.[GoodsId],a.[ItemId],b.[GoodsName],c.[ItemName],a.[ItemQuantity],d.[BasicSalePrice],e.[GameItemKey] from [GoodsDb].dbo.[GoodsItems] as a 
left join [GoodsDb].dbo.[Goods] as b on a.[GoodsId] = b.[GoodsId ]
left join [GoodsDb].dbo.[Items] as c on a.[ItemId] = c.[ItemId]
left join [GoodsDb].dbo.[GoodsItemBasicPrices] as d on a.[GoodsId] = d.[GoodsId]
left join [GoodsDb].dbo.[GameItems] as e on a.[ItemId] = e.[ItemId]
where e.[GameItemKey] = '{ItemKey}' and a.[GoodsId] = a.[ItemId] and a.[ItemQuantity] = '{Count}'";
                DataItems_Entity Data = null;
                using (SqlDataReader sdr = db._SQLRead(sqlstr))
                {
                    if (sdr.Read())
                    {
                        Data = new DataItems_Entity()
                        {
                            GoodsId = Convert.ToInt32(sdr["GoodsId"]),
                            ItemId = Convert.ToInt32(sdr["ItemId"]),
                            GoodsName = sdr["GoodsName"].ToString(),
                            ItemName = sdr["ItemName"].ToString(),
                        };
                    }
                }
                if (Data == null) // 无 开始新增
                {
#region 获取最新ID
                    string sql = $"select top 1 [GoodsId],[ItemId] from [GoodsDb].dbo.[GoodsItems] where GoodsId>=15000 and GoodsId<50000 order by GoodsId desc";
                    int ID = Convert.ToInt32(db._SQLScalar(sql));
                    ID++;//自增1

#region ItemsID
                    sql = $"Select [ItemId] from [GoodsDb].dbo.[Items] where [ItemId]='{ID}'";
                    int ItemId = Convert.ToInt32(db._SQLScalar(sql) == DBNull.Value ? 0 : db._SQLScalar(sql));
                    if (ItemId == 0)
                    {
                        ItemId = ID;
                        sql = $@"
begin tran
begin try
insert into [GoodsDb].dbo.[Items] (ItemId,ItemName,ItemAppGroupCode,ItemType,IsConsumable,BasicPrice,BasicCurrencyGroupId,Changed,ChangerAdminAccount,ItemDescription) values ({ItemId},N'签到转盘活动','bnsgrnTH',3,0,0,69,GetDate(),'TestAdminAccount',N'签到转盘活动');
insert into [GoodsDb].dbo.[GameItems] (ItemId,GameItemKey,GameItemData) values ({ItemId},'{ItemKey}','AAAAAAAAAAA=');
insert into [GoodsDb].dbo.[ItemDisplay] (ItemId,LanguageCode,ItemDisplayName,ItemDisplayDescription) values ({ItemId},11,N'签到转盘活动',N'签到转盘活动');
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
                    ";
                        int ItemInsertCount = db._SQL_IDU(sql);

                        if (ItemInsertCount < 3)
                        {
                            this.Invoke(new Action(delegate
                            {

                                MessageBox.Show("Items添加失败！请手动排查原因", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button44.Enabled = true;
                                return;
                            }));


                        }
                    }

#endregion

#region GoodsID
                    sql = $"Select [GoodsId] from [GoodsDb].dbo.[Goods] where [GoodsId]='{ID}'";
                    int GoodsId = Convert.ToInt32(db._SQLScalar(sql) == DBNull.Value ? 0 : db._SQLScalar(sql));
                    if (GoodsId == 0)
                    {
                        GoodsId = ID;
                        sql = $@"
            begin tran
begin try

insert into [GoodsDb].dbo.[Goods ](GoodsId,GoodsName,GoodsAppGroupCode,GoodsType,DeliveryType,SaleStatus,EffectiveFrom,EffectiveTo,SaleableQuantity,RefundUnitCode,IsRefundable,IsAvailableRecurringPayment,Changed,ChangerAdminAccount,GoodsDescription,GoodsData,GoodsPurchaseType,GoodsPurchaseCheckMask) VALUES ({GoodsId},N'签到转盘活动','bnsgrnTH',3,1,2,'2017-09-05 00:00:00','2099-12-31 23:59:59',0,1,0,1,GETDATE(),'TestAdminAccount',N'签到转盘活动','AAAAAAE=',1,0);

insert into [GoodsDb].dbo.[GoodsDisplay] (GoodsId,LanguageCode,GoodsDisplayName,GoodsDisplayDescription) values ({GoodsId},11,N'系统发送道具',N'签到转盘活动');

insert into [GoodsDb].dbo.[GoodsBasicPrices] (GoodsId,CurrencyGroupId,BasicSalePrice,RefundFee) values ({GoodsId},71,0,0);

insert into [GoodsDb].dbo.[GoodsSalePricePolicies] (GoodsId,CurrencyGroupId,PricePolicyType,EffectiveFrom,EffectiveTo,SalePrice) values ({GoodsId},71,1,'2017-06-06 00:00:00','2099-12-31 23:59:59',9999);

insert into [GoodsDb].dbo.[GoodsChanges] (ChangeId,ChangeType,Registered,RegistrarAdminAccount,GoodsAppGroupCode,IsDisplayable) values ({GoodsId},2,GETDATE(),'TestAdminAccount','bnsgrnTH',1);

insert into [GoodsDb].dbo.[GoodsItems] (GoodsId,ItemId,ItemQuantity,ItemExpirationType,ItemData,DeliveryPriority) values ({GoodsId},{ItemId},'{Count}',0,'AAAAAAEA',1);

insert into [GoodsDb].dbo.[GoodsItemBasicPrices] (GoodsId,ItemId,CurrencyGroupId,BasicSalePrice) values ({GoodsId},{ItemId},71,9999)
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
                    ";
                        int GoodsInsertCount = db._SQL_IDU(sql);

                        if (GoodsInsertCount < 7)
                        {
                            this.Invoke(new Action(delegate
                            {

                                MessageBox.Show("Goods添加失败！请手动排查原因", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button44.Enabled = true;
                                return;
                            }));
                        }
                    }
#endregion

                    this.Invoke(new Action(delegate 
                    {
                        textBox31.Text = GoodsId.ToString();
                        textBox32.Text = ItemId.ToString();
                        button44.Enabled = true;
                    }));
#endregion
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        textBox31.Text = Data.GoodsId.ToString();
                        textBox32.Text = Data.ItemId.ToString();
                        button44.Enabled = true;
                    }));
                }

            });

            
        }


        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox30.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label55.Text = $@"{name[0].Name}";
                }
                else
                {
                    label55.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }



        private void button50_Click(object sender, EventArgs e)
        {
            try
            {
                
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = MD5Encrypt(Guid.NewGuid() + DateTime.Now.ToString("yyyyMMddHHmmss"));
                lvi.Text = (listView11.Items.Count + 1).ToString();
                lvi.SubItems.Add(textBox42.Text.Trim() == "" ? "0" : textBox42.Text.Trim());
                lvi.SubItems.Add(textBox43.Text.Trim() == "" ? "0" : textBox43.Text.Trim());
                lvi.SubItems.Add(label71.Text.Trim());
                lvi.SubItems.Add(textBox47.Text.Trim() == "" ? "0" : textBox47.Text.Trim());
                listView11.Items.Add(lvi);
            }
            catch
            {

            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            try
            {

                ListViewItem lvi = new ListViewItem();
                lvi.Tag = MD5Encrypt(Guid.NewGuid()+DateTime.Now.ToString("yyyyMMddHHmmss"));
                lvi.Text = (listView10.Items.Count + 1).ToString();
                lvi.SubItems.Add(textBox39.Text.Trim() == "" ? "0" : textBox39.Text.Trim());
                lvi.SubItems.Add(textBox40.Text.Trim() == "" ? "0" : textBox40.Text.Trim());
                lvi.SubItems.Add(label67.Text.Trim());
                lvi.SubItems.Add(textBox49.Text.Trim() == "" ? "0" : textBox49.Text.Trim());
                lvi.SubItems.Add(textBox44.Text.Trim() == "" ? "0" : textBox44.Text.Trim());
                listView10.Items.Add(lvi);
            }
            catch
            {

            }
        }


        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox42.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label71.Text = $@"{name[0].Name}";
                }
                else
                {
                    label71.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }

        private void textBox39_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox39.Text);
                List<dj_item> name = Data.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label67.Text = $@"{name[0].Name}";
                }
                else
                {
                    label67.Text = $@"未找到道具名";
                }

            }
            catch
            {

            }
        }

        private void listView11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView11.SelectedItems.Count == 1)
            {
                textBox42.Text = listView11.SelectedItems[0].SubItems[1].Text.Trim();
                textBox43.Text = listView11.SelectedItems[0].SubItems[2].Text.Trim();
                textBox47.Text = listView11.SelectedItems[0].SubItems[3].Text.Trim();
            }
        }

        private void listView9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView9.SelectedItems.Count == 1)
            {
                textBox37.Text = listView9.SelectedItems[0].SubItems[1].Text.Trim();
                textBox38.Text = listView9.SelectedItems[0].SubItems[2].Text.Trim();
                textBox41.Text = listView9.SelectedItems[0].SubItems[3].Text.Trim();
            }
        }

        private void listView10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView10.SelectedItems.Count == 1)
            {
                textBox39.Text = listView10.SelectedItems[0].SubItems[1].Text.Trim();
                textBox40.Text = listView10.SelectedItems[0].SubItems[2].Text.Trim();
                textBox49.Text = listView10.SelectedItems[0].SubItems[4].Text.Trim();
                textBox44.Text = listView10.SelectedItems[0].SubItems[5].Text.Trim();
            }
        }


        private void 删除ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listView11.SelectedItems.Count > 0) 
            {
                listView11.Items.Remove(listView11.SelectedItems[0]);
            }
        }

        private void 删除ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listView10.SelectedItems.Count > 0)
            {
                listView10.Items.Remove(listView10.SelectedItems[0]);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            LockProjcet = this.checkBox3.Checked;
        }

        private void textBox44_TextChanged(object sender, EventArgs e)
        {
            if (textBox39.Text == "0" || textBox39.Text.Trim() == "")
            {
                label67.Text = $"{textBox44.Text.Trim()}积分";
            }
        }

        private void button51_Click(object sender, EventArgs e)
        {
            if (!db.IsConnection)
            {
                MessageBox.Show("请等待数据库连接完成！", "无法继续！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (u_ID.Equals(""))
            {
                MessageBox.Show("请先进行加载！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
            begin tran
begin try
delete [WarehouseDb_bnsgrnTH].dbo.[GoodsWarehouseItems] where [GoodsWarehouseId] in (select [GoodsWarehouseId] from [WarehouseDb_bnsgrnTH].dbo.[GoodsWarehouses] where [userid] = '{u_ID}');
delete [WarehouseDb_bnsgrnTH].dbo.[EnumeratedGoodsWarehouses] where [GoodsWarehouseId] in (select [GoodsWarehouseId] from [WarehouseDb_bnsgrnTH].dbo.[GoodsWarehouses] where [userid] = '{u_ID}');
delete [WarehouseDb_bnsgrnTH].dbo.[GoodsWarehouses] where [userid] = '{u_ID}';

delete [GameWarehouseDB].dbo.[WarehouseItemArchive] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseItem] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseGoodsArchive] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseGoods] where [OwnerAccountID] = '{u_ID}';
delete [GameWarehouseDB].dbo.[WarehouseStatistics]
delete [GameWarehouseDB].dbo.[ErrorLog]

--delete [GradeMembersDb].dbo.[Members] where [UserId] = '{u_ID}';--不清理会员


delete [PurchaseDb].dbo.[GoodsDeliveryRequests] where [SenderUserId] = '{u_ID}';

delete [PurchaseDb].dbo.[PurchaseGoods] where [PurchaseId] in( select [PurchaseId] from [PurchaseDb].dbo.[Purchases] where [UserId] = '{u_ID}');
delete [PurchaseDb].dbo.[PurchasePayment] where [PurchaseId] in( select [PurchaseId] from [PurchaseDb].dbo.[Purchases] where [UserId] = '{u_ID}');

delete [PurchaseDb].dbo.[Purchases] where [UserId] = '{u_ID}';
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
";
            Task.Run(() =>
            {
                try
                {
                    if (db.IsConnection)
                    {
                        if (db._SQL_IDU(sqlstr) > 0)
                        {
                            //操作成功
                            msg = "清理成功！";
                        }
                        else
                        {
                            msg = "清理失败！";
                        }

                    }
                    else
                    {
                        MessageBox.Show($"请先连接数据库！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现未知错误 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            });
        }

        private void label22_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();//清空剪切板内容

                Clipboard.SetData(DataFormats.Text, u_ID.ToUpperInvariant());//复制内容到剪切板
            }
            catch
            {

            }
        }

        private void 批量复制IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                //ID_TB.Text = mList[listView1.SelectedIndices[0]].Tag.ToString();
                string key = string.Empty;
                foreach (int item in listView1.SelectedIndices)
                {
                    key += $"{mList[item].Tag},";
                }
                key = key.Substring(0, key.Length - 1);
                Clipboard.Clear();//清空剪切板内容

                Clipboard.SetData(DataFormats.Text, key);//复制内容到剪切板

            }
        }

    }
}
