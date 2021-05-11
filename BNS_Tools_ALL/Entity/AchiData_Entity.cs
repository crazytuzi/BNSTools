using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class AchiData_Entity
    {
        public List<Achi> Achi { get; set; }
        public List<Itemdatas> item { get; set; }
    }
    public class Achi 
    {
        public string AchiKey { get; set; }
        public int AchiValue { get; set; }
    }
    public class Itemdatas
    {
        public int itemid { get; set; }
        public int Count { get; set; }
        public string DisName { get; set; }
    }
}
