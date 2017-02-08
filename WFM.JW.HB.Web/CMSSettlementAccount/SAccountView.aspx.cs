using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;


namespace WFM.JW.HB.Web.CMSSettlementAccount
{
    public partial class SAccountView : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            this.Pager.GoPage += new GoPageEventHandler(GoPage);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                InitControls();
            }
        }

        private void InitControls()
        {
            //初始化控件
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();
            ////类型
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "合同类别");
            spec = spec.Or(new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "履约情况"));
            spec = spec.And(new Models.CriteriaSpecification("A.[enable]", Models.CriteriaOperator.Equal, true));

            var list = service.GetAllData(spec);

            DropDownListContractType.DataSource = from t in list
                                                  where t.ItemParent.BaseTypeName == "合同类别"
                                                  orderby t.ItemOrder
                                                  select t;
            DropDownListContractType.DataValueField = "BaseTypeItemID";
            DropDownListContractType.DataTextField = "ItemName";
            DropDownListContractType.DataBind();

            DropDownListContractType.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            //初始化GridView
            //InitGridView(null);
            GoPage(1);
        }

        private void InitGridView(Models.IParametersSpecification spec)
        {
            Services.SAccountServices service = new Services.SAccountServices();
            var list = service.GetAllData(spec, CheckBoxAllContract.Checked);

            CLGridViewSAccount.DataSource = list;
            CLGridViewSAccount.DataBind();
        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            if (this.TextBoxProject.Text.Length != 0)
            {
                spec = spec.And(new Models.CriteriaSpecification("B.ProjectName", Models.CriteriaOperator.Like, TextBoxProject.Text));
            }

            //if (this.TextBoxContractName.Text.Length != 0)
            //{
            //    spec = spec.And(new Models.CriteriaSpecification("A.ContractName", Models.CriteriaOperator.Like, TextBoxContractName.Text));
            //}


            if (DropDownListContractType.SelectedValue.CanConvertTo<int>())
                spec = spec.And(new Models.CriteriaSpecification("A.ContractTypeID", Models.CriteriaOperator.Equal, DropDownListContractType.SelectedValue.ConvertTo<int>()));


            //dateTime
            if (TextBoxStartDate.Value.Length != 0 && TextBoxStartDate.Value.CanConvertTo<DateTime>())
            {
                spec = spec.And(new Models.CriteriaSpecification("A.ContractDate", Models.CriteriaOperator.GreaterThanOrEqual, TextBoxStartDate.Value.ConvertTo<DateTime>()));
            }

            if (TextBoxEndDate.Value.Length != 0 && TextBoxEndDate.Value.CanConvertTo<DateTime>())
            {
                spec = spec.And(new Models.CriteriaSpecification("A.ContractDate", Models.CriteriaOperator.LessThanOrEqual, TextBoxEndDate.Value.ConvertTo<DateTime>()));

            }


            return spec;
        }


        private void GoPage(int CurrentPage)
        {
            var ps = this.Pager.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewSAccount.PageSize);
                this.Pager.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.SAccountServices service = new Services.SAccountServices();


           // ps.TotalCount = service.AccountCount(spec, CheckBoxAllContract.Checked);
            var ds = service.AcountGroupData(spec, CheckBoxAllContract.Checked,
                                                     @"COUNT(DISTINCT g.SettlementInfoID), 
                                                       COUNT(DISTINCT A.ContractID),  
                                                       SUM(DISTINCT a.ContractValue),
                                                       SUM(DISTINCT a.ContractSettle),
                                                       SUM(g.AmountDeclared),
                                                       SUM(g.AmountAudited) ,        
                                                       SUM(g.AuditDeduction)");
            try
            {
                ps.TotalCount = (int)ds.Tables[0].Rows[0][0]; //TotalNumber 

                TotalSettlementNumber.Text = ds.Tables[0].Rows[0][0].ToString();
                ContractTotalNumber.Text = ds.Tables[0].Rows[0][1].ToString();                
                ContractTotalValue.Text = ds.Tables[0].Rows[0][2].ToString();
                if (ContractTotalValue.Text.CanConvertTo<double>())
                    ContractTotalValue.Text = ContractTotalValue.Text.ConvertTo<double>().ToString("C");
                else ContractTotalValue.Text = "￥0.00";

                ContractTotalSettmentValue.Text = ds.Tables[0].Rows[0][3].ToString();
                if (ContractTotalSettmentValue.Text.CanConvertTo<double>())
                    ContractTotalSettmentValue.Text = ContractTotalSettmentValue.Text.ConvertTo<double>().ToString("C");
                else ContractTotalSettmentValue.Text = "￥0.00";


                ContractTotalSettmentDeclare.Text = ds.Tables[0].Rows[0][4].ToString();
                if (ContractTotalSettmentDeclare.Text.CanConvertTo<double>())
                    ContractTotalSettmentDeclare.Text = ContractTotalSettmentDeclare.Text.ConvertTo<double>().ToString("C");
                else ContractTotalSettmentDeclare.Text = "￥0.00";

                TotalDeduceValue.Text = ds.Tables[0].Rows[0][6].ToString();
                if (TotalDeduceValue.Text.CanConvertTo<double>())
                    TotalDeduceValue.Text = TotalDeduceValue.Text.ConvertTo<double>().ToString("C");
                else TotalDeduceValue.Text = "￥0.00";
            }
            catch
            {
            }

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedAccount(spec, ps.CurrentPageNo, ps.EachPageTotal, CheckBoxAllContract.Checked);

            //-------------------------
            var clist = from l in list
                        select new { l.Contract.ContractID, l.Contract.ContractValue};
            ContractNumber.Text = clist.Distinct().Count().ToString();
            ContractValue.Text = clist.Distinct().Sum(c => c.ContractValue).ToString("C2");
            
            var slist = from l in list
                        where l.SettlementInfoID != 0
                        select new { l.SettlementInfoID, l.AmountAudited, l.AmountChanged, l.AmountDeclared, l.AuditDeduction };            
            SettlementNumber.Text = slist.Distinct().Count().ToString();

           
            ContractSettmentDeclare.Text = list.Sum(cs => cs.AmountDeclared).ToString("C2");
            SettmentAuditValue.Text = list.Sum(cs => cs.AmountAudited).ToString("C2");
            DeduceValue.Text = list.Sum(cs => cs.AuditDeduction).ToString("C2");
            //----------------------------


            CLGridViewSAccount.DataSource = list;
            CLGridViewSAccount.DataBind();

            Pager.PageReload();
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            //if (this.TextBoxProject.Text.Length != 0)
            GoPage(1);
        }

        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AppendHeader("content-disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("合同台账" + System.DateTime.Now.ToShortDateString(), System.Text.Encoding.UTF8) + ".xls\"");
            Response.ContentType = "Application/ms-excel";


            Services.SAccountServices saservice = new Services.SAccountServices();
            var spec = GetQuerySpec();
            var list = saservice.GetAllData(spec, CheckBoxAllContract.Checked);


            Report.SAccountReport report = new Report.SAccountReport(MapPath("~/CMSReport/合同及结算台帐总表（合约）.xml"));

            var sheetname = (from l in list
                             select l.Contract.Project.ProjectName).Distinct();
            foreach (string name in sheetname)
            {
                report.GenerateReport(name, from l in list where l.Contract.Project.ProjectName == name select l);
            }


            report.SAReport.Save(Response.OutputStream);
            Response.End();


        }

        protected void CLGridViewSAccount_DataBound(object sender, EventArgs e)
        {
            int cid = 0;
            foreach (GridViewRow rv in CLGridViewSAccount.Rows)
            {
                Models.ContractInfo contract = (Models.ContractInfo)CLGridViewSAccount.DataKeys[rv.RowIndex].Values[1];

                if (contract.ContractID == cid)
                {
                    rv.Cells[3].CssClass = "samerow";
                    rv.Cells[4].CssClass = "samerow";
                    rv.Cells[5].CssClass = "samerow";
                    rv.Cells[6].CssClass = "samerow";
                    rv.Cells[7].CssClass = "samerow";
                    rv.Cells[8].CssClass = "samerow";
                    rv.Cells[9].CssClass = "samerow";
                    rv.Cells[10].CssClass = "samerow";
                    rv.Cells[11].CssClass = "samerow";
                    rv.Cells[12].CssClass = "samerow";

                }
                //rv.CssClass = "samerow";
                cid = contract.ContractID;
            }
        }

        //protected void CLGridViewSAccount_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    //if (e.Row.RowType == DataControlRowType.DataRow)
        //    //{
        //    //    int cid = 0;
        //    //    foreach (GridViewRow rv in CLGridViewSAccount.Rows)
        //    //    {
        //    //        Models.ContractInfo contract = (Models.ContractInfo)CLGridViewSAccount.DataKeys[rv.RowIndex].Values[1];

        //    //        if (contract.ContractID == cid)
        //    //            rv.Cells[1].CssClass = "samerow";
        //    //        cid = contract.ContractID;
        //    //    }
        //    //}
        //}
    }
}