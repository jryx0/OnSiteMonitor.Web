using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSSettlement
{
    public partial class SettlementView : CLBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            this.Pages1.GoPage += new GoPageEventHandler(GoPage);
            base.OnInit(e);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.ContractInfo>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new 
                    InitControls(true, null);
                }
                else
                {//modify
                    InitControls(false, mes.Value.Value);
                }
            }


        }


        private void InitControls(bool type, Models.ContractInfo contract)
        {
           // LinkButtonBack.CommandArgument = type.ToString();
            LinkButtonNew.CommandArgument = type.ToString();

            //Models.Employees employees = 

            #region 初始化下拉列表
            Services.EmployeeServices service = new Services.EmployeeServices();

            ////类型
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true);
            var list = service.GetAllData(spec);

            DropDownListAuditor.DataSource = list;
            DropDownListAuditor.DataValueField = "EmployeeID";
            DropDownListAuditor.DataTextField = "EmployeeName";
            DropDownListAuditor.DataBind();

            DropDownListAuditor.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            //结算状态
            Services.BaseTypeItemServices typeservice = new Services.BaseTypeItemServices();
            spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "结算状态");
            spec = spec.And(new Models.CriteriaSpecification("A.[enable]", Models.CriteriaOperator.Equal, true));

            var typelist = typeservice.GetAllData(spec);
            DropDownListSettlementStatus.DataSource = typelist;
            DropDownListSettlementStatus.DataValueField = "BaseTypeItemID";
            DropDownListSettlementStatus.DataTextField = "ItemName";
            DropDownListSettlementStatus.DataBind();
            DropDownListSettlementStatus.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项





            #endregion

            spec = null;
            if (!type)
            {//came from contractinfo
                if (contract != null)
                {//query settlement of contract                   
                    //set current contractinfo
                    TextBoxContractCode.Text = contract.ContractCode;
                    TextBoxContractName.Text = contract.ContractName;

                    //Hidden contractid
                    ContractID.Value = contract.ContractID.ToString();

                    //save contractid from back to contractview
                  //  LinkButtonBack.CommandArgument = contract.ContractID.ToString();
                    LinkButtonNew.CommandArgument = contract.ContractID.ToString();

                    spec = new Models.CriteriaSpecification("CMS_CSCL_SettlementInfo.ContractID", Models.CriteriaOperator.Equal, contract.ContractID);
                }
            }
            InitGridView(spec);
        }

        //初始化GridView
        void InitGridView(Models.IParametersSpecification spec)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(CLGridViewSettlement.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }
           
            Services.SettlementServices service = new Services.SettlementServices();
            //ps.TotalCount = service.GetPagedDataCount(spec);
            //ps.TotalCount = service.GetPagedDataCount(spec);

            var ds = service.GetGroupData(spec, "count(*), sum(AmountDeclared), sum(AmountAudited), sum(AuditDeduction)");
            try
            {
                
                 

                ps.TotalCount = (int)ds.Tables[0].Rows[0][0]; //TotalNumber 
                TotalNumber.Text = ps.TotalCount.ToString();
                TotalDeclareValue.Text = ds.Tables[0].Rows[0][1].ToString();
                if (TotalDeclareValue.Text.CanConvertTo<double>())
                    TotalDeclareValue.Text = TotalDeclareValue.Text.ConvertTo<double>().ToString("C");
                else TotalDeclareValue.Text = "￥0.00";

                TotalSettmentValue.Text = ds.Tables[0].Rows[0][2].ToString();
                if (TotalSettmentValue.Text.CanConvertTo<double>())
                    TotalSettmentValue.Text = TotalSettmentValue.Text.ConvertTo<double>().ToString("C");
                else
                    TotalSettmentValue.Text = "￥0.00";
                

                TotalDeduce.Text = ds.Tables[0].Rows[0][3].ToString();
                if (TotalDeduce.Text.CanConvertTo<double>())
                    TotalDeduce.Text = TotalDeduce.Text.ConvertTo<double>().ToString("C");
                else TotalDeduce.Text = "￥0.00";
            }
            catch
            {
            }



            ps.SetStartIndex(0);
            var settlelist = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);

            //-------------------------------
            SettleNumber.Text = settlelist.Count().ToString();
            DeclareValue.Text = settlelist.Sum(cs => cs.AmountDeclared).ToString("C");
            SettmentValue.Text = settlelist.Sum(cs => cs.AmountAudited).ToString("C");
            SettleDeduce.Text = settlelist.Sum(cs => cs.AuditDeduction).ToString("C");
            //------------------------------


            CLGridViewSettlement.DataSource = settlelist;
            CLGridViewSettlement.DataBind();
            if (settlelist.Count() != 0)
                if (settlelist.First().Contract != null)
                {
                    //TextBoxContractCode.Text = settlelist.First().Contract.ContractCode;
                    //TextBoxContractName.Text = settlelist.First().Contract.ContractName;
                    ContractID.Value = settlelist.First().Contract.ContractID.ToString();
                }
            Pages1.PageReload();
        }

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
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewSettlement.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewSettlement.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int settlementid = CLGridViewSettlement.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.SettlementRecord();
                            mes.Value.SettlementInfoID = settlementid;

                            if (ContractID.Value.CanConvertTo<int>())
                            {
                                mes.Value.Contract = new Models.ContractInfo();
                                mes.Value.Contract.ContractID = ContractID.Value.ConvertTo<int>();

                                mes.retType = 200;
                            }
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (this.TextBoxSettlementCode.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("SettlementCode", Models.CriteriaOperator.Like, this.TextBoxSettlementCode.Text));


            //dateTime
            if (TextBoxSettlementStartDate.Value.Length != 0 && TextBoxSettlementStartDate.Value.CanConvertTo<DateTime>())
            {
                spec = spec.And(new Models.CriteriaSpecification("SettlementDate", Models.CriteriaOperator.GreaterThanOrEqual, TextBoxSettlementStartDate.Value.ConvertTo<DateTime>()));
            }

            if (TextBoxSettlementEndDate.Value.Length != 0 && TextBoxSettlementEndDate.Value.CanConvertTo<DateTime>())
            {
                spec = spec.And(new Models.CriteriaSpecification("SettlementDate", Models.CriteriaOperator.LessThanOrEqual, TextBoxSettlementEndDate.Value.ConvertTo<DateTime>()));

            }

            //status
            if (DropDownListSettlementStatus.SelectedValue.CanConvertTo<int>())
            {
                int DropValule = DropDownListSettlementStatus.SelectedValue.ConvertTo<int>();
                spec = spec.And(new Models.CriteriaSpecification("SettlementStatusID", Models.CriteriaOperator.Equal, DropValule));
            }

            //auditor
            if (DropDownListAuditor.SelectedValue.CanConvertTo<int>())
            {
                var DropValule = DropDownListAuditor.SelectedItem.Text;
                spec = spec.And(new Models.CriteriaSpecification("Auditor", Models.CriteriaOperator.Equal, DropValule));
            }

            //[Statementor]
            if (TextBoxStatementor.Text.Length != 0)
            {
                spec = spec.And(new Models.CriteriaSpecification("Statementor", Models.CriteriaOperator.Like, TextBoxStatementor.Text));
            }

            //contractname 
            if (this.TextBoxContractName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_ContractInfo.ContractName", Models.CriteriaOperator.Like, this.TextBoxContractName.Text));
            //TextBoxContractCode
            if (this.TextBoxContractCode.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_ContractInfo.ContractCode", Models.CriteriaOperator.Like, this.TextBoxContractCode.Text));



            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(CLGridViewSettlement.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.SettlementServices service = new Services.SettlementServices();
            //ps.TotalCount = service.GetPagedDataCount(spec);

            var ds = service.GetGroupData(spec, "count(*), sum(AmountDeclared), sum(AmountAudited), sum(AuditDeduction)");
            try
            {
                ps.TotalCount = (int)ds.Tables[0].Rows[0][0]; //TotalNumber 
                TotalNumber.Text = ps.TotalCount.ToString();
                TotalDeclareValue.Text = ds.Tables[0].Rows[0][1].ToString();
                if (TotalDeclareValue.Text.CanConvertTo<double>())
                    TotalDeclareValue.Text = TotalDeclareValue.Text.ConvertTo<double>().ToString("C");
                else TotalDeclareValue.Text = "￥0.00";

                TotalSettmentValue.Text = ds.Tables[0].Rows[0][2].ToString();
                if (TotalSettmentValue.Text.CanConvertTo<double>())
                    TotalSettmentValue.Text = TotalSettmentValue.Text.ConvertTo<double>().ToString("C");
                else TotalSettmentValue.Text = "￥0.00";

                TotalDeduce.Text = ds.Tables[0].Rows[0][3].ToString();
                if (TotalDeduce.Text.CanConvertTo<double>())
                    TotalDeduce.Text = TotalDeduce.Text.ConvertTo<double>().ToString("C");
                else TotalDeduce.Text = "￥0.00";
            }
            catch
            {
            }
                    





            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var settlelist = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            //-------------------------------
            SettleNumber.Text = settlelist.Count().ToString();
            DeclareValue.Text = settlelist.Sum(cs => cs.AmountDeclared).ToString("C");
            SettmentValue.Text = settlelist.Sum(cs => cs.AmountAudited).ToString("C");
            SettleDeduce.Text = settlelist.Sum(cs => cs.AmountDeclared).ToString("C");
            //------------------------------


            CLGridViewSettlement.DataSource = settlelist;
            CLGridViewSettlement.DataBind();

            if (settlelist.Count() != 0)
                if (settlelist.First().Contract != null)
                {
                    //TextBoxContractCode.Text = settlelist.First().Contract.ContractCode;
                    //TextBoxContractName.Text = settlelist.First().Contract.ContractName;
                    ContractID.Value = settlelist.First().Contract.ContractID.ToString();
                }
            Pages1.PageReload();
        }
        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {

            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (this.TextBoxSettlementCode.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("SettlementCode", Models.CriteriaOperator.Like, this.TextBoxSettlementCode.Text));


            ////dateTime
            //if (TextBoxSettlementStartDate.Value.Length != 0 && TextBoxSettlementStartDate.Value.CanConvertTo<DateTime>())
            //{
            //    spec = spec.And(new Models.CriteriaSpecification("SettlementDate", Models.CriteriaOperator.GreaterThanOrEqual, TextBoxSettlementStartDate.Value.ConvertTo<DateTime>()));
            //}

            //if (TextBoxSettlementEndDate.Value.Length != 0 && TextBoxSettlementEndDate.Value.CanConvertTo<DateTime>())
            //{
            //    spec = spec.And(new Models.CriteriaSpecification("SettlementDate", Models.CriteriaOperator.LessThanOrEqual, TextBoxSettlementEndDate.Value.ConvertTo<DateTime>()));

            //}

            ////status
            //if (DropDownListSettlementStatus.SelectedValue.CanConvertTo<int>())
            //{
            //    int DropValule = DropDownListSettlementStatus.SelectedValue.ConvertTo<int>();
            //    spec = spec.And(new Models.CriteriaSpecification("SettlementStatusID", Models.CriteriaOperator.Equal, DropValule));
            //}

            ////auditor
            //if (DropDownListAuditor.SelectedValue.CanConvertTo<bool>())
            //{
            //    bool DropValule = DropDownListAuditor.SelectedValue.ConvertTo<bool>();
            //    spec = spec.And(new Models.CriteriaSpecification("Auditor", Models.CriteriaOperator.Equal, DropValule));
            //}

            ////[Statementor]
            //if (TextBoxStatementor.Text.Length!= 0)
            //{
            //    spec = spec.And(new Models.CriteriaSpecification("Statementor", Models.CriteriaOperator.Like, TextBoxStatementor.Text));
            //}

            ////contractname 
            //if (this.TextBoxContractName.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_ContractInfo.ContractName", Models.CriteriaOperator.Like, this.TextBoxContractName.Text));
            ////TextBoxContractCode
            // if (this.TextBoxContractCode.Text.Length != 0)
            //     spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_ContractInfo.ContractCode", Models.CriteriaOperator.Like, this.TextBoxContractCode.Text));

            //var spec = GetQuerySpec();
            //InitGridView(spec);
            GoPage(1);
        }


        protected void LinkButtonNew_Command(object sender, CommandEventArgs e)
        {
            //检测selectid 是否选择

            //must check the contractid

            //if (e.CommandArgument.ToString().CanConvertTo<int>())
            {//无contract Name 
                var mes = GenereatePageParameter(0);
                string para = null;
                if (mes.retType != 200)
                {//no, selectedid
                    if (ContractID.Value.CanConvertTo<int>())
                        para = "&new=" + ContractID.Value;
                }
                mes.retType = 100;

                Response.Redirect("~/CMSSettlement/SettlementEdit.aspx?id=" + mes.Message + para);
            }
        }

        protected void LinkButtonBack_Command(object sender, CommandEventArgs e)
        {
            if (ContractID.Value.CanConvertTo<int>())
                Response.Redirect("~/CMSContract/ContractView.aspx?id=" + ContractID.Value);
            else
                Response.Redirect("~/CMSContract/ContractView.aspx");
        }

        protected void LinkButtonModify_Command(object sender, CommandEventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSSettlement/SettlementEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的结算
                ShowDialog.Value = mes.Message;
            }
        }

        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的结算数据";
            if (mes.retType == 200)
            {//OK删除
                Services.SettlementServices service = new Services.SettlementServices();
                var retMes = service.DeleteData(mes.Value);
                mes.copy(retMes);
                InitGridView(null);
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                //showWarning(mes.Message);
                ShowDialog.Value = mes.Message;
            }

        }

        //双击
        protected void CLGridViewSettlement_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Command(null, null);

                selectedIndex.Value = "";
            }
        }
    }
}