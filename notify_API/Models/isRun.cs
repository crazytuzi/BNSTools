using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class isRun
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        public string Acction { get; set; }
        public string DisName { get; set; }
        public int itemID { get; set; }
        public int Count { get; set; }
        public int IsRun { get; set; }
    }
}