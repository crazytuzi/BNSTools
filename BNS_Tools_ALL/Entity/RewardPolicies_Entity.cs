using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools_ALL.Entity
{
    public class RewardPolicies_Entity
    {
        public int RewardPolicyId { get; set; }
        public int PromotionId { get; set; }
        public int ItemId { get; set; }
        public int RewardTargetKey { get; set; }
        public int BoardConditionValue { get; set; }
        public string BoarName { get; set; }
        public string Key { get; set; }
        public int Data_ID { get; set; }
        public int Count { get; set; }
    }
}
