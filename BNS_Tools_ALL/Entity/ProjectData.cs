using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class ProjectData
    {
        public int PCID { get; set; }
        public string GameAccountID { get; set; }
        public string SlotID { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public double exp { get; set; }
        public long money { get; set; }
        public long Balance { get; set; }
        public long mastery_exp { get; set; }
        public long faction_reputation { get; set; }
        public long hp { get; set; }
        public string IsBan { get; set; }
        public string UserName { get; set; }
    }
}
