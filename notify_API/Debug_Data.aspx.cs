using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class Debug_Data : System.Web.UI.Page
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
            return_result = "success";
        }
    }
}