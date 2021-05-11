using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class Achiactivity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        public string msg { get; set; }
        public DateTime StartTimer { get; set; }
        public DateTime DueTimer { get; set; }
        public string Data { get; set; }
    }
}