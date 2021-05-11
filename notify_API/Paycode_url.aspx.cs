using Newtonsoft.Json;
using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class Paycode_url : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string Acction = Request.QueryString["Acction"];
            string price = Request.QueryString["Price"];
            string type = Request.QueryString["type"] == null ? "1" : Request.QueryString["type"];
            string param = Request.QueryString["param"] == null ? "1" : Request.QueryString["param"];
            if (Acction != null && price != null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string pay_url = ConfigurationManager.ConnectionStrings["AddPayUrl"].ConnectionString;
                string pay_id = ConfigurationManager.ConnectionStrings["pay_id"].ConnectionString;
                string notify_url = ConfigurationManager.ConnectionStrings["notify_url"].ConnectionString;
                string key = ConfigurationManager.ConnectionStrings["key"].ConnectionString;
                var par = new { param = param };
                string json = JsonConvert.SerializeObject(par);
                dic.Add("id", pay_id);
                dic.Add("type", type);
                dic.Add("price", price);
                dic.Add("pay_id", Acction);
                dic.Add("param", param);
                dic.Add("notify_url", notify_url);
                dic.Add("return_url", "https://codepay.kjkl8.com/demo_show.html");
                dic.Add("sign", Tools.MD5Encrypt(GetParamSrc(dic)+ key));

                string data = Post(pay_url, dic);
                return_result = data;
            }
        }
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        /// <summary>
        /// 参数按照参数名ASCII码从小到大排序（字典序）
        /// </summary>
        /// <param name="paramsMap"></param>
        /// <returns></returns>
        public static string GetParamSrc(Dictionary<string, string> paramsMap)
        {

            var vDic = paramsMap.OrderBy(x => x.Key, new ComparerString()).ToDictionary(x => x.Key, y => y.Value);

            StringBuilder str = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                str.Append(pkey + "=" + pvalue + "&");
            }
            string result = str.ToString().Substring(0, str.ToString().Length - 1);

            return result;
        }
        public class ComparerString : IComparer<String>
        {
            public int Compare(String x, String y)
            {
                return string.CompareOrdinal(x, y);
            }
        }

    }
}