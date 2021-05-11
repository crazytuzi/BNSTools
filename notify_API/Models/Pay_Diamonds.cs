using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class Pay_Diamonds
    {
        public string GUID { get; set; }
        public string UserName { get; set; }
        public long AddDiamonds { get; set; }
        public string Type { get; set; }
        public DateTime Timer { get; set; }
    }
}