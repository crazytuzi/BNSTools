using Newtonsoft.Json;
using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.getItem
{
    public partial class GetRecord : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string GUID = Request.QueryString["GUID"];
            string Acction = Request.QueryString["Acction"];
            if (GUID != null && Acction != null)
            {
                List<GetItemRecord_Entity> list = new List<GetItemRecord_Entity>();
                string sqlstr = $@"select [itemName],[ChangedTimer] from [AchiDB].dbo.[Record] where [AchiactivityGUID] = '{GUID}' and [Acction] = '{Acction}'";
                using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                {
                    while (sdr.Read())
                    {
                        list.Add(new GetItemRecord_Entity()
                        {
                            index = list.Count + 1,
                            ItemName = sdr["ItemName"].ToString(),
                            ItemTimer = Convert.ToDateTime(sdr["ChangedTimer"]).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        
                    }
                    sdr.Close();
                }
                return_result = JsonConvert.SerializeObject(new { code = 0, msg = "", count = list.Count, data = list });
            }
            else
            {
                return_result = "Error";
            }
        }
    }
}