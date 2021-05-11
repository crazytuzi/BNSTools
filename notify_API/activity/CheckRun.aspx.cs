using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API.activity
{
    public partial class CheckRun : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Tools.Time = 3;
            Tools.CheckIsRun();
            if (Tools.IsCheckRun)
            {
                return_result = "OK";
            }
            else
            {
                return_result = "Error";
            }
            //try
            //{
            //    if (!Tools.IsCheckRun)
            //    {
            //        Tools.IsCheckRun = true;//锁定 防止重复执行
            //        //执行
            //        List<isRun> run_List = AchiDB.db.Queryable<isRun>().Where(it => it.IsRun == false).ToList();//获取所有未发送的
                    
            //        for (int i = 0; i < run_List.Count; i++)
            //        {
            //            //执行发送
            //            Dictionary<string, string> get_dic = new Dictionary<string, string>();
            //            get_dic.Add("Acction", run_List[i].Acction);
            //            get_dic.Add("ItemID", $"{run_List[i].itemID}");
            //            get_dic.Add("Count", $"{run_List[i].Count}");
            //            string url = Tools.Get("http://127.0.0.1:8012/SendItem.aspx", get_dic);
            //            if (url.Contains("Success"))
            //            {
            //                run_List[i].IsRun = true;
            //                AchiDB.db.Updateable(run_List[i]).UpdateColumns(it => new { it.IsRun }).ExecuteCommand();
            //            }
            //            Thread.Sleep(200);//间隔0.2秒
            //        }
            //        return_result = "OK";
            //    }
            //    else
            //    {
            //        return_result = "OK";
            //    }
            //}
            //finally 
            //{//运行结束 或错误的结束 解除锁定
            //    Tools.IsCheckRun = false;
            //}
        }
    }
}