using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.RunLock
{
    public partial class Control : System.Web.UI.Page
    {
        static int Count = 0;
        static bool Sta = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Password = Request.QueryString["Password"];
                if (Password == "cl981125")
                {
                    Sta = true;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Sta)
            {
                SqlDataAdapter sda = DBHelper.SelectAdapter(TextBox1.Text.Trim());
                DataSet ds = new DataSet();
                sda.Fill(ds);
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }
            
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (Sta)
            {
                if (DBHelper.IDU(TextBox2.Text.Trim()) > 0)
                {
                    Count++;
                    Label1.Text = $"Success Count:{Count}";
                }
            }
            
        }
    }
}