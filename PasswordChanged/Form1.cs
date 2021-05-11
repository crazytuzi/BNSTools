using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordChanged
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text != textBox4.Text)
                {
                    MessageBox.Show("两次密码不一致！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                button1.Enabled = false;
                button1.Text = "操作中...";
                Task.Run(()=> 
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("Acction", textBox1.Text.Trim());
                    dic.Add("oldPassword", textBox2.Text);
                    dic.Add("NewPassword", textBox3.Text);
                    string str = Tools.Get("http://IP:Port/PasswordChanged.aspx", dic);
                    this.Invoke(new Action(delegate 
                    {
                        switch (str.Trim())
                        {
                            case "Success":
                                MessageBox.Show("密码修改成功辣quq", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case "LoginError":
                                MessageBox.Show("那个那个..好像旧密码不对呢！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case "ChangedError":
                                MessageBox.Show("密码修改失败了TAT 可恶 竟然失败了！再试试吧！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                break;
                            default:
                                //MessageBox.Show($"出现故障或旧密码出错无法修改！\r\n故障内容:\r\n{str}", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                MessageBox.Show("那个那个..好像旧密码不对呢！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                        }
                        button1.Enabled = true;
                        button1.Text = "确认修改";
                    }));
                });
                

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"可恶！这是什么鬼错误啊！\r\n{ex.Message}", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
