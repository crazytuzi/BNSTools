using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserPropertyChanged
{
    public partial class Form1 : Form
    {
        string sqlstr;

        bool sqlOpen;

        SqlConnection conn;

        int PCID = 0;
        List<data_code> data_ids;

        string u_ID;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("数据库IP不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("数据库账号不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("数据库密码不能为空！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!sqlOpen)//未连接
            {
                string ServerIP = textBox1.Text.Trim();
                string Acction = textBox2.Text.Trim();
                string Password = textBox3.Text;
                sqlstr = $"server={ServerIP};database=VirtualCurrencyDb;uid={Acction};pwd={Password}";
                Task.Run(() =>
                {
                    try
                    {
                        this.Invoke(new Action(delegate
                        {
                            button3.Text = "连接中";
                            button3.Enabled = false;
                        }));
                        conn = new SqlConnection(sqlstr);
                        conn.Open();
                        sqlOpen = true;
                        this.Invoke(new Action(delegate
                        {
                            button3.Text = "断开连接";
                        }));

                        UpdateAppConfig("ServerIP", ServerIP);
                        UpdateAppConfig("Acction", Acction);
                        UpdateAppConfig("Password", Password);
                    }
                    catch (Exception ex)
                    {
                        sqlOpen = false;
                        MessageBox.Show($"数据库连接异常 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    finally
                    {
                        this.Invoke(new Action(delegate
                        {
                            if (!sqlOpen)
                            {
                                button3.Text = "连接数据库";
                            }
                            button3.Enabled = true;
                        }));
                    }
                });
            }
            else
            {
                Task.Run(() =>
                {
                    try
                    {
                        this.Invoke(new Action(delegate
                        {
                            button3.Text = "正在断开";
                            button3.Enabled = false;
                        }));
                        conn.Close();
                        sqlOpen = false;
                        this.Invoke(new Action(delegate
                        {
                            button3.Text = "连接数据库";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(delegate
                        {
                            button3.Text = "断开连接";
                        }));
                        MessageBox.Show($"数据库断开时出现异常 错误内容:{ex.Message}", "出现未知异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    finally
                    {
                        button3.Enabled = true;
                    }
                });
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

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox7.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写游戏账号！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sqlstr = $@"
select cr.[level],cr.name,cr.[mastery_level],rw.[data_id],rw.[current_mission_step],js.PCID from [BlGame01].dbo.[CreatureProperty] as cr inner join [BlGame01].dbo.QuestProperty as rw inner join [LobbyDB].dbo.[Character] as js inner join [PlatformAcctDb].dbo.[Users] as us on js.GameAccountID = us.UserId on rw.pcid = js.PCID on cr.pcid = js.PCID where [CharacterState] = 2 and us.[UserName] = '{textBox7.Text.Trim()}'";
            List<UserData> us_data = new List<UserData>();
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {
                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                UserData T = new UserData()
                                {
                                    data_id = sdr["data_id"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["data_id"]),
                                    Level = sdr["level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["level"]),
                                    master_level = sdr["mastery_level"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["mastery_level"]),
                                    Name = sdr["name"].ToString(),
                                    PCID = sdr["PCID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["PCID"]),
                                    current_mission_step = sdr["current_mission_step"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["current_mission_step"]),
                                };
                                if (T.PCID != 0)
                                {
                                    us_data.Add(T);
                                }
                            }
                        }
                        this.Invoke(new Action(() => 
                        {
                            listView1.Items.Clear();
                            for (int i = 0; i < us_data.Count; i++)
                            {
                                ListViewItem lvi = new ListViewItem();
                                lvi.Text = (i + 1).ToString();
                                lvi.SubItems.Add(us_data[i].data_id.ToString());
                                lvi.SubItems.Add(us_data[i].current_mission_step.ToString());
                                lvi.SubItems.Add(us_data[i].Name);
                                lvi.SubItems.Add(us_data[i].Level.ToString());
                                lvi.SubItems.Add(us_data[i].master_level.ToString());
                                lvi.SubItems.Add(us_data[i].PCID.ToString());

                                listView1.Items.Add(lvi);
                            }

                        }));

                        string sql = $"select [UserId] from [PlatformAcctDb].dbo.[Users] where [UserName] = '{textBox7.Text.Trim()}'";

                        SqlCommand com = new SqlCommand(sql,conn);
                        u_ID = com.ExecuteScalar().ToString();

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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) 
            {
                label4.Text = $"角色名:未选择角色";
                label4.ForeColor = Color.Red;
            }
            else
            {
                PCID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[6].Text);
                label4.Text = $"角色名:{listView1.SelectedItems[0].SubItems[3].Text}";
                label4.ForeColor = Color.DodgerBlue;
            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox4.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写等级！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (textBox5.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写洪门等级！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "";
            string sqlstr = $@"update [BlGame01].dbo.[CreatureProperty] set [level] = '{textBox4.Text.Trim()}',[mastery_level] = '{textBox5.Text.Trim()}' where [pcid] = '{PCID}'
";
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        if (cmd.ExecuteNonQuery() > 0)
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text == "" || Convert.ToInt32(textBox4.Text) < 55)
            {
                textBox5.Text = "0";
            }
        }


        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            sqlstr = "";
            sqlOpen = false;
            textBox1.Text = GetAppConfig("ServerIP") == null ? textBox1.Text : GetAppConfig("ServerIP");
            textBox2.Text = GetAppConfig("Acction") == null ? textBox2.Text : GetAppConfig("Acction");
            textBox3.Text = GetAppConfig("Password") == null ? textBox3.Text : GetAppConfig("Password");

            DataTable dt = new DataTable();
            dt.TableName = "dt";
            dt.Columns.Add("Code");
            dt.Columns.Add("Name");

            DataRow dr1 = dt.NewRow();
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                textBox8.Text = this.comboBox1.SelectedValue.ToString();
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "";
            string sqlstr = $@"
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{PCID}'
";
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        if (cmd.ExecuteNonQuery() > 0)
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "";
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
'{PCID}','{textBox8.Text.Trim()}','1','1','1'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0')

";
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        if (cmd.ExecuteNonQuery() > 0)
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {

            }
            else
            {
                PCID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[6].Text);
                int Data_ID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                string msg = "";
                string sqlstr = $@"
delete from [BlGame01].dbo.[QuestProperty]  where [pcid] = '{PCID}' and [data_id] = '{Data_ID}'";
                Task.Run(() =>
                {
                    try
                    {
                        if (sqlOpen)
                        {

                            SqlCommand cmd = new SqlCommand(sqlstr, conn);
                            if (cmd.ExecuteNonQuery() > 0)
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
                    finally
                    {
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = msg;
                        }));
                        Thread.Sleep(3000);
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = "";
                        }));
                    }
                });
            }
        }

        private void 完成度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {

            }
            else
            {
                PCID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[6].Text);
                int Data_ID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                string msg = "";
                string sqlstr = $@"
update [BlGame01].dbo.[QuestProperty] set [current_mission_step] = [current_mission_step]+1  where [pcid] = '{PCID}' and [data_id] = '{Data_ID}'";
                Task.Run(() =>
                {
                    try
                    {
                        if (sqlOpen)
                        {

                            SqlCommand cmd = new SqlCommand(sqlstr, conn);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                //操作成功
                                msg = "操作成功！";
                                this.Invoke(new Action(delegate
                                {
                                    listView1.SelectedItems[0].SubItems[2].Text = (Convert.ToInt32(listView1.SelectedItems[0].SubItems[2].Text) + 1).ToString();
                                }));
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
                    finally
                    {
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = msg;
                        }));
                        Thread.Sleep(3000);
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = "";
                        }));
                    }
                });
            }
        }

        private void 完成度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {

            }
            else
            {
                PCID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[6].Text);
                int Data_ID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                string msg = "";
                string sqlstr = $@"
update [BlGame01].dbo.[QuestProperty] set [current_mission_step] = [current_mission_step]-1  where [pcid] = '{PCID}' and [data_id] = '{Data_ID}'";
                Task.Run(() =>
                {
                    try
                    {
                        if (sqlOpen)
                        {

                            SqlCommand cmd = new SqlCommand(sqlstr, conn);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                //操作成功
                                msg = "操作成功！";
                                this.Invoke(new Action(delegate
                                {
                                    listView1.SelectedItems[0].SubItems[2].Text = (Convert.ToInt32(listView1.SelectedItems[0].SubItems[2].Text) - 1).ToString();
                                }));
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
                    finally
                    {
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = msg;
                        }));
                        Thread.Sleep(3000);
                        this.Invoke(new Action(delegate
                        {
                            Status.Text = "";
                        }));
                    }
                });
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (PCID == 0)
            {
                MessageBox.Show("请先选择角色！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "";
            List<string> sql = new List<string>();
            foreach (var item in data_ids)
            {
                sql.Add($@"delete from[BlGame01].dbo.[QuestProperty]  where[pcid] = '{PCID}' and[data_id] = '{item.data_id}'");
                sql.Add($@"
insert into[BlGame01].dbo.[QuestProperty]
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
'{PCID}','{item.data_id}','1','1','{item.current_mission_step}'
,'false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0','false','0')");
            }
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        int count = 0;
                        foreach (var item in sql)
                        {
                            SqlCommand cmd = new SqlCommand(item, conn);
                            count +=cmd.ExecuteNonQuery();
                        }
                        if (count > 0)
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (PCID == 0)
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
            string msg = "";
            string sqlstr = $@"update [BlGame01].dbo.[CreatureProperty] set [money] = '{money}' where [pcid] = '{PCID}'";
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        if (cmd.ExecuteNonQuery() > 0)
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (u_ID == "")
            {
                MessageBox.Show("未获取到账号信息！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox9.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写点券数量！", "执行失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int money = Convert.ToInt32(textBox9.Text.Trim());
            string msg = "";
            string sqlstr = $@"exec dbo.p_AddDeposit '{u_ID}','51','5','{textBox9.Text.Trim()}','{textBox9.Text.Trim()}','0','1900-01-01T00:00:00+00:00','2999-12-30 03:30:30.000 +09:00','99','{DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss")}',N'GM工具充值','0','0'";
            Task.Run(() =>
            {
                try
                {
                    if (sqlOpen)
                    {

                        SqlCommand cmd = new SqlCommand(sqlstr, conn);
                        cmd.ExecuteNonQuery();
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
                finally
                {
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = msg;
                    }));
                    Thread.Sleep(3000);
                    this.Invoke(new Action(delegate
                    {
                        Status.Text = "";
                    }));
                }
            });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string AcctionID = "6B01F387-9015-4DA0-8D36-742975FD8A3F";
            string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
            string url = $"http://127.0.0.1:6605/spawned/VirtualCurrencySrv.1.616860619/test/command_console";
            string Request = string.Empty;
            Request += $@"
<Request>
  <CurrencyId>51</CurrencyId>
  <Amount>{1}</Amount>
  <EffectiveTo>2099-05-05T03:30:30+09:00</EffectiveTo>
  <IsRefundable>0</IsRefundable>
  <DepositReasonCode>5</DepositReasonCode>
  <DepositReason>Post在线充值</DepositReason>
  <RequestCode>99</RequestCode>
  <RequestId>{MD5Encrypt(AcctionID + tim)}</RequestId>
</Request>";
            Request = System.Web.HttpUtility.UrlEncode(Request, System.Text.Encoding.UTF8);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("protocol", "VirtualCurrency");
            dic.Add("command", "Deposit");
            dic.Add("to", $"{AcctionID}");
            dic.Add("from", "");
            dic.Add("message", $"{Request}");
            Post(url,dic);

        }
        public static string Post(string url, Dictionary<string, string> dic)
        {
            int i = 0;
            StringBuilder param = new StringBuilder();
            foreach (var item in dic)
            {
                if (i > 0)
                    param.Append("&");
                param.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] postD = Encoding.UTF8.GetBytes(param.ToString());
            string slt = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postD, 0, postD.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            slt = sr.ReadToEnd();
            return slt;
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
    }
}
