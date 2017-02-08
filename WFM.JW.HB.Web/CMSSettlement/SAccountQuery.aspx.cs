using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;


namespace WFM.JW.HB.Web.CMSSettlement
{
    public partial class SAccountQuery : System.Web.UI.Page
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

            if (this.TextBoxContractName.Text.Length != 0)
            {
                spec = spec.And(new Models.CriteriaSpecification("A.ContractName", Models.CriteriaOperator.Like, TextBoxContractName.Text));
            }

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

            var list = service.GetPagedAccount2(spec, ps.CurrentPageNo, ps.EachPageTotal, CheckBoxAllContract.Checked);

            //-------------------------
            var clist = from l in list
                        select new { l.Contract.ContractID, l.Contract.ContractValue };
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


        /**
    * type = 0 新增
    * type = 1 修改  
    * 
    * mes.Value = null 时 
    * */
        private Models.Messaging<Models.SettlementRecord> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.SettlementRecord> mes = new Models.Messaging<Models.SettlementRecord>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取SettlementRecordid
                    if (index >= 0 && index < CLGridViewSAccount.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewSAccount.DataKeys[index].Values[0].CanConvertTo<int>() &&
                            CLGridViewSAccount.DataKeys[index].Values[1].CanConvertTo<Models.ContractInfo>())
                        {//获取gridview datakey中的值
                            int settlementid = CLGridViewSAccount.DataKeys[index].Values[0].ConvertTo<int>();
                            var contract = CLGridViewSAccount.DataKeys[index].Values[1].ConvertTo<Models.ContractInfo>();
                            mes.Value = new Models.SettlementRecord();
                            mes.Value.SettlementInfoID = settlementid;
                            mes.Value.Contract = contract;
                            mes.retType = 100;
                        }
                    }
                }
            }

           
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }


        protected void LinkButtonSettlementModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            if (mes.retType == 100)
            {
                Models.Messaging<Models.ContractInfo> cmes = new Models.Messaging<Models.ContractInfo>();
                cmes.Value = mes.Value.Contract;
                cmes.Message = mes.Message;

                Page.Session[cmes.Message] = cmes;
                Response.Redirect("~/CMSSettlement/SettlementView.aspx?id=" + cmes.Message);
            }
        }

        protected void LinkButtonContractModify_Click(object sender, EventArgs e)
        {
            
            var mes = GenereatePageParameter(1);

            if (mes.retType == 100)
            {
                Models.Messaging<Models.ContractInfo> cmes = new Models.Messaging<Models.ContractInfo>();
                cmes.Value = mes.Value.Contract;
                cmes.Message = mes.Message;

                Page.Session[cmes.Message] = cmes;
                Response.Redirect("~/CMSContract/ContractView.aspx?id=" + cmes.Message);
            }
        }

        protected void CLGridViewSAccount_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonSettlementModify_Click(null, null);

                selectedIndex.Value = "";
            }
        }

        protected void ButtonExport_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AppendHeader("content-disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("合同结算" + System.DateTime.Now.ToShortDateString(), System.Text.Encoding.UTF8) + ".xls\"");
            Response.ContentType = "Application/ms-excel";

            Services.SAccountServices service = new Services.SAccountServices();
            var spec = GetQuerySpec();
            var list = service.GetAllData(spec, CheckBoxAllContract.Checked);


            CMSReport.CSReport report = new CMSReport.CSReport();
            report.GenerateReport("合同结算", list);


            report.SAReport.Save(Response.OutputStream);
            Response.End();
        }

    }
}