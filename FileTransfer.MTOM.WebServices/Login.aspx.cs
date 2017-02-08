using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;

namespace FileTransfer.MTOM.WebServices
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String username = Request["Username"];
            if (username != null)
            {
                // web service is requesting authentication via a HTTP request
                // if (FormsAuthentication.Authenticate(Request["Username"], Request["Password"]))
               // if (Membership.ValidateUser(username, Request["Password"]))
               if(username == Request["Password"])
                { 
                    FormsAuthentication.SetAuthCookie(username, false);

                   // var fh = new UserInfoHelper();
                    //fh.GetUserRegion(username) + "&" + fh.GenerateRegionPath(username);

                   // Session[username] = fh.GetRegionInof(username);
                }
            }
        }
    }
}