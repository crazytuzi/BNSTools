using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class ChrProject
    {
        /// <summary>
        /// PCID
        /// </summary>
        public int PCID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 势力阵营
        /// </summary>
        public string Camp { get; set; }

    }
}