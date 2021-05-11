using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Locker.Models
{
    public class Login_Msg
    {
        public string Mac { get; set; }
        public DateTime Timer { get; set; }
    }
}