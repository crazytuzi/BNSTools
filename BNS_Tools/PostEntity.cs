using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS_Tools
{
    public class PostEntity
    {
        public string PostID { get; set; }
        public string dataID { get; set; }
        public string Amount { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public string ReceiptTime { get; set; }
        public string SendAcction { get; set; }
        public string RecipientAcction { get; set; }
    }
}
