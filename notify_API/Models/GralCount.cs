using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class GralCount
    {
        /// <summary>
        /// 活动GUID
        /// </summary>
        public string DrawGUID { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Acction { get; set; }
        /// <summary>
        /// 所剩积分
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 累计获得积分
        /// </summary>
        public int Re_Balance { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ChangedTimer { get; set; }
    }
}