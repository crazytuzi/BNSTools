using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public class notify_data
    {
        public string GUID { get; set; }
        public string Data { get; set; }
        public DateTime timer { get; set; }
    }
}