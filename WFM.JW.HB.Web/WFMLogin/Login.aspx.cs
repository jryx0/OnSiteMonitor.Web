using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace WFM.JW.HB.Web.WFMLogin
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
           // RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (Membership.ValidateUser(userName.Text, password.Text))
            {
                // Log the user into the site               
                FormsAuthentication.RedirectFromLoginPage(userName.Text, false);
            }
            // If we reach here, the user's credentials were invalid
            ErrorMsg.Text = "用户名或密码错误，请重新登录！";
        }
    }
}