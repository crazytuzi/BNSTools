using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Locker.Models
{
    public class Us_Help
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ID { get; set; }
        public string U_Mac { get; set; }
        public string U_TXT { get; set; }
        public string Server_IP { get; set; }
        public string Server_Acction { get; set; }
        public string Server_Password { get; set; }
        public string U_TEL { get; set; }
    }
}