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
    public partial class GetUserMsg : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string DrawGUID = Request.QueryString["DrawGUID"];
            string Acction = Request.QueryString["Acction"];
            if (DrawGUID != null && Acction != null)
            {
                string sqlstr = $@"select isNULL(sum(Balance),0) as Balance from [AchiDB].dbo.[DrawBalance] where [DrawGUID] = '{DrawGUID}' and [Acction] = '{Acction}'";
                int Balance = Convert.ToInt32(DBHelper.SelectScalar(sqlstr));

                int quota = 0, Re_quota = 0;

                sqlstr = $@"select isNULL(sum(Balance),0) as Balance ,isNULL(sum(Re_Balance),0) as Re_Balance from [AchiDB].dbo.[GralCount] where [DrawGUID] = '{DrawGUID}' and [Acction] = '{Acction}'";
                using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                {
                    if (sdr.Read())
                    {
                        quota = Convert.ToInt32(sdr["Balance"]);
                        Re_quota = Convert.ToInt32(sdr["Re_Balance"]);
                    }
                    sdr.Close();
                }

                return_result = JsonConvert.SerializeObject(new { Balance = Balance , quota = quota , Re_quota = Re_quota });
            }
            else
            {
                return_result = "Error";
            }
        }
    }
}