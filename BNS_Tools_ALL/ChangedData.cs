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
    public partial class ChangedData : Form
    {
        string id;
        int pcid;
        DataChanged db;
        Form1 man;
        int count;
        long datas;
        string typ;
        List<dj_item> dat;

        public int ReturnValue { get; set; }
        public long ReturnValue1 { get; set; }
        public ChangedData(DataChanged _db, Form1 _man, int _pcid, string _id, string _dat,string _typ,List<dj_item> _data)
        {
            InitializeComponent();
            this.Text = $"Changed:[{_id}]";
            db = _db;
            man = _man;
            id = _id;
            pcid = _pcid;
            this.textBox1.Text = _dat;
            typ = _typ;
            dat = _data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (textBox1.Text.Trim() != "")
                {
                    datas = Convert.ToInt64(textBox1.Text.Trim());
                    string sqlstr = string.Empty;
                    if (typ == "exp")
                    {
                        sqlstr = $@"
begin tran
begin try  
update [BlGame01].dbo.[WeaponProperty] set [exp] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
update [BlGame01].dbo.[AccessoryProperty] set [exp] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                    }
                    else if (typ == "data_id")
                    {
                        sqlstr = $@"
begin tran
begin try  
update [BlGame01].dbo.[WeaponProperty] set [data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
update [BlGame01].dbo.[AccessoryProperty] set [data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
update [BlGame01].dbo.[GroceryProperty] set [data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
update [BlGame01].dbo.[CostumeProperty] set [data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
update [BlGame01].dbo.[GemProperty] set [data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                    }
                    else if (typ == "hh")
                    {
                        sqlstr = $@"
begin tran
begin try  
update [BlGame01].dbo.[WeaponProperty] set [appearance_item_data_id] = '{datas}' where [pcid] = '{pcid}' and [id] = '{id}';
     end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran";
                    }
                    Task.Run(() =>
                    {
                        
                        if (db.IsConnection)
                        {
                            count = db._SQL_IDU(sqlstr);
                            ReturnValue = count;
                            ReturnValue1 = datas;
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
                MessageBox.Show($"修改时发生错误 \r\n角色PCID:{pcid}\r\n物品ID:{id}\r\n错误信息:{ex.Message}", "无法执行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox1.Text);
                List<dj_item> name = dat.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label2.Text = $@"道具名:{name[0].Name}";
                }
                else
                {
                    label2.Text = $@"未找到";
                }
                
            }
            catch 
            {

            }
            
        }

        private void ChangedData_Shown(object sender, EventArgs e)
        {
            try
            {
                int data = Convert.ToInt32(textBox1.Text);
                List<dj_item> name = dat.Where(it => it.ID == data).ToList();
                if (name.Count > 0)
                {
                    label2.Text = $@"道具名:{name[0].Name}";
                }
                else
                {
                    label2.Text = $@"未找到";
                }

            }
            catch
            {

            }
        }
    }
}
