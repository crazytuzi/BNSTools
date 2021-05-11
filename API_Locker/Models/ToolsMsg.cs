using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Locker.Models
{
    public class ToolsMsg
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string BoxMsg { get; set; }
        public bool isShow { get; set; }
    }
}