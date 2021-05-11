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
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.activity
{
    public partial class Achievement : System.Web.UI.Page
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
            string js = getPostStr();
            var jsdata = new { Acction = "", PCID = 0, GUID = "" };
            jsdata = JsonConvert.DeserializeAnonymousType(js, jsdata);//解析至匿名对象
            if (jsdata != null)
            {
                // Acction PCID GUID
                string Acction = jsdata.Acction;
                int PCID = jsdata.PCID;
                string GUID = jsdata.GUID;
                List<Record> Record_List = AchiDB.db.Queryable<Record>().Where(it => it.AchiactivityGUID == GUID).Where(it => it.Acction == Acction).ToList();
                if (Record_List.Count == 0)
                {//此活动未领取过
                    List<Achiactivity> Achi_List = AchiDB.db.Queryable<Achiactivity>().Where(it => it.GUID == GUID).Where(it => it.StartTimer < DateTime.Now).Where(it => it.DueTimer > DateTime.Now).ToList();
                    if (Achi_List.Count == 1)//如果活动存在
                    {
                        string json = Achi_List[0].Data;//获取数据结构体
                        var data = new { Achi = new[] { new { AchiKey = "", Achivalue = 0 } }, item = new[] { new { DisName = "",itemid = 0, Count = 0 } } };//匿名对象
                        data = JsonConvert.DeserializeAnonymousType(json, data);//解析至匿名对象
                        string tj = string.Empty;

                        int count = 0;//查询是否满足
                        foreach (var item in data.Achi)
                        {
                            string sql = $"select count(*) from [AchievementDB].dbo.[Register] where [pcid] = '{PCID}' and [RegisterId] = '{item.AchiKey}' and [RegisterValue] >= {item.Achivalue}";
                            count += Convert.ToInt32(DBHelper.SelectScalar(sql));
                        }
                        
                        

                        if (count >= data.Achi.Length)
                        {
                            //满足 进行发送 异步操作

                            foreach (var item in data.item)
                            {
                                isRun itemrun = new isRun()
                                {
                                    Acction = Acction,
                                    Count = item.Count,
                                    itemID = item.itemid,
                                    DisName = item.DisName,
                                    GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                    IsRun = 0
                                };
                                AchiDB.db.Insertable(itemrun).ExecuteCommand();

                                Record Recor = new Record()
                                {
                                    Acction = Acction,
                                    GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                    AchiactivityGUID = GUID,
                                    ItemID = item.itemid,
                                    ItemName = item.DisName,
                                    ChangedTimer = DateTime.Now,
                                    PCID = PCID
                                };

                                AchiDB.db.Insertable(Recor).ExecuteCommand();
                                //插入库中
                            }

                            Task.Run(() => 
                            {
                                //异步通知发送
                                Tools.Get("http://127.0.0.1:8012/activity/CheckRun.aspx", new Dictionary<string, string>());
                            });
                            return_result = "OK";
                        }
                        else
                            return_result = "Unqualified";//不满足活动要求
                    }
                    else
                        //无此活动 或此活动已到期
                        return_result = "Not activity";
                }
                else
                {
                    //已领取过
                    return_result = "Already";
                }
            }
            else
            {
                //参数不正确
                return_result = "API Code Error";
            }
            

        }

    }
}