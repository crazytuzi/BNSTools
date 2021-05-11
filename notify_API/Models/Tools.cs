using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace notify_API.Models
{
    public static class Tools
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="dic">请求参数定义</param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, string> dic)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            //添加参数
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
                resp.Close();
            }
            return result;
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
        /// 静态 是否正在执行
        /// </summary>
        public static bool IsCheckRun = false;
        public static int Time = 60 * 30;//间隔秒数 1分钟*30  半小时执行一次
        public static void CheckIsRun()
        {
            if (!IsCheckRun)
            {
                IsCheckRun = true;//锁定 正在执行

                Task.Run(() =>
                {
                    while (true)//不停止
                    {
                        if (Time == 0)
                        {
                            try
                            {
                                List<isRun> run_List = AchiDB.db.Queryable<isRun>().Where(it => it.IsRun == 0).ToList();//获取所有未发送的

                                for (int i = 0; i < run_List.Count; i++)
                                {
                                    //执行发送
                                    Dictionary<string, string> get_dic = new Dictionary<string, string>();
                                    get_dic.Add("Acction", run_List[i].Acction);
                                    get_dic.Add("ItemID", $"{run_List[i].itemID}");
                                    get_dic.Add("Count", $"{run_List[i].Count}");
                                    get_dic.Add("Pwd", $"BNSServerP");
                                    string url = Tools.Get("http://127.0.0.1:8012/SendItem.aspx", get_dic);
                                    if (url.Contains("Success"))//发送成功
                                    {
                                        run_List[i].IsRun = 1;
                                        AchiDB.db.Updateable(run_List[i]).UpdateColumns(it => new { it.IsRun }).ExecuteCommand();//修改库 标志成功
                                    }
                                    Thread.Sleep(200);//间隔0.2秒
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            Time = 60 * 30;
                        }
                        else
                        {
                            Time--;
                            Thread.Sleep(1000);//1秒执行一次
                        }
                    }
                });
            }
            
            
        }

    }
}
