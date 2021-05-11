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
    public partial class notify_url_data : System.Web.UI.Page
    {
        public string return_result = "";
        public string getPostStr()
        {
            int intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);
            return Encoding.UTF8.GetString(b);
        }
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            coll = Request.Form;

            string[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> dic = GetRequestPost();
            string JsonData = getPostStr();
            try
            {
                
                if (JsonData == "")
                {
                    return_result = "fail";
                    return;
                }
                string ALLGUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff"));

                DBsql.db.Insertable(new notify_data()
                {
                    Data = JsonData,
                    timer = DateTime.Now,
                    GUID = ALLGUID
                }).ExecuteCommand();

                string pay_no = string.Empty;
                string username = string.Empty;
                decimal money = 0.00M;

                switch (dic.Count)
                {
                    case 21:
                        pay_no = dic["trade_no"];
                        username = dic["out_trade_no"];
                        money = Convert.ToDecimal(dic["money"]);
                        break;
                    case 16:
                        pay_no = dic["pay_no"];
                        username = dic["pay_id"];
                        money = Convert.ToDecimal(dic["price"]);

                        break;
                }

                List<SuccessPay> dat = DBsql.db.Queryable<SuccessPay>().Where(it => it.trade_no.Contains(pay_no)).ToList();
                if (dat.Count == 0)
                {
                    dat.Add(new SuccessPay()
                    {
                        GUID = ALLGUID,
                        money = money,
                        Timer = DateTime.Now,
                        trade_no = pay_no,
                        UserName = username,
                        Status = false
                    });
                    //增加回调记录
                    DBsql.db.Insertable(dat[0]).ExecuteCommand();
                }
                if (dat.Count == 1 && !dat[0].Status)
                {
                    long bl = Convert.ToInt64(ConfigurationManager.ConnectionStrings["moneybl"].ConnectionString);
                    long Diamonds = Convert.ToInt64(money * bl); //1:1000



                    try
                    {
                        string sqlstr = $"select [UserId] from [PlatformAcctDb].dbo.[Users] where [LoginName] = N'{username}'+ N'@ncsoft.com'";
                        string AcctionID = DBHelper.SelectScalar(sqlstr).ToString().ToUpper();

                        if (AcctionID != "")
                        {
                            string Epoch_str = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                            Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<AppName>VirtualCurrencySrv</AppName>"));
                            Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<Epoch>") + 7);
                            string Epoch = Epoch_str.Substring(0, Epoch_str.IndexOf("</Epoch>"));

                            int typ = 0;
                            string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
                            string url = $"http://127.0.0.1:6605/spawned/VirtualCurrencySrv.1.{Epoch}/test/command_console";
                            string Request = string.Empty;
                            Request += $@"
<Request>
  <CurrencyId>51</CurrencyId>
  <Amount>{Diamonds}</Amount>
  <EffectiveTo>2099-05-05T03:30:30+09:00</EffectiveTo>
  <IsRefundable>0</IsRefundable>
  <DepositReasonCode>5</DepositReasonCode>
  <DepositReason>Post在线充值</DepositReason>
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
                                typ = 1;
                            }
                            catch
                            {
                                try
                                {
                                    Data_Changed_Add(AcctionID, Diamonds);
                                    typ = 2;
                                }
                                catch
                                {
                                    typ = 0;
                                }
                            }
                            //增加充值记录
                            Pay_Diamonds pay_Diam = new Pay_Diamonds()
                            {
                                GUID = ALLGUID,
                                UserName = username,
                                AddDiamonds = Diamonds,
                                Timer = DateTime.Now
                            };
                            switch (typ)
                            {
                                case 1:
                                    pay_Diam.Type = "Post在线充值";
                                    break;
                                case 2:
                                    pay_Diam.Type = "数据库充值";
                                    break;
                                default:
                                    break;
                            }
                            DBsql.db.Insertable(pay_Diam).ExecuteCommand();
                            dat[0].Status = true;
                            DBsql.db.Updateable(dat[0]).WhereColumns(it => new { it.trade_no }).ExecuteCommand();
                            return_result = "success";

                        }

                    }
                    catch 
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                DBsql.db.Insertable(new notify_data()
                {
                    Data = $"回调发生异常:{ex.Message}->{JsonData}",
                    timer = DateTime.Now,
                    GUID = Guid.NewGuid().ToString()
                }).ExecuteCommand();
                return_result = "fail";
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
        public static bool Data_Changed_Add(string AcctionID,long Diamonds) 
        {
            try
            {
                string tim = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
                string sqlstr = $@"exec [VirtualCurrencyDb].dbo.[p_AddDeposit] '{AcctionID}','51','5','{Diamonds}','{Diamonds}','0','1900-01-01T00:00:00+00:00','2999-12-30 03:30:30.000 +09:00','99','{MD5Encrypt(AcctionID + tim)}',N'数据库充值','0','0'";

                DBHelper.IDU(sqlstr);
                Thread.Sleep(1000);
                return true;
            }
            catch
            {
               
            }
            return false;
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