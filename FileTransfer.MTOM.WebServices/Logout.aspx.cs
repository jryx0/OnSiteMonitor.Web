using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FileTransfer.MTOM.WebServices
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String username = Request["Username"];
            if (username != null)
            {
                if (Membership.ValidateUser(username, Request["Password"]))
                {
                    FormsAuthentication.SignOut();
                    Session.Clear();
                }
            }
        }
    }
}