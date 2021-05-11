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
    public partial class Del_data_id : Form
    {
        Dictionary<int, string> dic;
        DataChanged db;
        public Del_data_id(Dictionary<int, string> dat, DataChanged _db,string tbox) 
        {
            InitializeComponent();
            dic = dat;
            db = _db;
            textBox1.Text = tbox;
        }
        public Del_data_id()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
                string data_str = textBox1.Text.Trim();
                string pcid = string.Empty;
            if (data_str != "")
            {
                string tips = string.Empty;
                int Index = 0;
                foreach (var item in dic)
                {

                    if (Index < 4)
                    {
                        tips += $"[{item.Value}],";

                    }
                    else
                    {
                        tips += $"[{item.Value}]\r\n";
                        Index = 0;
                    }
                    pcid += $"'{item.Key}',";
                    Index++;
                }
                if (tips.Substring(tips.Length - 1, 1) == ",")
                {
                    tips = tips.Substring(0, tips.Length - 1);
                }
                if (pcid.Substring(pcid.Length - 1, 1) == ",")
                {
                    pcid = pcid.Substring(0, pcid.Length - 1);
                }

                DialogResult di = MessageBox.Show($"请确认删除物品ID为({data_str})的物品.\r\n 共选择账号:\r\n{tips}", "请确认操作！", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                Task.Run(() =>
                {
                    if (DialogResult.Yes == di)
                    {
                        string sqlstr = $@"
begin tran
begin try  
delete [BlGame01].dbo.[AccessoryProperty] where [pcid] in ({pcid}) and [data_id] in ({data_str})
delete [BlGame01].dbo.[GroceryProperty] where [pcid] in ({pcid}) and [data_id] in ({data_str})
delete [BlGame01].dbo.[WeaponProperty] where [pcid] in ({pcid}) and [data_id] in ({data_str})
delete [BlGame01].dbo.[CostumeProperty] where [pcid] in ({pcid}) and [data_id] in ({data_str})
delete [BlGame01].dbo.[Closet] where [pcid] in ({pcid}) and [ItemDataID] in ({data_str})
delete [BlGame01].dbo.[GemProperty] where [pcid] in ({pcid}) and [data_id] in ({data_str})
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
                                MessageBox.Show($"已完成操作！受影响数据共:{count}条！", "执行完毕");
                                this.Close();
                            }));
                        }
                    }
                });
            }
            
        }
    }
}
