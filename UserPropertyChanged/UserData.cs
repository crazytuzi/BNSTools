using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPropertyChanged
{
    public class UserData
    {
        /// <summary>
        /// 角色PCID
        /// </summary>
        public int PCID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 洪门等级
        /// </summary>
        public int master_level { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int data_id { get; set; }
        /// <summary>
        /// 任务完成度
        /// </summary>
        public int current_mission_step { get; set; }
    }
}
