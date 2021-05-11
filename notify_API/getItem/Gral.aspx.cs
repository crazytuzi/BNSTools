using Newtonsoft.Json;
using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.getItem
{
    public partial class Gral : System.Web.UI.Page
    {
        public string return_result = "";
        public string getPostStr()
        {
            int intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);
            return Encoding.UTF8.GetString(b);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var json = getPostStr();
            if (json != "")
            {
                var dat = new { GUID = "", DrawGUID = "", Acction = "" };
                dat = JsonConvert.DeserializeAnonymousType(json, dat);//解析至匿名对象
                string GUID = dat.GUID;
                string DrawGUID = dat.DrawGUID;
                string Acction = dat.Acction;

                string sqlstr = $@"select [GralData] from [AchiDB].dbo.[Draw] where [GUID] = '{DrawGUID}'  and [Start_Timer] < '{DateTime.Now}' and [Due_Timer] > '{DateTime.Now}'";//获取活动的兑换内容
                string JsonData = DBHelper.SelectScalar(sqlstr).ToString();
                bool dh_mode = Convert.ToBoolean(ConfigurationManager.ConnectionStrings["cfdh"].ConnectionString == null ? "true": ConfigurationManager.ConnectionStrings["cfdh"].ConnectionString);
                if (JsonData != null && JsonData != "")
                {
                    sqlstr = $@"select count(*) from [AchiDB].dbo.[GetGral] where [DrawGUID] = '{DrawGUID}' and [ItemGUID] = '{GUID}'";
                    int count = Convert.ToInt32(DBHelper.SelectScalar(sqlstr));
                    if (count == 0 || dh_mode == false)
                    {
                        var _data = new { GralData = new[] { new { GUID = "", Item_ID = 0, Count = 0, Item_Name = "", jf = 0 } }, xh = false };//解析兑换内容
                        _data = JsonConvert.DeserializeAnonymousType(JsonData, _data);//解析至匿名对象

                        bool xh = _data.xh;
                        var data = new { GUID = "", Item_ID = 0, Count = 0, Item_Name = "", jf = 0 };
                        foreach (var item in _data.GralData)//从库中获取到兑换的道具
                        {
                            if (item.GUID == GUID)
                            {
                                data = item;
                                break;
                            }
                            else
                                continue;
                        }

                        if (data != null)
                        {

                            int Balance = 0, Re_Balance = 0;

                            sqlstr = $@"select isNULL(sum(Balance),0) as Balance ,isNULL(sum(Re_Balance),0) as Re_Balance from [AchiDB].dbo.[GralCount] where [DrawGUID] = '{DrawGUID}' and [Acction] = '{Acction}'";
                            using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                            {
                                if (sdr.Read())
                                {
                                    Balance = Convert.ToInt32(sdr["Balance"]);
                                    Re_Balance = Convert.ToInt32(sdr["Re_Balance"]);
                                }
                            }
                            
                            if (Balance > data.jf)
                            {
                                if (!dh_mode)
                                {
                                    AchiDB.db.Deleteable<GralCount>(it => it.Acction == dat.Acction).Where(it => it.DrawGUID == dat.DrawGUID).ExecuteCommand();

                                    AchiDB.db.Insertable(new GralCount()
                                    {
                                        DrawGUID = dat.DrawGUID,
                                        Acction = dat.Acction,
                                        Balance = Balance - data.jf,
                                        Re_Balance = Re_Balance,
                                        ChangedTimer = DateTime.Now
                                    }).ExecuteCommand();
                                }

                                isRun itemrun = new isRun()//发送道具
                                {
                                    Acction = Acction,
                                    Count = data.Count,
                                    itemID = data.Item_ID,
                                    DisName = $"{data.Item_Name}",
                                    GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                    IsRun = 0
                                };

                                
                                GetGral Gral = new GetGral()//积分兑换记录
                                {
                                    Acction = Acction,
                                    Count = data.Count,
                                    DrawGUID = DrawGUID,
                                    ItemGUID = data.GUID,
                                    ItemID = data.Item_ID,
                                    ItemName = $"{data.Item_Name}X{data.Count}",
                                    NewTimer = DateTime.Now
                                };

                                Record Recor = new Record()//领取记录
                                {
                                    Acction = Acction,
                                    GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                    AchiactivityGUID = DrawGUID,
                                    ItemID = data.Item_ID,
                                    ItemName = $"{data.Item_Name}X{data.Count}",
                                    ChangedTimer = DateTime.Now,
                                    PCID = 0
                                };

                                AchiDB.db.Insertable(itemrun).ExecuteCommand();
                                AchiDB.db.Insertable(Gral).ExecuteCommand();
                                AchiDB.db.Insertable(Recor).ExecuteCommand();

                                Task.Run(() =>
                                {
                                    //异步通知发送 请求检索
                                    Tools.Get("http://127.0.0.1:8012/activity/CheckRun.aspx", new Dictionary<string, string>());
                                });
                                
                                return_result = JsonConvert.SerializeObject(new { Code = 0, msg = "兑换成功！" });
                            }
                            else
                                return_result = JsonConvert.SerializeObject(new { Code = 3, msg = "积分不足" });
                        }
                        else
                            return_result = JsonConvert.SerializeObject(new { Code = 2, msg = "无法找到此道具" });

                    }
                    else
                    {
                        return_result = JsonConvert.SerializeObject(new { Code = 1, msg = "此道具已领取" });
                    }

                    
                }
                else
                {
                    return_result = JsonConvert.SerializeObject(new { Code = 2, msg = "无法找到此道具" });
                }
                
            }
            else
            {
                return_result = JsonConvert.SerializeObject(new { Code = 4, msg = "系统出错！无法兑换" });
            }
        }
    }
}