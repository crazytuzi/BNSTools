using BNS_Tools_ALL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools_ALL
{
    public partial class PCIDChanged : Form
    {
        int pcid;
        DataChanged db;
        int count;
        string typ;
        List<data_Changed> dat;

        public int ReturnValue { get; set; }
        public PCIDChanged()
        {
            InitializeComponent();
        }
        public PCIDChanged(DataChanged _db, int _pcid, string _typ, List<data_Changed> _data)
        {
            InitializeComponent();
            db = _db;
            pcid = _pcid;
            typ = _typ;
            dat = _data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "") 
            {
                try
                {
                    List<string> sql = new List<string>();
                    string item_id = string.Empty;
                    foreach (var item in dat)
                    {
                        item_id += $"'{item.id}'";
                        if (item != dat[dat.Count - 1])
                        {
                            item_id += $",";
                        }
                    }
                    if (typ == "ChangedPCID")
                    {
                        sql.Add($"update [BlGame01].dbo.[GemProperty] set [pcid] = '{textBox1.Text.Trim()}' where [pcid]='{pcid}' and [id] in ({item_id}); -- 八卦");
                        sql.Add($"update [BlGame01].dbo.[AccessoryProperty] set [pcid] = '{textBox1.Text.Trim()}' where [pcid]='{pcid}' and [id] in ({item_id}); -- 首饰/衣服");
                        sql.Add($"update[BlGame01].dbo.[GroceryProperty] set[pcid] = '{textBox1.Text.Trim()}' where[pcid] = '{pcid}' and [id] in ({item_id}); --材料 / 消耗品");
                        sql.Add($"update[BlGame01].dbo.[WeaponProperty] set[pcid] = '{textBox1.Text.Trim()}' where[pcid] = '{pcid}' and [id] in ({item_id}); --武器");
                        sql.Add($"update[BlGame01].dbo.[CostumeProperty] set[pcid] = '{textBox1.Text.Trim()}' where[pcid] = '{pcid}' and [id] in ({item_id}); --衣服");
                        sql.Add($"update[BlGame01].dbo.[Closet] set[pcid] = '{textBox1.Text.Trim()}' where[pcid] = '{pcid}' and [ItemInstanceID] in ({item_id}); --衣柜");
                        Task.Run(() =>
                        {

                            if (db.IsConnection)
                            {
                                foreach (var item in sql)
                                {
                                    count += db._SQL_IDU(item);
                                    Thread.Sleep(100);
                                }
                                ReturnValue = count;
                                this.Invoke(new Action(delegate
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }));

                            }
                        });
                    }
                    else if (typ == "NewPCID")
                    {
                        string Pc = textBox1.Text.Trim();
                        Task.Run(()=> 
                        {
                            sql.Add($"insert into [BlGame01].dbo.[GemProperty] select [pawn],[converted_lock_expiration_time],([id]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'id','{textBox1.Text.Trim()}' as 'PCID',[depot],[pos],[data_id],[level],[exp],[used],[sequestration],[locked],[lock_expiration_time],[equip_gem_piece_id],[ability_type_main],[ability_value_main],[ability_type_sub1],[ability_value_sub1],[ability_type_sub2],[ability_value_sub2] from [BlGame01].dbo.[GemProperty] where [pcid]='{pcid}' and [id] in ({item_id}); -- 八卦");

                            sql.Add($"insert into [BlGame01].dbo.[AccessoryProperty]  select [pawn],[converted_lock_expiration_time],[converted_time_limit_expiration_time],([id]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'id','{textBox1.Text.Trim()}' as 'PCID',[depot],[pos],[data_id],[level],[exp],[used],[sequestration],[locked],[lock_expiration_time],[time_limit_type],[time_limit_expiration_time],[guild_id] from  [BlGame01].dbo.[AccessoryProperty] where [pcid]='{pcid}' and [id] in ({item_id}); -- 首饰/衣服");
                            
                            sql.Add($"insert into [BlGame01].dbo.[GroceryProperty] select [pawn],([id]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'id','{textBox1.Text.Trim()}' as 'PCID',[depot],[pos],[data_id],[level],[exp],[sequestration],[count] from [BlGame01].dbo.[GroceryProperty] where[pcid] = '{pcid}' and [id] in ({item_id}); --材料 / 消耗品");
                            
                            sql.Add($"insert into [BlGame01].dbo.[WeaponProperty] select [pawn],[converted_lock_expiration_time],[converted_time_limit_expiration_time],([id]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'id','{textBox1.Text.Trim()}' as 'PCID',[depot],[pos],[data_id],[level],[exp],[used],[sequestration],[locked],[lock_expiration_time],[time_limit_type],[time_limit_expiration_time],[durability],[enchant],[appearance_item_data_id],[appearance_item_level],[gem_1],[gem_2],[gem_3],[gem_4],[gem_5],[gem_6] from [BlGame01].dbo.[WeaponProperty] where[pcid] = '{pcid}' and [id] in ({item_id}); --武器");
                            
                            sql.Add($"insert into [BlGame01].dbo.[CostumeProperty] select [pawn],[converted_lock_expiration_time],[converted_time_limit_expiration_time],([id]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'id','{textBox1.Text.Trim()}' as 'PCID',[depot],[pos],[data_id],[level],[exp],[used],[sequestration],[locked],[lock_expiration_time],[time_limit_type],[time_limit_expiration_time],[guild_id] from [BlGame01].dbo.[CostumeProperty] where[pcid] = '{pcid}' and [id] in ({item_id}); --衣服");
                            
                            sql.Add($"insert into [BlGame01].dbo.[Closet] select '{textBox1.Text.Trim()}' as 'PCID',[ClosetGroupID],([ItemInstanceID]+(select cast( floor(rand()*10000000) as int))+{textBox1.Text.Trim()}) as 'ItemInstanceID',[ItemDataID],[Used],[Locked],[LockExpirationTime],[TimeLimitType],[TimeLimitExpirationTime],[ClosetState]  from [BlGame01].dbo.[Closet] where[pcid] = '{pcid}' and [ItemInstanceID] in ({item_id}); --衣柜");

                            if (db.IsConnection)
                            {
                                foreach (var item in sql)
                                {
                                    count += db._SQL_IDU(item);
                                    Thread.Sleep(500);
                                }
                                ReturnValue = count;
                                this.Invoke(new Action(delegate
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }));

                            }
                        });
                        
                    }

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误 \r\n错误信息:{ex.Message}", "无法执行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
        }
    }
}
