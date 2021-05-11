using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class SuccessPay
    {
        public string GUID { get; set; }
        public string UserName { get; set; }
        public decimal money { get; set; }
        public string trade_no { get; set; }
        public DateTime Timer { get; set; }
        public bool Status { get; set; }
    }
}