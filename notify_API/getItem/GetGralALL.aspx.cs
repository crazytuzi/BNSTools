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
    public partial class GetGralALL : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string GUID = Request.QueryString["GUID"];
            if (GUID != null)
            {
                List<Draw_Data_Entity> list = new List<Draw_Data_Entity>();
                string sqlstr = $@"select * from [AchiDB].dbo.[Draw_Data] where [DrawGUID] = '{GUID}' order by [Number]";
                using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                {
                    while (sdr.Read())
                    {
                        Draw_Data_Entity T = new Draw_Data_Entity()
                        {
                            Data = sdr["Data"].ToString(),
                            ChangedTimer = Convert.ToDateTime(sdr["ChangedTimer"]),
                            DataName = sdr["DataName"].ToString(),
                            DoubleNumber = Convert.ToInt32(sdr["DoubleNumber"]),
                            DrawGUID = sdr["DrawGUID"].ToString(),
                            GUID = sdr["GUID"].ToString(),
                            Number = Convert.ToInt32(sdr["Number"])
                        };
                        list.Add(T);
                    }
                    sdr.Close();
                }
                return_result = JsonConvert.SerializeObject(list);
            }
            else
            {
                return_result = "Error";
            }
        }
    }
}