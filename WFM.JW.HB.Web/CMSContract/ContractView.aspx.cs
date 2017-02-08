using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSProject
{
    public partial class ContractView : CLBasePage
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
                var mes = GetUrlPara<int>("new", false);
                if (mes.retType == 100)
                { 
                    InitControls(mes.Value);
                   // InitGridView(new Models.CriteriaSpecification("A.ProjectID", Models.CriteriaOperator.Equal, mes.Value));
                   
                }
                else
                { //初始化绑定        
                    var cmes = GetUrlPara<Models.Messaging<Models.ContractInfo>>("id", true);
                    if (cmes.retType == 100)
                    {
                        TextBoxContractName.Text = cmes.Value.Value.ContractName;
                    }
                    InitControls(-1);
                    //InitGridView(null);
                }
                GoPage(1);
            }
        }

        //初始化控件
        private void InitControls(int projectid)
        {
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();

            ////类型
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "合同类别");
            spec = spec.Or(new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "履约情况"));
            spec = spec.And(new Models.CriteriaSpecification("A.[enable]", Models.CriteriaOperator.Equal,true));

            var list = service.GetAllData(spec);

            DropDownListContractType.DataSource = from t in list
                                                  where t.ItemParent.BaseTypeName == "合同类别"
                                                  orderby t.ItemOrder
                                                  select t;
            DropDownListContractType.DataValueField = "BaseTypeItemID";
            DropDownListContractType.DataTextField = "ItemName";
            DropDownListContractType.DataBind();

            DropDownListContractType.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            //状态
            DropDownListContractStatus.DataSource = from t in list
                                                    where t.ItemParent.BaseTypeName == "履约情况"
                                                    orderby t.ItemOrder
                                                    select t; 
            DropDownListContractStatus.DataValueField = "BaseTypeItemID";
            DropDownListContractStatus.DataTextField = "ItemName";
            DropDownListContractStatus.DataBind();

            DropDownListContractStatus.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            //部门
            Services.DepartmentsService department = new Services.DepartmentsService();
            spec = new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, true);


            DropDownListDepartment.DataSource = department.GetAllData(spec);
            DropDownListDepartment.DataValueField = "DepartmentID";
            DropDownListDepartment.DataTextField = "DepartmentName";
            DropDownListDepartment.DataBind();

            DropDownListDepartment.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项


            //供方
            //Services.SupplierService supplier = new Services.SupplierService();
            //spec = new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true);

            //DropDownSupplier.DataSource = supplier.GetAllData(spec);
            //DropDownSupplier.DataValueField = "SupplierID";
            //DropDownSupplier.DataTextField = "SupplierName";
            //DropDownSupplier.DataBind();
            //DropDownSupplier.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

           
            //项目
            Services.ProjectInfoServices project = new Services.ProjectInfoServices();
            var projectlist = project.GetAllData(null);
            DropDownProject.DataSource = from p in projectlist
                                         where p.Status.ItemName != "关闭"
                                         select p;
            //DropDownProject.DataSource = project.GetAllData(null);
            DropDownProject.DataValueField = "ProjectID";
            DropDownProject.DataTextField = "ProjectName";
            DropDownProject.DataBind();
            DropDownProject.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            DropDownProject.SelectedValue = projectid.ToString();
        }

        //初始化GridView
        void InitGridView(Models.IParametersSpecification spec)
        {
            Services.ContractServices service = new Services.ContractServices();
            CLGridViewContractInfo.DataSource = service.GetAllData(spec);

            CLGridViewContractInfo.DataBind();
        }

        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.ContractInfo> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.ContractInfo> mes = new Models.Messaging<Models.ContractInfo>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewContractInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewContractInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int contractid = CLGridViewContractInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.ContractInfo();
                            mes.Value.ContractID = contractid;
                           
                            //mes.Value.ContractName = ((Label)(CLGridViewContractInfo.Rows[index].Cells[0].FindControl("LabelDepartment"))).Text;
                            mes.Value.ContractName = CLGridViewContractInfo.Rows[index].Cells[4].Text;
                            mes.Value.ContractCode = CLGridViewContractInfo.Rows[index].Cells[3].Text;

                            mes.retType = 200;
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

            if (this.TextBoxContractName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("ContractName", Models.CriteriaOperator.Like, this.TextBoxContractName.Text));

            if (this.TextBoxContractCode.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("ContractCode", Models.CriteriaOperator.Like, this.TextBoxContractCode.Text));


            if (DropDownProject.SelectedValue.CanConvertTo<int>())
                spec = spec.And(new Models.CriteriaSpecification("A.ProjectID", Models.CriteriaOperator.Equal, DropDownProject.SelectedValue.ConvertTo<int>()));

          //  if (DropDownSupplier.SelectedValue.CanConvertTo<int>())
            if (this.TextBoxSupplier.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("A.SupplierName", Models.CriteriaOperator.Like, TextBoxSupplier.Text));

            if (DropDownListDepartment.SelectedValue.CanConvertTo<int>())
                spec = spec.And(new Models.CriteriaSpecification("A.DepartmentID", Models.CriteriaOperator.Equal, DropDownListDepartment.SelectedValue.ConvertTo<int>()));

            if (DropDownListContractType.SelectedValue.CanConvertTo<int>())
                spec = spec.And(new Models.CriteriaSpecification("A.ContractTypeID", Models.CriteriaOperator.Equal, DropDownListContractType.SelectedValue.ConvertTo<int>()));


            if (DropDownListContractStatus.SelectedValue.CanConvertTo<int>())
                spec = spec.And(new Models.CriteriaSpecification("A.ContractStatusID", Models.CriteriaOperator.Equal, DropDownListContractStatus.SelectedValue.ConvertTo<int>()));


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

        //分页查询
        void GoPage(int CurrentPage)
        {

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewContractInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            
            Services.ContractServices service = new Services.ContractServices();
            //ps.TotalCount = service.GetPagedDataCount(spec);
            ///---------------
            var ds = service.GetGroupData(spec, "count(*), sum(contractValue), sum(ContractSettle)");
            try
            {
                ps.TotalCount = (int)ds.Tables[0].Rows[0][0]; //TotalNumber 
                ContractTotalNumber.Text = ps.TotalCount.ToString();
                ContractTotalValue.Text = ds.Tables[0].Rows[0][1].ToString(); 
                if(ContractTotalValue.Text.CanConvertTo<double>())
                    ContractTotalValue.Text = ContractTotalValue.Text.ConvertTo<double>().ToString("C");
                else ContractTotalValue.Text = "￥0.00";

                ContractTotalSettmentValue.Text = ds.Tables[0].Rows[0][2].ToString();
                if (ContractTotalSettmentValue.Text.CanConvertTo<double>())
                    ContractTotalSettmentValue.Text = ContractTotalSettmentValue.Text.ConvertTo<double>().ToString("C");
                else ContractTotalSettmentValue.Text = "￥0.00";
            }
            catch
            {
            }
            ///-----------------------------------       

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);

            //-------------------------------
            ContractNumber.Text = list.Count().ToString();
            ContractValue.Text = list.Sum(c => c.ContractValue).ToString("C");
            ContractSettmentValue.Text = list.Sum(cs => cs.ContractSettle).ToString("C");

            //------------------------------


            CLGridViewContractInfo.DataSource = list;
            CLGridViewContractInfo.DataBind();

            Pages1.PageReload();
        }

        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (this.TextBoxContractName.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("ContractName", Models.CriteriaOperator.Like, this.TextBoxContractName.Text));

            //if (this.TextBoxContractCode.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("ContractCode", Models.CriteriaOperator.Like, this.TextBoxContractCode.Text));


            //if (DropDownProject.SelectedValue.CanConvertTo<int>())
            //    spec = spec.And(new Models.CriteriaSpecification("A.ProjectID", Models.CriteriaOperator.Equal, DropDownProject.SelectedValue.ConvertTo<int>()));

            //if (DropDownSupplier.SelectedValue.CanConvertTo<int>())
            //    spec = spec.And(new Models.CriteriaSpecification("A.SupplierID", Models.CriteriaOperator.Equal, DropDownSupplier.SelectedValue.ConvertTo<int>()));

            //if(DropDownListDepartment.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.DepartmentID", Models.CriteriaOperator.Equal, DropDownListDepartment.SelectedValue.ConvertTo<int>()));
            
            //if(DropDownListContractType.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.ContractTypeID", Models.CriteriaOperator.Equal, DropDownListContractType.SelectedValue.ConvertTo<int>()));


            //if(DropDownListContractStatus.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.ContractStatusID", Models.CriteriaOperator.Equal, DropDownListContractStatus.SelectedValue.ConvertTo<int>()));

            
            //InitGridView(spec);
        }

        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(0);

            Response.Redirect("~/CMSContract/ContractEdit.aspx?id=" + mes.Message);

        }
        
        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSContract/ContractEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的供方
                ShowDialog.Value = "请选择要修改的工程项目";
                //showWarning("请选择要修改的工程项目");
            }
        }

        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的项目";
            if (mes.retType == 200)
            {//OK删除
                Services.ContractServices service = new Services.ContractServices();
                var retMes = service.DeleteData(mes.Value);

                if (retMes.retType == 100)
                    // InitGridView(null);
                    GoPage(1);
                
                mes.copy(retMes);
            }
            
            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                //showWarning(mes.Message);
                ShowDialog.Value = mes.Message;
            }
        }

        protected void CLGridViewProjectInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                //LinkButtonModify_Click(null, null);
                LinkButtonDetails_Click(null, null);
                
            }
        }

        protected void LinkButtonDetails_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSSettlement/SettlementView.aspx?id=" + mes.Message);
            }
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AppendHeader("content-disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("合同清单" + System.DateTime.Now.ToShortDateString(), System.Text.Encoding.UTF8) + ".xls\"");
            Response.ContentType = "Application/ms-excel";

            Services.ContractServices service = new Services.ContractServices();
            var spec = GetQuerySpec();
            var list = service.GetAllData(spec);


            CMSReport.ContractReport report = new CMSReport.ContractReport();
            report.GenerateReport("合同清单", list);


            report.SAReport.Save(Response.OutputStream);
            Response.End();
        }

    }
}