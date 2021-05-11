using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class DrawBalance
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        public string DrawGUID { get; set; }
        public string Acction { get; set; }
        public int Balance { get; set; }
        public DateTime ChangedTimer { get; set; }
    }
}