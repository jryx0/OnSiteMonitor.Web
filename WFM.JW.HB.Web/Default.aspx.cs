using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFM.JW.HB.Web
{
    public partial class _Default : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/WFMClues/ClueReportRegion.aspx");
            //if (!IsPostBack)
            //{
            //    Services.ProjectInfoServices service = new Services.ProjectInfoServices();
            //    var list = service.GetAllData(null);

            //    ProjectNumber.Text = "项目总数：" + list.Count().ToString();
            //    ContractNumber.Text = "合同总数：" + list.Sum(c => c.ContractsNumbers);

            //    var value = list.Sum(v => v.ContractsTotalValue);
            //    var settle = list.Sum(s => s.ContractsTotalSettle);
            //    ContractValue.Text = "合同总金额：" + String.Format("{0:C2}", value) + " 元";
            //    ContractSettlement.Text = "已结算总金额：" + String.Format("{0:C2}", settle) + " 元";

            //}

        }
    }
}
