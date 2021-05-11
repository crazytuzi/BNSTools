using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class Draw_Data_Entity
    {
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 奖池名字
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 活动GUID
        /// </summary>
        public string DrawGUID { get; set; }
        /// <summary>
        /// 奖池内容
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 单抽小号
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 11连抽消耗
        /// </summary>
        public int DoubleNumber { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ChangedTimer { get; set; }
    }
}
