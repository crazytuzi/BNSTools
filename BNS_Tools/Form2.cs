using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools
{
    public partial class Form2 : Form
    {
        DataChanged db;
        int SayType = 2;
        int SayPosition = 0;
        public Form2(DataChanged _db)
        {
            InitializeComponent();
            db = _db;
        }

        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now.AddDays(1);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            Task.Run(() => 
            {
                string sqlstr = $@"select [WorldID],[WorldName] from [LobbyDB].dbo.[GameWorld]";
                List<Word_Entity> list = new List<Word_Entity>();
                using (SqlDataReader sdr = db._SQLRead(sqlstr))
                {
                    while (sdr.Read())
                    {
                        Word_Entity entity = new Word_Entity()
                        {
                            WordID = Convert.ToInt32(sdr["WorldID"]),
                            WordName = sdr["WorldName"].ToString()
                        };
                        list.Add(entity);
                    }
                }
                this.Invoke(new Action(delegate 
                {
                    comboBox1.DataSource = null;

                    comboBox1.DisplayMember = "WordName";
                    comboBox1.ValueMember = "WordID";
                    comboBox1.DataSource = list;
                }));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Trim() != "" && textBox5.Text.Trim() != "")
            {
                
                string Start = dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:00");
                string Due = dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59");
                string DayInterval = textBox1.Text.Trim();
                string minInterval = textBox2.Text.Trim();
                string RepeatCount = textBox6.Text.Trim();
                string author = textBox3.Text.Trim();
                string title = textBox4.Text.Trim();
                string Message = textBox5.Text.Trim();

                Task.Run(() =>
                {
                    try
                    {
                        string sqlstr = $@"exec [ManagementDB].dbo.[usp_RegisterAnnounce] '0','{SayType}','{SayPosition}','{123}','{Start}','{Due}','{DayInterval}','{minInterval}','{RepeatCount}',N'{author}',N'{title}',N'{Message}'";
                        db._SQL_IDU(sqlstr);
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("操作成功!", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show($"发送异常! \r\n{ex.Message}", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    
                });
            }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            SayType = Convert.ToInt32(rb.Tag);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            SayPosition = Convert.ToInt32(rb.Tag);
        }
    }
}
