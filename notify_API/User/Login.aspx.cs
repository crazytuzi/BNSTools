using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.User
{
    public partial class Login : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //Login and Check UserChrProject result Name and PCID
            Data_Code result = new Data_Code();
            List<ChrProject> CreData = new List<ChrProject>();
            UserProjcet User_Data = new UserProjcet();

            string Acction = Request.QueryString["Acction"];
            string Password = Request.QueryString["Password"];

            if (Acction != null && Password != null)
            {
                string Epoch_str = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<AppName>AuthSrv2</AppName>"));
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<Epoch>") + 7);
                string Epoch = Epoch_str.Substring(0, Epoch_str.IndexOf("</Epoch>"));
                bool login = Acction_Login(Acction, Password, Epoch);
                if (login)
                {
                    //登录成功
                    string sql = $@"
select c.[userid],a.[pcid],a.[Name], (case a.[faction] when '1' then '武林盟' when '2' then '浑天教' when '0' then '无' end) as 'faction'  from 
[BlGame01].dbo.[CreatureProperty]  as a
inner join [LobbyDB].dbo.[Character] as b 
on a.[pcid] = b.[pcid] 
inner join [PlatformAcctDb].dbo.[users] as c
on b.[GameAccountID] = c.[userid]
where b.[CharacterState] = '2' and c.[loginName] = N'{Acction}'+N'@ncsoft.com'";
                    
                    using (SqlDataReader sdr = DBHelper.SelectReader(sql))
                    {
                        while (sdr.Read())
                        {
                            ChrProject data = new ChrProject()
                            {
                                Camp = sdr["faction"].ToString(),
                                Name = sdr["Name"].ToString(),
                                PCID = Convert.ToInt32(sdr["PCID"])
                            };
                            if (User_Data.UserId == null)
                            {
                                User_Data.UserId = sdr["userid"].ToString();
                            }
                            CreData.Add(data);
                        }
                        sdr.Close();
                    }
                    
                    result.Code = 0;
                    result.Msg = "登录成功";
                    User_Data.Data = CreData;
                    result.Data = User_Data;


                }
                else 
                {
                    result.Code = 1;
                    result.Msg = "登录失败";
                    User_Data.Data = CreData;
                    result.Data = User_Data;
                }

            }
            else
            {
                result.Code = 2;
                result.Msg = "账号或密码为空";
                User_Data.Data = CreData;
                result.Data = User_Data;
            }

            return_result = Tojsons.ToJSON(result); ;//返回

        }
        private bool Acction_Login(string Acction, string Password, string Epoch)
        {
            try
            {
                string Get_Str = "http://" + $@"127.0.0.1:6605/spawned/AuthSrv.1.{Epoch}/test/login_trusted?loginName={Acction}%40ncsoft.com&password={Password}&passwordHash=";
                string _res_1 = Tools.Get(Get_Str, new Dictionary<string, string>());
                if (_res_1.Contains("<Reply>"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}