using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class Record
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        public string AchiactivityGUID { get; set; }
        public string Acction { get; set; }
        public int PCID { get; set; }
        public string ItemName { get; set; }
        public int ItemID { get; set; }
        public DateTime ChangedTimer { get; set; }
    }
}