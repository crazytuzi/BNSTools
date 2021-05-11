using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class GralData_Entity
    {
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// Item道具
        /// </summary>
        public int Item_ID { get; set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 道具名称
        /// </summary>
        public string Item_Name { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int jf { get; set; }
    }
}
