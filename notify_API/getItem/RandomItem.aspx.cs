using Newtonsoft.Json;
using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.getItem
{
    public partial class RandomItem : System.Web.UI.Page
    {
        public string return_result = "";
        int count = 0;
        List<GetItem_Entity> list = new List<GetItem_Entity>();
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
            var js_dat = new { GUID = "", DrawGUID = "", Acction = "", Number = 0 };//GUID是奖池GUID GralGUID为活动GUID Acction为账号
            
            if (json != "")
            {
                js_dat = JsonConvert.DeserializeAnonymousType(json, js_dat);//解析至匿名对象
                var js = ""; 
                int number = 0, doublenumber = 0;
                string sqlstr = $@"select top 1 a.[Data],a.[Number],a.[DoubleNumber] from [AchiDB].dbo.[Draw_Data] as a 
inner join [AchiDB].dbo.[Draw] as b on a.[DrawGUID] = b.[GUID] 
where a.[GUID] = '{js_dat.GUID}' and b.[GUID] = '{js_dat.DrawGUID}'  and b.[Start_Timer] < '{DateTime.Now}' and b.[Due_Timer] > '{DateTime.Now}'";//获取奖池内容与需要的余额

                using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
                {
                    if (sdr.Read())
                    {
                        js = sdr["Data"].ToString();
                        number = Convert.ToInt32(sdr["Number"]);
                        doublenumber = Convert.ToInt32(sdr["DoubleNumber"]);
                    }
                    sdr.Close();
                }
                sqlstr = $@"select isNULL(sum(Balance),0) as Balance from [AchiDB].dbo.[DrawBalance] where [DrawGUID] = '{js_dat.DrawGUID}' and [Acction] = '{js_dat.Acction}'";//获取余额
                int bal = Convert.ToInt32(DBHelper.SelectScalar(sqlstr));
                bool BalSuccess = false;

                if (js_dat.Number == 1)
                {
                    if (bal >= number)
                    {
                        BalSuccess = true;
                        bal -= number;
                    }
                }
                else if (js_dat.Number == 11)
                {
                    if (bal >= doublenumber)
                    {
                        BalSuccess = true;
                        bal -= doublenumber;
                    }
                }

                if (BalSuccess)//如果余额充足
                {
                    var data = new { Data = new[] { new { ItemID = 0, ItemName = "", ItemCount = 0, Itemgl = 0.00F, jf = 0 } } };//默认0号奖池 无上线 jf为可获得积分
                    data = JsonConvert.DeserializeAnonymousType(js, data);//解析至匿名对象

                    count = 0;//抽奖空间计数
                    list = new List<GetItem_Entity>();

                    foreach (var item in data.Data)
                    {
                        GetItem_Entity T = new GetItem_Entity()
                        {
                            Count = item.ItemCount,
                            Itemgl = Convert.ToInt32(item.Itemgl * 1000),
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            jf = item.jf
                        };
                        list.Add(T);
                        count += Convert.ToInt32(item.Itemgl * 1000);
                    }

                    list = ListRandom(list);//打乱顺序

                    sqlstr = $@"            
begin tran
begin try
delete [AchiDB].dbo.[DrawBalance] where [DrawGUID] = '{js_dat.DrawGUID}' and [Acction] = '{js_dat.Acction}';
insert into [AchiDB].dbo.[DrawBalance]([GUID],[DrawGUID],[Acction],[Balance],[ChangedTimer]) values('{Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff"))}','{js_dat.DrawGUID}','{js_dat.Acction}','{bal}','{DateTime.Now}');
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
";
                    if (DBHelper.IDU(sqlstr) == 2)
                    {
                        List<GetItem_Entity> _list = new List<GetItem_Entity>();
                        string str = string.Empty;
                        for (int i = 0; i < js_dat.Number; i++)//抽取指定次数
                        {
                            _list.Add(GetItems());//循环抽奖

                            AchiDB.db.Insertable(new Record()
                            {
                                GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                ItemID = _list[i].ItemID,
                                ItemName = $"{_list[i].ItemName}X{_list[i].Count}",
                                Acction = js_dat.Acction,
                                PCID = 0,
                                ChangedTimer = DateTime.Now,
                                AchiactivityGUID = js_dat.DrawGUID
                            }).ExecuteCommand();


                            if (_list[i].ItemID != 0)//如果道具ID为0 不添加到发送表
                            {
                                AchiDB.db.Insertable(new isRun()
                                {
                                    Acction = js_dat.Acction,
                                    Count = _list[i].Count,
                                    DisName = _list[i].ItemName,
                                    IsRun = 0,
                                    itemID = _list[i].ItemID,
                                    GUID = Tools.MD5Encrypt(Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff:ffffff")),
                                }).ExecuteCommand();
                            }

                            if (_list[i].jf > 0)//存在积分
                            {
                                AchiDB.db.Insertable(new GralCount()
                                {
                                    DrawGUID = js_dat.DrawGUID,
                                    Acction = js_dat.Acction,
                                    Balance = _list[i].jf,
                                    Re_Balance = _list[i].jf,
                                    ChangedTimer = DateTime.Now
                                }).ExecuteCommand();
                            }
                            
                            str += $"[{_list[i].ItemName}X{_list[i].Count}],";


                            Thread.Sleep(20);
                        }
                        Task.Run(() =>
                        {
                            //异步通知发送 请求检索
                            Tools.Get("http://127.0.0.1:8012/activity/CheckRun.aspx", new Dictionary<string, string>());
                        });
                        str = str.Substring(0, str.Length - 1);
                        return_result = JsonConvert.SerializeObject(new { Code = 0, Msg = str });
                    }
                    else
                    {
                        return_result = JsonConvert.SerializeObject(new { Code = 1, Msg = "余额支付失败,请重试！" });
                    }
                }
                else
                {
                    return_result = JsonConvert.SerializeObject(new { Code = 1, Msg = "余额不足！" });
                }

                
            }

            
        }
        private GetItem_Entity GetItems()
        {
            GetItem_Entity T = null;
            if (list.Count > 0)
            {
                Random x = new Random();
                int num = x.Next(1,count + 1);
                int jpnum = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (jpnum < num && num <= list[i].Itemgl + jpnum)
                    {
                        T = list[i];
                        return T;
                    }
                    else
                    
                        jpnum += list[i].Itemgl;
                    
                }
            }
            return T;
                
        }
        public static List<T> ListRandom<T>(List<T> sources)
        {
            try
            {
                var random = new Random();
                var resultList = new List<T>();
                foreach (var item in sources)
                {
                    resultList.Insert(random.Next(resultList.Count), item);
                }
                return resultList;
            }
            catch 
            {
                return sources;
            }
        }
    }
}