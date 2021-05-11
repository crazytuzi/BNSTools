using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools
{
    public class Announce_Entity
    {
        public int registerID { get; set; }
        public int godSayType { get; set; }
        public int godSayPosition { get; set; }
        public string worlds { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int dayInterval { get; set; }
        public int minInterval { get; set; }
        public int repeatCount { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string godSayMessage { get; set; }
    }
}
