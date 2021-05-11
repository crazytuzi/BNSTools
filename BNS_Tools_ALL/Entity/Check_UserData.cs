using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class Check_UserData
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
        /// 角色金钱
        /// </summary>
        public decimal money { get; set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 洪门等级
        /// </summary>
        public int mastery_level { get; set; }

        /// <summary>
        /// 角色经验
        /// </summary>
        public long exp { get; set; }

        /// <summary>
        /// 洪门经验
        /// </summary>
        public long mastery_exp { get; set; }

        /// <summary>
        /// 势力经验
        /// </summary>
        public long faction_reputation { get; set; }

        /// <summary>
        /// 所属账号
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 账号UID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 点券余额
        /// </summary>
        public long Balance { get; set; }

        /// <summary>
        /// 召唤兽名称
        /// </summary>
        public string SummName { get; set; }
        /// <summary>
        /// 仙豆余额
        /// </summary>
        public int duel { get; set; }
        /// <summary>
        /// 势力
        /// </summary>
        public string faction { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public int Job { get; set; }
    }
}
