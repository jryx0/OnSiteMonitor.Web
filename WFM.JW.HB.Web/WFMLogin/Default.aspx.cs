using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFM.JW.HB.Web.WFMLogin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LinkButton1_Command(object sender, CommandEventArgs e)
        {
            Services.ContractServices contract = new Services.ContractServices();
            var list = contract.GetAllData();

            ContractGrid.DataSource = from l in list
                                      select new { l.ContractID, l.ContractName, l.ContractCode };
            ContractGrid.DataBind();
        }
    }
}