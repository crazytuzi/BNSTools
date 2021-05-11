using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class Draw_Entity
    {

        /// <summary>
        /// 唯一ID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 抽奖活动名称
        /// </summary>
        public string DrawName { get; set; }

        /// <summary>
        /// 积分兑换奖池
        /// </summary>
        public string GralData { get; set; }

        /// <summary>
        /// 附带信息
        /// </summary>
        public string F_Msg { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start_Timer { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime Due_Timer { get; set; }
    }
}
