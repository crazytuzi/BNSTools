using API_Locker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Locker.Controllers
{
    public class UserMsgAddController : ApiController
    {
        public void Get(string str,string IP,string Mac)
        {
            try
            {
                if (!IP.Contains("192.168"))
                {
                    List<ALLconnect> list = DBsql.db.Queryable<ALLconnect>().Where(it => it.Mac == Mac).ToList();

                    if (list.Count > 0)
                    {
                        DBsql.db.Updateable(new ALLconnect()
                        {
                            Mac = Mac,
                            IP = IP,
                            STR = str,
                            Timer = DateTime.Now
                        }).WhereColumns(it => new { it.Mac }).ExecuteCommand();
                    }
                    else
                    {
                        DBsql.db.Insertable(new ALLconnect()
                        {
                            Mac = Mac,
                            IP = IP,
                            STR = str,
                            Timer = DateTime.Now
                        }).ExecuteCommand();
                    }
                }
                

                
            }
            catch
            {

            }
        }
    }
}