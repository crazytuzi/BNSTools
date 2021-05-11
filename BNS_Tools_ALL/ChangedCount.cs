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
    public partial class ChangedCount : Form
    {
        string id;
        int pcid;
        DataChanged db;
        Form1 man;
        int data_count;
        int count;
        public int ReturnValue { get; set; }
        public int ReturnValue1 { get; set; }

        public ChangedCount()
        {
            InitializeComponent();
        }
        public ChangedCount(DataChanged _db,Form1 _man, int _pcid,string _id,string _count) 
        {
            InitializeComponent();
            this.Text = $"Changed-Count:[{_id}]";
            db = _db;
            man = _man;
            id = _id;
            pcid = _pcid;
            this.textBox1.Text = _count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Trim() != "")
                {
                    data_count = Convert.ToInt32(textBox1.Text.Trim());
                    Task.Run(() =>
                    {
                        string sqlstr = $@"
begin tran
begin try  
update [BlGame01].dbo.[GroceryProperty] set [count] = '{data_count}' where [pcid] = '{pcid}' and [id] = '{id}'
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                        if (db.IsConnection)
                        {
                            count = db._SQL_IDU(sqlstr);
                            ReturnValue = count;
                            ReturnValue1 = data_count;
                            this.Invoke(new Action(delegate
                            {
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }));
                            
                        }
                    });
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改数量时发生错误 \r\n角色PCID:{pcid}\r\n物品ID:{id}\r\n物品Count{data_count}\r\n错误信息:{ex.Message}","无法执行",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                this.Close();
            }
            
        }
    }
}
