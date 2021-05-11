using Newtonsoft.Json;
using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class debug : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string Epoch_str = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<AppName>VirtualCurrencySrv</AppName>"));
                Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<Epoch>") + 7);
                string Epoch = Epoch_str.Substring(0, Epoch_str.IndexOf("</Epoch>"));
                string Acction = Request.QueryString["Acction"];
                string dq = Request.QueryString["dq"];
                string sqlstr = $"select [UserId] from [PlatformAcctDb].dbo.[Users] where [LoginName] = N'{Acction}'+ N'@ncsoft.com'";
                string AcctionID = DBHelper.SelectScalar(sqlstr).ToString().ToUpper();

                if (AcctionID != "")
                {

                    string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
                    string url = $"http://127.0.0.1:6605/spawned/VirtualCurrencySrv.1.{Epoch}/test/command_console";
                    string Request = string.Empty;
                    Request += $@"
<Request>
  <CurrencyId>51</CurrencyId>
  <Amount>{dq}</Amount>
  <EffectiveTo>2099-05-05T03:30:30+09:00</EffectiveTo>
  <IsRefundable>0</IsRefundable>
  <DepositReasonCode>5</DepositReasonCode>
  <DepositReason>Post补单</DepositReason>
  <RequestCode>99</RequestCode>
  <RequestId>{MD5Encrypt(AcctionID + tim)}</RequestId>
</Request>";
                    Request = System.Web.HttpUtility.UrlEncode(Request, System.Text.Encoding.UTF8);
                    Dictionary<string, string> dicx = new Dictionary<string, string>();
                    dicx.Add("protocol", "VirtualCurrency");
                    dicx.Add("command", "Deposit");
                    dicx.Add("to", $"{AcctionID}");
                    dicx.Add("from", "");
                    dicx.Add("message", $"{Request}");

                    try
                    {
                        Post(url, dicx);
                    }
                    catch
                    {
                        return_result = "error";
                    }
                    return_result = "success";

                }

            }
            catch
            {

            }
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strText">待加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] targetData = md5.ComputeHash(Encoding.UTF8.GetBytes(strText));

            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String.ToUpper();
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            int i = 0;
            StringBuilder param = new StringBuilder();
            foreach (var item in dic)
            {
                if (i > 0)
                    param.Append("&");
                param.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] postD = Encoding.UTF8.GetBytes(param.ToString());
            string slt = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postD, 0, postD.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            slt = sr.ReadToEnd();
            sr.Close();
            response.Close();
            
            return slt;
        }
    }
}