using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class Ping : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            return_result = "Success";
        }
    }
}