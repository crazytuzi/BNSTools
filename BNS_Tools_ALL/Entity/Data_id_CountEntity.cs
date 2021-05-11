using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class Data_id_CountEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 道具ID
        /// </summary>
        public int data_id { get; set; }

        /// <summary>
        /// 道具名称
        /// </summary>
        public string Data_Name { get; set; }

        /// <summary>
        /// 道具经验
        /// </summary>
        public int exp { get; set; }

        /// <summary>
        /// 道具等级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 道具叠堆数量
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 道具类型
        /// </summary>
        public string types { get; set; }

        /// <summary>
        /// 幻化道具名
        /// </summary>
        public string app_item_data_Name { get; set; }
        /// <summary>
        /// 幻化道具ID
        /// </summary>
        public string app_item_data_ID { get; set; }
        /// <summary>
        /// 宝石槽1
        /// </summary>
        public string gem_1 { get; set; }
        /// <summary>
        /// 宝石槽2
        /// </summary>
        public string gem_2 { get; set; }
        /// <summary>
        /// 宝石槽3
        /// </summary>
        public string gem_3 { get; set; }
        /// <summary>
        /// 宝石槽4
        /// </summary>
        public string gem_4 { get; set; }
        /// <summary>
        /// 宝石槽5
        /// </summary>
        public string gem_5 { get; set; }
        /// <summary>
        /// 宝石槽6
        /// </summary>
        public string gem_6 { get; set; }
    }
    
}
