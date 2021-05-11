using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class GetGral
    {
        public string DrawGUID { get; set; }
        public string ItemGUID { get; set; }
        public string Acction { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; }
        public DateTime NewTimer { get; set; }
    }
}