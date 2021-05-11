using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Store_Tools.Entity
{
    public class Goods_Data
    {
        /// <summary>
        /// GoodsID
        /// </summary>
        public int GoodsId { get; set; }
        /// <summary>
        /// ItemID
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 道具名称
        /// </summary>
        public string GoodsDisplayName { get; set; }
        /// <summary>
        /// 道具价格
        /// </summary>
        public double BasicSalePrice { get; set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int ItemQuantity { get; set; }
        /// <summary>
        /// 道具上架页ID
        /// </summary>
        public string CategoryId { get; set; }
    }
}
