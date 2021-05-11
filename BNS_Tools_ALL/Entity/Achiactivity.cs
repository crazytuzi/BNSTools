using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BNS_Tools_ALL.Entity
{
    public class Achiactivity
    {
        public string GUID { get; set; }
        public string msg { get; set; }
        public DateTime StartTimer { get; set; }
        public DateTime DueTimer { get; set; }
        public string Data { get; set; }
    }
}