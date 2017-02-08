using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFM.JW.HB.Web.WFMLogin
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request["Username"];
            if (username != null)
            {
                // web service is requesting authentication via a HTTP request
                // if (FormsAuthentication.Authenticate(Request["Username"], Request["Password"]))
                if (Membership.ValidateUser(username, Request["Password"]))
                {
                    FormsAuthentication.SetAuthCookie(username, false);

                    //var fh = new UserInfoHelper();
                    ////fh.GetUserRegion(username) + "&" + fh.GenerateRegionPath(username);

                    //Session[username] = fh.GetRegionInof(username);
                }
            }
        }
    }
}