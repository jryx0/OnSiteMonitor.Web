using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFM.JW.HB.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        public String  CurrentRegion
        {
            set { this.CurrentRegionLabel.Text = value; }
            get { return this.CurrentRegionLabel.Text; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
             
        }
    }
}
