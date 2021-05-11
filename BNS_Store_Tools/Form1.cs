using BNS_Store_Tools.Entity;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Store_Tools
{
    public partial class Form1 : Form
    {
        private static List<string> Mac = getMacAddr_Local();
        DataChanged db;

        Categories Store_Title;


        /// <summary>
        /// 已上架集合
        /// </summary>
        List<Goods_Data> Goods_Data_not_null;

        /// <summary>
        /// 未上架集合
        /// </summary>
        List<Goods_Data> Goods_Data_is_null;

        string _db = $"[GoodsDb]";
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "连接数据库")
            {
                button3.Text = "连接中…";
                db.Connection(textBox1.Text.Trim(), textBox2.Text.Trim(), textBox3.Text);

                UpdateAppConfig("ServerIP", textBox1.Text.Trim());
                UpdateAppConfig("Acction", textBox2.Text.Trim());
                UpdateAppConfig("Password", textBox3.Text);
            }
            else if (button3.Text == "断开连接")
            {
                button3.Text = "连接数据库";
                db.Close();
            }
            
        }
        private void ConnectionStatus(bool Status) 
        {
            if (Status)
            {
                //连接成功
                this.Invoke(new Action(delegate 
                {
                    button3.Text = "断开连接";
                }));

                GetAllData();

            }
            else
            {
                //连接失败
                this.Invoke(new Action(delegate
                {
                    button3.Text = "连接数据库";
                }));
            }
        }
        private void IDU_Status(int Count) 
        {
        
        }
        private void ReadData(SqlDataReader sdr) 
        {
        
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("MAC", Mac[0]);

            //初始化
            db = new DataChanged(this);
                db.ConnStatus += ConnectionStatus;
                db.IDU_Count += IDU_Status;
                db.SDR_Count += ReadData;

                textBox1.Text = GetAppConfig("ServerIP") == null ? textBox1.Text : GetAppConfig("ServerIP");
                textBox2.Text = GetAppConfig("Acction") == null ? textBox2.Text : GetAppConfig("Acction");
                textBox3.Text = GetAppConfig("Password") == null ? textBox3.Text : GetAppConfig("Password");

        }
        private void 重新获取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => 
            {
                GetAllData();
            });
        }
        private void GetAllData() 
        {
            //开始进行查询

            //--先获取父级
            string sql = string.Empty;
            sql = $"select * from [Categories] where [CategoryName] = N'钻石商城'";
            using (SqlDataReader sdr = db._SQLRead(sql))
            {
                if (sdr.Read())
                {
                    Store_Title = new Categories()
                    {
                        CategoryId = sdr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["CategoryId"]),
                        CategoryName = sdr["CategoryName"].ToString(),
                        ParentCategoryId = sdr["ParentCategoryId"] ==DBNull.Value ? 0 : Convert.ToInt32(sdr["ParentCategoryId"]),
                        ParentCount = new List<Categories>()
                    };
                }
            }//获取完商店 开始获取主菜单
            if (Store_Title.CategoryId > 0)//商城菜单存在
            {
                sql = $"select * from [Categories] where [ParentCategoryId] = '{Store_Title.CategoryId}'";//查询主菜单
                using (SqlDataReader sdr = db._SQLRead(sql))
                {
                    while (sdr.Read())
                    {
                        Store_Title.ParentCount.Add(new Categories()
                        {
                            CategoryId = sdr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["CategoryId"]),
                            CategoryName = sdr["CategoryName"].ToString(),
                            ParentCategoryId = sdr["ParentCategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ParentCategoryId"]),
                            ParentCount = new List<Categories>()
                        });
                    }
                }

                for (int i = 0; i < Store_Title.ParentCount.Count; i++)//子菜单
                {
                    sql = $"select * from [Categories] where [ParentCategoryId] = '{Store_Title.ParentCount[i].CategoryId}'";
                    using (SqlDataReader sdr = db._SQLRead(sql))
                    {
                        while (sdr.Read())
                        {
                            Store_Title.ParentCount[i].ParentCount.Add(new Categories()
                            {
                                CategoryId = sdr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["CategoryId"]),
                                CategoryName = sdr["CategoryName"].ToString(),
                                ParentCategoryId = sdr["ParentCategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ParentCategoryId"]),
                                ParentCount = null
                            });
                        }
                    }
                }
            }
            //--获取商店物品
            sql = $@"
select distinct(b.[GoodsId]),b.[BasicSalePrice],c.[ItemQuantity],a.[GoodsDisplayName],c.[ItemId],d.[CategoryId] from 
[GoodsDisplay] as a inner join
[GoodsItemBasicPrices] as b  inner join
(select * from [GoodsItems] a where not exists (select 1 from [GoodsItems] where [GoodsId] = a.GoodsId and [ItemId] > a.ItemId)) as c left join
(select * from [GoodsCategories] a where not exists(select 1 from [GoodsCategories] where GoodsId=a.GoodsId and CategoryId>a.CategoryId)) as d
on d.GoodsId = c.GoodsId
on c.GoodsId = b.GoodsId
on b.GoodsId = a.GoodsId
where 1 = 1 
";
            Goods_Data_is_null = new List<Goods_Data>();
            Goods_Data_not_null = new List<Goods_Data>();

            using (SqlDataReader sdr = db._SQLRead(sql))
            {
                while (sdr.Read())
                {
                    if (sdr["CategoryId"] == DBNull.Value)
                    {
                        Goods_Data_is_null.Add(new Goods_Data()
                        {
                            GoodsId = sdr["GoodsId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["GoodsId"]),
                            ItemId = sdr["ItemId"] == DBNull.Value ? "ID读取错误" : sdr["ItemId"].ToString(),
                            ItemQuantity = sdr["ItemQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ItemQuantity"]),
                            GoodsDisplayName = sdr["GoodsDisplayName"].ToString(),
                            BasicSalePrice = sdr["BasicSalePrice"] == DBNull.Value ? 0 : Convert.ToDouble(sdr["BasicSalePrice"]),
                            CategoryId = sdr["CategoryId"] == DBNull.Value ? "未上架" : $"页面ID[{sdr["CategoryId"].ToString()}]"
                        });
                    }
                    else
                    {
                        Goods_Data_not_null.Add(new Goods_Data()
                        {
                            GoodsId = sdr["GoodsId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["GoodsId"]),
                            ItemId = sdr["ItemId"] == DBNull.Value ? "ID读取错误" : sdr["ItemId"].ToString(),
                            ItemQuantity = sdr["ItemQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ItemQuantity"]),
                            GoodsDisplayName = sdr["GoodsDisplayName"].ToString(),
                            BasicSalePrice = sdr["BasicSalePrice"] == DBNull.Value ? 0 : Convert.ToDouble(sdr["BasicSalePrice"]),
                            CategoryId = sdr["CategoryId"] == DBNull.Value ? "未上架" : $"页面ID[{sdr["CategoryId"].ToString()}]"
                        });
                    }
                    
                }
            }

            List<ListViewItem> _ls_data = new List<ListViewItem>();
            for (int i = 0; i < Goods_Data_not_null.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.Tag = Goods_Data_is_null[i].GoodsId;
                lvi.SubItems.Add(Goods_Data_not_null[i].GoodsDisplayName);
                lvi.SubItems.Add(Goods_Data_not_null[i].ItemQuantity.ToString());
                lvi.SubItems.Add(Goods_Data_not_null[i].BasicSalePrice.ToString());
                lvi.SubItems.Add(Goods_Data_not_null[i].ItemId.ToString());
                _ls_data.Add(lvi);
            }
            List<ListViewItem> _ls_data_null = new List<ListViewItem>();
            for (int i = 0; i < Goods_Data_is_null.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.Tag = Goods_Data_is_null[i].GoodsId;
                lvi.SubItems.Add(Goods_Data_is_null[i].GoodsDisplayName);
                lvi.SubItems.Add(Goods_Data_is_null[i].ItemQuantity.ToString());
                lvi.SubItems.Add(Goods_Data_is_null[i].BasicSalePrice.ToString());
                lvi.SubItems.Add(Goods_Data_is_null[i].ItemId.ToString());
                _ls_data_null.Add(lvi);
            }

            this.Invoke(new Action(() =>
            {
                listView1.Items.Clear();
                listView2.Items.Clear();
                listView1.Items.AddRange(_ls_data.ToArray());
                listView2.Items.AddRange(_ls_data_null.ToArray());
            }));

            this.Invoke(new Action(delegate
            {
                //绑定下拉框
                comboBox3.DisplayMember = "CategoryName";
                comboBox3.ValueMember = "CategoryId";
                comboBox3.DataSource = Store_Title.ParentCount.ToArray();
            }));

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Trim().Equals("") || textBox5.Text.Trim().Equals("") || textBox6.Text.Trim().Equals("") || textBox7.Text.Trim().Equals(""))
            {
                MessageBox.Show("兄Dei,信息填完整啊！","温馨提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            int itemID = Convert.ToInt32(textBox4.Text.Trim());//道具ID
            string GoodsDisplayName = textBox5.Text.Trim();
            int ItemQuantity = Convert.ToInt32(textBox6.Text.Trim());
            double BasicSalePrice = Convert.ToDouble(textBox7.Text.Trim());
            string gameItemKey = GetKey(itemID.ToString());//Key

            Task.Run(()=> 
            {

                string sql = $@"select max([GoodsId]) from {_db}.[dbo].[Goods] ";
                int GoodsId = Convert.ToInt32(db._SQLScalar(sql)) + 1;//自增1

                bool status = InsertStore(GoodsId, itemID, GoodsDisplayName, ItemQuantity, BasicSalePrice, gameItemKey);
                if (status)
                {
                    Goods_Data sp = new Goods_Data()
                    {
                        BasicSalePrice = BasicSalePrice,
                        CategoryId = "未上架",
                        GoodsDisplayName = GoodsDisplayName,
                        GoodsId = GoodsId,
                        ItemId = itemID.ToString(),
                        ItemQuantity = ItemQuantity
                    };
                    Goods_Data_is_null.Add(sp);
                    this.Invoke(new Action(delegate 
                    {
                        listView2.Items.Add(rest_lvi(Convert.ToInt32(listView2.Items[listView2.Items.Count - 1].SubItems[0].Text) + 1, sp));
                    }));
                    
                }
                xs(status);
            });

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
        private bool InsertStore(int GoodsId, int itemID,string GoodsDisplayName,int ItemQuantity,double BasicSalePrice,string gameItemKey) 
        {
            try
            {
                string sql = string.Empty;
                //sql = $@"select max([GoodsId]) from {_db}.[dbo].[Goods] ";
                //int GoodsId = Convert.ToInt32(db._SQLScalar(sql)) + 1;//自增1

                sql = $@"
begin tran
begin try  
";

                sql += $@"insert into {_db}.[dbo].[Goods](GoodsId,GoodsName,GoodsAppGroupCode,GoodsType,DeliveryType,SaleStatus,EffectiveFrom,EffectiveTo,SaleableQuantity,RefundUnitCode,IsRefundable,IsAvailableRecurringPayment,Changed,ChangerAdminAccount,GoodsDescription,GoodsData,ParentGoodsId,GoodsPurchaseType,SelectableItemQuantity, GoodsPurchaseCheckMask) 
            values ('{GoodsId}',N'{textBox5.Text.Trim()}','bnsgrnTH','3','1','2','2017-09-05','2099-12-31',0,1,'False','True','2017-09-05','TestAdminAccount',N'{textBox5.Text.Trim()}','AAAAAAE=',NULL,'1',NULL,'0');";

                sql += $@"insert into {_db}.[dbo].[GoodsChanges](ChangeId,ChangeType,Registered,RegistrarAdminAccount,ChangeDescription,GoodsAppGroupCode,IsDisplayable) values
        ('{GoodsId}', '2', '2017-09-05 20:00', 'TestAdminAccount', NULL, 'bnsgrnTH', 'True');";

                sql += $@"insert into {_db}.[dbo].[GoodsDisplay](GoodsId,LanguageCode,GoodsDisplayName,GoodsDisplayDescription) values
                ('{GoodsId}','11',N'{textBox5.Text.Trim()}',N'{textBox5.Text.Trim()}');";

                sql += $@"insert into {_db}.[dbo].[GoodsSalePricePolicies](GoodsId,CurrencyGroupId,PricePolicyType,EffectiveFrom,EffectiveTo,SalePrice) 
                    values('{GoodsId}', 71, 1, '2017-06-06 00:00', '2099-12-31 23:59', '{BasicSalePrice}');";

                sql += $@"insert into {_db}.[dbo].[Items](ItemId,ItemName,ItemAppGroupCode,ItemType,IsConsumable,BasicPrice,BasicCurrencyGroupId,Changed,ChangerAdminAccount,ItemDescription) 
                    values('{itemID}', N'{GoodsDisplayName}', 'bnsgrnTH', '3', 'False', '{BasicSalePrice}', '69', '2015-02-13 00:00', 'TestAdminAccount', N'{GoodsDisplayName}');";

                sql += $@"insert into {_db}.[dbo].[ItemDisplay](ItemId,LanguageCode,ItemDisplayName,ItemDisplayDescription) 
                values ('{itemID}', 11, N'{GoodsDisplayName}',  N'{GoodsDisplayName}');";

                sql += $@"insert into {_db}.[dbo].[GoodsItems](GoodsId,ItemId,ItemQuantity,ItemExpirationType,ItemData,DeliveryPriority) values
                ('{GoodsId}', '{itemID}', '{ItemQuantity}', 0, 'AAAAAAEA', 1);";

                sql += $@"insert into {_db}.[dbo].[GameItems](ItemId,GameItemKey,GameItemData) 
                values('{itemID}', '{gameItemKey}', 'AAAAAAAAAAA=');";

                sql += $@"insert into {_db}.[dbo].[GoodsBasicPrices](GoodsId,CurrencyGroupId,BasicSalePrice,RefundFee) values
                ('{GoodsId}', 71, '{BasicSalePrice}', 0);";

                sql += $@"insert into {_db}.[dbo].[GoodsItemBasicPrices] (GoodsId,ItemId,CurrencyGroupId,BasicSalePrice) values
                ('{GoodsId}', '{itemID}', 71, '{BasicSalePrice}');";

                sql += $@"
                end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";

                int code = db._SQL_IDU(sql);
                if (code > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
            
        }
        private bool UpStore(int GoodsID,int page1 = 60,int page2 = 0) 
        {
           string sql = $@"insert into {_db}.[dbo].[GoodsCategories](GoodsId,CategoryId,DisplayOrder) values
            ('{GoodsID}', '{page1}', '1')";
            if (db._SQL_IDU(sql) > 0)
            {
                if (page2 > 0)
                {
                    sql = $@"insert into {_db}.[dbo].[GoodsCategories](GoodsId,CategoryId,DisplayOrder) values
                        ('{GoodsID}','{page2}', '1')";
                    if (db._SQL_IDU(sql) > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return false;
            
        }
        private bool DownStore(int GoodsID) 
        {
            string sql = $@"delete {_db}.[dbo].[GoodsCategories] where [GoodsID] = '{GoodsID}'";
            if (db._SQL_IDU(sql) > 0)
            {
                return true;
            }
            else
                return false;
        }
        private bool Delete(int ItemID, int GoodsID) 
        {
            try
            {
                string sql = string.Empty;
                sql = $@"
begin tran
begin try  
";

                sql += $@"delete[GoodsCategories] where[GoodsId] = '{GoodsID}';";
                sql += $@"delete[GoodsItemBasicPrices] where[GoodsId] = '{GoodsID}';";
                sql += $@"delete [GoodsBasicPrices] where [GoodsId] = '{GoodsID}';";
                sql += $@"delete [GameItems] where [ItemId] = '{ItemID}';";
                sql += $@"delete[GoodsItems] where[GoodsId] = '{GoodsID}';";
                sql += $@"delete [ItemDisplay] where [ItemId] ='{ItemID}';";
                sql += $@"delete [Items] where [ItemId] = '{ItemID}';";
                sql += $@"delete[GoodsSalePricePolicies] where[GoodsId] = '{GoodsID}';";
                sql += $@"delete[GoodsDisplay] where[GoodsId] = '{GoodsID}';";
                sql += $@"delete[GoodsChanges] where[ChangeId] = '{GoodsID}';";
                sql += $@"delete[Goods] where[GoodsId] = '{GoodsID}';";

                sql += $@"
                end try
begin catch
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";
                

                int code = db._SQL_IDU(sql);
                if (code > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void xs(bool status) 
        {
            string msg = string.Empty;
            if (status)
            
                msg = "操作成功";
            
            else
            
                msg = "操作失败";
            

            this.Invoke(new Action(delegate
            {
                label13.Text = msg;
            }));
            Thread.Sleep(2500);
            this.Invoke(new Action(delegate
            {
                label13.Text = "";
            }));
        }
        private void 删除商品ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int GoodsID = Convert.ToInt32(listView2.SelectedItems[0].Tag);
            int ItemsID = Convert.ToInt32(listView2.SelectedItems[0].SubItems[4].Text);
            Task.Run(() => 
            {
                bool Status = Delete(ItemsID,GoodsID);
                if (Status)
                {
                    for (int i = 0; i < Goods_Data_is_null.Count; i++)
                    {
                        var item = Goods_Data_is_null[i];
                        if (item.ItemId == ItemsID.ToString() && item.GoodsId == GoodsID)
                        {
                            Goods_Data_is_null.Remove(item);
                            this.Invoke(new Action(delegate
                            {
                                listView2.Items.Remove(listView2.SelectedItems[0]);
                            }));
                        }
                    }
                }
                
                xs(Status);
            });
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(comboBox3.SelectedValue) > -1 && Store_Title.ParentCount[Convert.ToInt32(comboBox3.SelectedIndex)].ParentCount != null)
            {
                comboBox4.DisplayMember = "CategoryName";
                comboBox4.ValueMember = "CategoryId";
                comboBox4.DataSource = Store_Title.ParentCount[Convert.ToInt32(comboBox3.SelectedIndex)].ParentCount.ToArray();

                if (Store_Title.ParentCount[Convert.ToInt32(comboBox3.SelectedIndex)].ParentCount.ToArray().Length == 0)//没有数据加载
                {
                    int value = Convert.ToInt32(comboBox3.SelectedValue);
                    List<Goods_Data> ls = new List<Goods_Data>();
                    foreach (var item in Goods_Data_not_null)
                    {
                        if (item.CategoryId == $"页面ID[{value}]")
                        {
                            ls.Add(item);
                        }
                    }
                    List<ListViewItem> lvi = rest_lvi(ls);
                    this.Invoke(new Action(delegate
                    {
                        listView1.Items.Clear();
                        listView1.Items.AddRange(lvi.ToArray());
                    }));
                }
            }
        }
        private List<ListViewItem> rest_lvi(List<Goods_Data> dat) 
        {
            List<ListViewItem> _ls_data = new List<ListViewItem>();
            for (int i = 0; i < dat.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.Tag = dat[i].GoodsId;
                lvi.SubItems.Add(dat[i].GoodsDisplayName);
                lvi.SubItems.Add(dat[i].ItemQuantity.ToString());
                lvi.SubItems.Add(dat[i].BasicSalePrice.ToString());
                lvi.SubItems.Add(dat[i].ItemId.ToString());
                _ls_data.Add(lvi);
            }
            return _ls_data;
        }
        private ListViewItem rest_lvi(int id , Goods_Data dat)
        {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = id.ToString();
                lvi.Tag = dat.GoodsId;
                lvi.SubItems.Add(dat.GoodsDisplayName);
                lvi.SubItems.Add(dat.ItemQuantity.ToString());
                lvi.SubItems.Add(dat.BasicSalePrice.ToString());
                lvi.SubItems.Add(dat.ItemId.ToString());
            return lvi;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //Goods_Data_is_null
            button5.Text = "搜索中";
            button5.Enabled = false;
            textBox9.Enabled = false;
            string str = textBox9.Text.Trim();
            Task.Run(()=> 
            {
                List<Goods_Data> ls = new List<Goods_Data>();
                foreach (var item in Goods_Data_is_null)
                {
                    if (item.GoodsDisplayName.Contains(str))
                    {
                        ls.Add(item);
                    }
                }
                List<ListViewItem> lvi = rest_lvi(ls);
                this.Invoke(new Action(delegate 
                {
                    listView2.Items.Clear();
                    listView2.Items.AddRange(lvi.ToArray());
                    button5.Enabled = true;
                    textBox9.Enabled = true;
                    button5.Text = "搜索";
                }));
            });

        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(comboBox4.SelectedValue);
            Task.Run(() =>
            {
                List<Goods_Data> ls = new List<Goods_Data>();
                foreach (var item in Goods_Data_not_null)
                {
                    if (item.CategoryId == $"页面ID[{value}]")
                    {
                        ls.Add(item);
                    }
                }
                List<ListViewItem> lvi = rest_lvi(ls);
                this.Invoke(new Action(delegate
                {
                    listView1.Items.Clear();
                    listView1.Items.AddRange(lvi.ToArray());
                }));
            });

        }
        private void 上架ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                if (comboBox3.SelectedIndex > -1)
                {
                    int p1 = Convert.ToInt32(comboBox3.SelectedValue);
                    int p2;
                    if (comboBox4.SelectedIndex > -1)
                        p2 = Convert.ToInt32(comboBox4.SelectedValue);
                    else
                        p2 = 0;
                    int GoodsID = Convert.ToInt32(listView2.SelectedItems[0].Tag);
                    string ItemId = listView2.SelectedItems[0].SubItems[4].Text;
                    Task.Run(() =>
                    {
                        bool Store = UpStore(GoodsID, p1, p2);
                        if (Store)
                        {
                            for (int i = 0; i < Goods_Data_is_null.Count; i++)
                            {
                                var item = Goods_Data_is_null[i];
                                if (item.ItemId == ItemId)
                                {
                                    Goods_Data_not_null.Add(item);
                                    Goods_Data_is_null.Remove(item);

                                    this.Invoke(new Action(delegate
                                    {
                                        listView2.Items.Remove(listView1.SelectedItems[0]);
                                        if (item.CategoryId == comboBox4.SelectedValue.ToString())
                                        {
                                            listView1.Items.Add(rest_lvi(Convert.ToInt32(listView2.Items[listView2.Items.Count - 1].SubItems[0].Text) +1, item));
                                        }
                                        
                                    }));
                                }
                            }

                        }
                        xs(Store);
                    });
                }
            }
        }
        private void 下架ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int GoodsId = Convert.ToInt32(listView1.SelectedItems[0].Tag);
                string ItemId = listView1.SelectedItems[0].SubItems[4].Text;
                Task.Run(() =>
                {
                    bool Store = DownStore(GoodsId);
                    if (Store)
                    {
                        for (int i = 0; i < Goods_Data_not_null.Count; i++)
                        {
                            var item = Goods_Data_not_null[i];
                            if (item.ItemId == ItemId)
                            {
                                Goods_Data_is_null.Add(item);
                                Goods_Data_not_null.Remove(item);

                                this.Invoke(new Action(delegate
                                {
                                    listView1.Items.Remove(listView1.SelectedItems[0]);
                                    listView2.Items.Add(rest_lvi(Convert.ToInt32(listView2.Items[listView2.Items.Count - 1].SubItems[0].Text) + 1, item));
                                }));
                            }
                        }

                    }
                    xs(Store);
                });
            }
            
        }
        private void 修改价格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0) 
            {
                int GoodId = Convert.ToInt32(listView2.SelectedItems[0].Tag);
                int Count = Convert.ToInt32(listView2.SelectedItems[0].SubItems[2].Text);
                int Price = Convert.ToInt32(listView2.SelectedItems[0].SubItems[3].Text);
                Form2 x = new Form2(db,GoodId,Count,Price);
                if (x.ShowDialog() == DialogResult.OK)
                {
                    listView2.SelectedItems[0].SubItems[2].Text = x.counts.ToString();
                    listView2.SelectedItems[0].SubItems[3].Text = x.prices.ToString();
                }
            }
                
        }
        private void 修改价格ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int GoodId = Convert.ToInt32(listView1.SelectedItems[0].Tag);
                int Count = Convert.ToInt32(listView1.SelectedItems[0].SubItems[2].Text);
                int Price = Convert.ToInt32(listView1.SelectedItems[0].SubItems[3].Text);
                Form2 x = new Form2(db, GoodId, Count, Price);
                if (x.ShowDialog() == DialogResult.OK)
                {
                    listView1.SelectedItems[0].SubItems[2].Text = x.counts.ToString();
                    listView1.SelectedItems[0].SubItems[3].Text = x.prices.ToString();
                }
            }
        }
        private void 删除商品ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int GoodsID = Convert.ToInt32(listView1.SelectedItems[0].Tag);
            int ItemsID = Convert.ToInt32(listView1.SelectedItems[0].SubItems[4].Text);
            Task.Run(() =>
            {
                bool Status = Delete(ItemsID, GoodsID);
                if (Status)
                {
                    for (int i = 0; i < Goods_Data_not_null.Count; i++)
                    {
                        var item = Goods_Data_not_null[i];
                        if (item.ItemId == ItemsID.ToString() && item.GoodsId == GoodsID)
                        {
                            Goods_Data_not_null.Remove(item);
                            this.Invoke(new Action(delegate
                            {
                                listView1.Items.Remove(listView1.SelectedItems[0]);
                            }));
                        }
                    }
                }

                xs(Status);
            });
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox4.Text = GetKey(textBox4.Text.Trim());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string json = textBox5.Text;
            var data = new { Achi = new[] { new {AchiKey="",Achivalue=0 } }, item = new[] { new { itemid = 0,Count=0 } } };
            data = JsonConvert.DeserializeAnonymousType(json, data);
        }
    }
}
