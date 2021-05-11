using API_Locker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Locker.Controllers
{
    public class GetUserController : ApiController
    {
        public string Get(string Mac)
        {
            
            List<Us_Msg> list = DBsql.db.Queryable<Us_Msg>().Where(it=>it.Mac == Mac).ToList();
            if (list.Count > 0)
            {
                if (list[0].IsBan == true)
                {
                    return "IsBan";
                }
                else 
                {
                    return "Login_Success";
                }
                    

            }

            try
            {
                DBsql.db.Insertable(new Login_Msg()
                {
                    Mac = Mac,
                    Timer = DateTime.Now
                }).ExecuteCommand();
            }
            catch
            {

            }

            return "Not_User";
        }
    }
}