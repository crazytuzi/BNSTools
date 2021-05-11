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
    public partial class GetDrawMsg : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string GUID = Request.QueryString["GUID"];
            if (GUID != null)
            {
                Draw_Entity T = null;
                string sqlstr = $@"select * from [AchiDB].dbo.[Draw] where [GUID] = '{GUID}' and [Start_Timer] < '{DateTime.Now}' and [Due_Timer] > '{DateTime.Now}'";
                using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                {
                    if (sdr.Read())
                    {
                        T = new Draw_Entity()
                        {
                            DrawName = sdr["DrawName"].ToString(),
                            Due_Timer = Convert.ToDateTime(sdr["Due_Timer"]),
                            F_Msg = sdr["F_Msg"].ToString(),
                            GralData = sdr["GralData"].ToString(),
                            GUID = sdr["GUID"].ToString(),
                            Msg = sdr["Msg"].ToString(),
                            Start_Timer = Convert.ToDateTime(sdr["Start_Timer"])
                        };
                    }
                    sdr.Close();
                }
                
                return_result = JsonConvert.SerializeObject(T);
            }
            else
            {
                return_result = "Error";
            }
        }
    }
}