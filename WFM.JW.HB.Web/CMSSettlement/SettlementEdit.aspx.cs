using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSSettlement
{
    public partial class SettlementEdit : CLBasePage
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
                var mes = GetUrlPara<Models.Messaging<Models.SettlementRecord>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new 
                    var contractmes = GetUrlPara<string>("new", false);
                    if (contractmes.retType == 100)
                    {
                        ContractID.Value = contractmes.Value.ToString();
                        InitControls(true, null);
                    }
                    else LinkButtonBack_Command(null, null);

                    //InitControls(true, null);
                }
                else
                {//modify
                    InitControls(false, mes.Value.Value);
                }
            }
        }

        void InitControls(bool type, Models.SettlementRecord settlement)
        {
            //初始化下拉框
            #region 初始化下拉列表
            Services.EmployeeServices service = new Services.EmployeeServices();

            ////类型
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true);
            var list = service.GetAllData(spec);

            DropDownListAuditor.DataSource = list;
            DropDownListAuditor.DataValueField = "EmployeeID";
            DropDownListAuditor.DataTextField = "EmployeeName";
            DropDownListAuditor.DataBind();

            DropDownListAuditor.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项



            ////类型
            Services.BaseTypeItemServices typeservice = new Services.BaseTypeItemServices();
            spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "结算状态");
            spec = spec.And(new Models.CriteriaSpecification("A.[enable]", Models.CriteriaOperator.Equal, true));

            var typelist = typeservice.GetAllData(spec);
            DropDownListStatus.DataSource = typelist;
            DropDownListStatus.DataValueField = "BaseTypeItemID";
            DropDownListStatus.DataTextField = "ItemName";
            DropDownListStatus.DataBind();

            #endregion

            #region 初始化合同信息
            if (ContractID.Value.CanConvertTo<int>())
            {
                int contractid = ContractID.Value.ConvertTo<int>();

                Services.ContractServices contractservice = new Services.ContractServices();
                Models.IParametersSpecification contractspec = new Models.CriteriaSpecification("ContractID", Models.CriteriaOperator.Equal, contractid);
                var contractlist = contractservice.GetAllData(contractspec);
                if (contractlist.Count() != 0)
                {
                    var contractfirst = contractlist.First();
                    TextBoxContractName.Value = contractfirst.ContractName;
                    TextBoxContractCode.Value = contractfirst.ContractCode;
                }

                //TextBoxContractName.Disabled = true;
                //TextBoxContractCode.Disabled = true;
            }
            #endregion


            TextBoxSettlementDate.Value = System.DateTime.Now.ToString("yyyy-MM-dd");

            LinkButtonApply.CommandArgument = "New";
            LinkButtonBack.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //修改，初始化
            if (settlement != null)
            {
                Services.SettlementServices settleService = new Services.SettlementServices();
                spec = new Models.CriteriaSpecification("SettlementInfoID", Models.CriteriaOperator.Equal, settlement.SettlementInfoID);
                var settleList = settleService.GetAllData(spec);
                if (settleList.Count() != 0)
                {
                    settlement = settleList.First();

                    //change contractID
                    ContractID.Value = settlement.Contract.ContractID.ToString();


                    TextBoxContractName.Value = settlement.Contract.ContractName;
                    TextBoxContractCode.Value = settlement.Contract.ContractCode;

                    TextBoxSettlementCode.Text = settlement.SettlementCode;
                    TextBoxSettlementDate.Value = settlement.SettlementDate.ToString("yyyy-MM-dd");

                    DropDownListStatus.SelectedValue = settlement.SettlementStatus.BaseTypeItemID.ToString();

                    TextBoxFiling.Text = settlement.Filing;
                    TextBoxDeclared.Text = settlement.AmountDeclared.ToString();
                    TextBoxChange.Text = settlement.AmountChanged.ToString();
                    TextBoxAudited.Text = settlement.AmountAudited.ToString();
                    TextBoxDeduction.Value = settlement.AuditDeduction.ToString();

                    DropDownListAuditor.SelectedValue = settlement.Auditor;
                    TextBoxComment.Text = settlement.Comment;
                    TextBoxStatementor.Text = settlement.Statementor;


                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonBack.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";

                    //hidden field
                    SettlementID.Value = settlement.SettlementInfoID.ToString();
                    Version.Value = settlement.version.ToString();
                }
            }
        }

        //生成Settlement对象
        //type  新增 or 修改
        private Models.Messaging<Models.SettlementRecord> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.SettlementRecord>();
            mes.Value = new Models.SettlementRecord();
            mes.retType = 1;

            //new supplier needed
            if (ContractID.Value.CanConvertTo<int>() &&
                DropDownListStatus.SelectedValue.CanConvertTo<int>() &&
                TextBoxSettlementDate.Value.CanConvertTo<DateTime>())
            {
                //合同ID
                mes.Value.Contract = new Models.ContractInfo();
                mes.Value.Contract.ContractID = ContractID.Value.ConvertTo<int>();

                //审核人
                if (DropDownListAuditor.SelectedValue.CanConvertTo<int>())
                {
                    mes.Value.Auditor = DropDownListAuditor.SelectedItem.Text;
                }

                //对账人
                mes.Value.Statementor = TextBoxStatementor.Text;

                //结算状态
                mes.Value.SettlementStatus = new Models.TypeItem();
                mes.Value.SettlementStatus.BaseTypeItemID = DropDownListStatus.SelectedValue.ConvertTo<int>();

                //结算编号
                mes.Value.SettlementCode = TextBoxSettlementCode.Text;

                //结算时间
                mes.Value.SettlementDate = TextBoxSettlementDate.Value.ConvertTo<DateTime>();

                //归档
                mes.Value.Filing = TextBoxFiling.Text;

                //报审金额
                if (TextBoxDeclared.Text.CanConvertTo<double>())
                    mes.Value.AmountDeclared = TextBoxDeclared.Text.ConvertTo<double>();
                //审核金额
                if (TextBoxAudited.Text.CanConvertTo<double>())
                    mes.Value.AmountAudited = TextBoxAudited.Text.ConvertTo<double>();

                //审减金额
                if (mes.Value.AmountDeclared >= mes.Value.AmountAudited)
                    mes.Value.AuditDeduction = mes.Value.AmountDeclared - mes.Value.AmountAudited;

                //增加变更
                if (TextBoxChange.Text.CanConvertTo<double>())
                    mes.Value.AmountChanged = TextBoxChange.Text.ConvertTo<double>();

                //备注
                mes.Value.Comment = TextBoxComment.Text;


                //创建人
                string username = Page.User.Identity.Name;
                if (String.IsNullOrEmpty(username))
                    mes.Value.Creator = "anonymous";
                else mes.Value.Creator = username;
                mes.Value.CreateDate = System.DateTime.Now;
                mes.retType = 100;
                mes.Message = "获取settlement对象成功!";
            }
            else
            {
                mes.retType = 1;
                mes.Message = "新增参数结算类型或状态错误";
            }


            //modify settlement needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (SettlementID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>())
                {
                    //隐藏控件－结算ID不能变－
                    mes.Value.SettlementInfoID = SettlementID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();

                    //修改人
                    string username = Page.User.Identity.Name;
                    if (String.IsNullOrEmpty(username))
                        mes.Value.Modifier = "anonymous";
                    else mes.Value.Modifier = username;
                    mes.Value.modifyDate = System.DateTime.Now;

                    mes.retType = 200;
                }
                else
                {
                    mes.retType = 2;
                    mes.Message = "修改无法获取结算ID和版本!";
                }
            }

            return mes;
        }

        private Models.Messaging<int> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;
            var mes = GenerateObject(type);

            if (mes.retType == 100)
            {//新增对象获取成功
                Services.SettlementServices service = new Services.SettlementServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.SettlementServices service = new Services.SettlementServices();
                retMes = service.UpdateData(mes.Value);
            }
            else
            {//错误
                retMes = new Models.Messaging<int>();
                retMes.copy(mes);
            }
            LabelInfo.Text = retMes.Message;

            return retMes;
        }
        //应用
        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            ChangeObject(e.CommandArgument.ToString());
        }

        //保存
        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
            if (ChangeObject(e.CommandArgument.ToString()).retType == 100)
                LinkButtonBack_Command(null, null);
        }

        //返回
        protected void LinkButtonBack_Command(object sender, CommandEventArgs e)
        {
            if (ContractID.Value.CanConvertTo<int>())
                Response.Redirect("~/CMSSettlement/SettlementView.aspx?id=" + ContractID.Value);
            else Response.Redirect("~/CMSSettlement/SettlementView.aspx");
        }
      
        
        //选择合同号
        private void GoPage(int CurrentPage)
        {

            var ps = this.Pager.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(ContractGrid.PageSize);
                this.Pager.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.ContractServices service = new Services.ContractServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);
            ContractGrid.DataSource = list;
            ContractGrid.DataBind();

            Pager.PageReload();
        }

        Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            if (this.TextBoxContractNamesearch.Text.Length != 0)
            {
                spec = spec.And(new Models.CriteriaSpecification("ContractName", Models.CriteriaOperator.Like, TextBoxContractNamesearch.Text));
            }

            if (this.TextBoxContractCodesearch.Text.Length != 0)
            {
                spec = spec.And(new Models.CriteriaSpecification("ContractCode", Models.CriteriaOperator.Like, TextBoxContractCodesearch.Text));
            }

            return spec;
        }

        protected void LinkButtonContract_Click(object sender, EventArgs e)
        {
            GoPage(1);

            //Services.ContractServices contract = new Services.ContractServices();
            //var list = contract.GetAllData();

            //ContractGrid.DataSource = from l in list
            //                          select new { l.ContractID, l.ContractName, l.ContractCode };
            //ContractGrid.DataBind();
        }

        protected void ContractGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
            }
        }
    }
}