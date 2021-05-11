using API_Locker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Locker.Controllers
{
    public class GetMsgController : ApiController
    {
        public string Get()
        {
            List<ToolsMsg> list = DBsql.db.Queryable<ToolsMsg>().Where(it=> it.isShow == true).ToList();
            if (list.Count > 0)
            {
               return $"{list[0].Title}||{list[0].BoxMsg}";
            }
            return "Not_Msg";
        }
    }
}