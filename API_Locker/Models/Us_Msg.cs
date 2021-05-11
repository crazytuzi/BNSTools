using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Locker.Models
{

    public class Us_Msg
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ID { get; set; }
        public string Mac { get; set; }
        public bool IsBan { get; set; }
        public string u_Type { get; set; }
    }
}