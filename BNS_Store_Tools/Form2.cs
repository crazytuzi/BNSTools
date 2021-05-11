using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Store_Tools
{
    public partial class Form2 : Form
    {
        DataChanged db;
        int GoodID;
        int Count;
        double Price;
        public int counts { get; set; }
        public double prices { get; set; }
        public Form2(DataChanged _db,int _GoodID,int _Count, double _Price)
        {
            InitializeComponent();
            db = _db;
            GoodID = _GoodID;
            Count = _Count;
            Price = _Price;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                prices = Convert.ToInt32(textBox1.Text);
                counts = Convert.ToInt32(textBox2.Text);

                string sql = $@"update [GoodsItems] set [ItemQuantity] = '{counts}' where [GoodsId] = '{GoodID}'";

                if (db._SQL_IDU(sql) > 0)
                {
                    sql = $@"
begin tran
begin try  
";
                    sql += $@"update [GoodsItemBasicPrices] set [BasicSalePrice] = '{prices}' where [GoodsID] = '{GoodID}';";
                    sql += $@"update  [GoodsSalePricePolicies] set [SalePrice] = '{prices}' where [GoodsID] = '{GoodID}';";
                    sql += $@"
                end try
begin catch
   select Error_number() as ErrorNumber,
          Error_severity() as ErrorSeverity,
          Error_state() as ErrorState ,
          Error_Procedure() as ErrorProcedure ,
          Error_line() as ErrorLine,
          Error_message() as ErrorMessage 
   if(@@trancount>0)
      rollback tran
end catch
if(@@trancount>0)
commit tran
";
                    if (db._SQL_IDU(sql) > 0)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                        this.DialogResult = DialogResult.No;
                }
                else
                    this.DialogResult = DialogResult.No;

                
                this.Close();
                
            }
            catch (Exception ex)
            {
                this.Close();
                this.DialogResult = DialogResult.No;
            }
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            this.Text = $@"商品信息修改[ID-{GoodID}]";
            textBox1.Text = Price.ToString();
            textBox2.Text = Count.ToString();
        }
    }
}
