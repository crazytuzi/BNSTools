using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class PasswordChanged : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string Acction = Request.QueryString["Acction"];
                string oldPassword = Request.QueryString["oldPassword"];
                string NewPassword = Request.QueryString["NewPassword"];

                string Epoch_str = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<AppName>AuthSrv2</AppName>"));
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<Epoch>") + 7);
                string Epoch = Epoch_str.Substring(0, Epoch_str.IndexOf("</Epoch>"));
                string login_str = Acction_Login(Acction, oldPassword, Epoch);
                switch (login_str)
                {
                    case "Success":

                        bool ChangedStu = AcctionPassChanged(Acction, NewPassword, Epoch);//开始修改密码
                        if (ChangedStu)
                            //修改成功 弹出提示
                            return_result = "Success";
                        else
                            return_result = "ChangedError";

                        break;
                    case "ServiceError":
                        return_result = "ServiceError";

                        break;
                    case "Error":
                        return_result = "LoginError";
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return_result = ex.Message;
            }


        }
        /// <summary>
        /// 密码修改 成功返回True 失败返回False
        /// </summary>
        /// <param name="Acction"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        private bool AcctionPassChanged(string Acction, string NewPassword, string Epoch)
        {
            Random x = new Random();
            string _Acction = Acction + x.Next(9) + x.Next(9);
            if (Acction.Length >= 9)
            {
                _Acction = Acction.Substring(0, 3) + x.Next(9) + x.Next(9) + x.Next(9) + x.Next(9) + x.Next(9);
            }
            string Get_Str = $"http://127.0.0.1:6605/spawned/AuthSrv.1.{Epoch}/account/change_login_name?old_login_name={Acction}@ncsoft.com&new_login_name={_Acction}@ncsoft.com&password={NewPassword}&_login_name_validated=on&_kick=on";

            string Get_Str_1 = $"http://127.0.0.1:6605/spawned/AuthSrv.1.{Epoch}/account/change_login_name?old_login_name={_Acction}@ncsoft.com&new_login_name={Acction}@ncsoft.com&password={NewPassword}&_login_name_validated=on&_kick=on";
            string _res_1 = Tools.Get(Get_Str, new Dictionary<string, string>());
            if (_res_1.Contains("<Reply/>"))
            {
                Thread.Sleep(300);
                string _res_2 = Tools.Get(Get_Str_1, new Dictionary<string, string>());
                if (_res_2.Contains("<Reply/>"))
                    //修改成功
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        private string Acction_Login(string Acction, string Password, string Epoch)
        {
            try
            {
                string Get_Str = "http://" + $@"127.0.0.1:6605/spawned/AuthSrv.1.{Epoch}/test/login_trusted?loginName={Acction}%40ncsoft.com&password={Password}&passwordHash=";
                string _res_1 = Tools.Get(Get_Str, new Dictionary<string, string>());
                if (_res_1.Contains("500"))
                {
                    return "ServiceError";
                }
                else if (_res_1.Contains("<Reply>"))
                {
                    return "Success";
                }
                else
                {
                    return "Error";
                }
            }
            catch
            {
                return "ServiceError";
            }
        }
    }
}