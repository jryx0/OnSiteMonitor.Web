using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSProject
{
    public partial class ContractEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.ContractInfo>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                    InitControls(null);
                }
                else
                {//modify
                    InitControls(mes.Value.Value);
                }
            }
        }


        //初始化控件
        private void InitControls(Models.ContractInfo info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //签约时间
            ContractDate.Value = DateTime.Now.ToString("yyyy-MM-dd");

            #region 初始化下拉列表
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

            DropDownListContractType.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项

            //状态
            DropDownListContractStatus.DataSource = from t in list
                                                    where t.ItemParent.BaseTypeName == "履约情况"
                                                    orderby t.ItemOrder
                                                    select t;
            DropDownListContractStatus.DataValueField = "BaseTypeItemID";
            DropDownListContractStatus.DataTextField = "ItemName";
            DropDownListContractStatus.DataBind();

            DropDownListContractStatus.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项

            //部门
            Services.DepartmentsService department = new Services.DepartmentsService();
            spec = new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, true);


            DropDownListDepartment.DataSource = department.GetAllData(spec);
            DropDownListDepartment.DataValueField = "DepartmentID";
            DropDownListDepartment.DataTextField = "DepartmentName";
            DropDownListDepartment.DataBind();

            DropDownListDepartment.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项


            //供方
            //Services.SupplierService supplier = new Services.SupplierService();
            //spec = new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true);

            //DropDownSupplier.DataSource = supplier.GetAllData(spec);
            //DropDownSupplier.DataValueField = "SupplierID";
            //DropDownSupplier.DataTextField = "SupplierName";
            //DropDownSupplier.DataBind();
            //DropDownSupplier.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项


            //项目
            Services.ProjectInfoServices project = new Services.ProjectInfoServices();

            var projectlist = project.GetAllData(null);
            DropDownProject.DataSource = from p in projectlist
                                         where p.Status.ItemName != "关闭"
                                         select p;
            DropDownProject.DataValueField = "ProjectID";
            DropDownProject.DataTextField = "ProjectName";
            DropDownProject.DataBind();
            DropDownProject.Items.Insert(0, new ListItem("---", "all")); //动态插入指定序号的新项

            #endregion


            //修改导航
            SiteMapNode smn = SiteMap.Provider.FindSiteMapNode("~/CMSContract/ContractEdit.aspx");//获得当前节点
            smn.ReadOnly = false;//将当前节点设置为只读
            smn.Title = "新增合同";//为当前节点title赋值


            if (info != null)
            {//modify init
                Services.ContractServices contract = new Services.ContractServices();
                spec = new Models.CriteriaSpecification("ContractID", Models.CriteriaOperator.Equal, info.ContractID);
                var contractlist = contract.GetAllData(spec);

                if (contractlist.Count() != 0)
                {
                    DropDownListContractType.Items.RemoveAt(0);
                    DropDownListDepartment.Items.RemoveAt(0);
                    DropDownListContractStatus.Items.RemoveAt(0);
                   // DropDownSupplier.Items.RemoveAt(0);
                    DropDownProject.Items.RemoveAt(0);

                    var c = contractlist.First();

                    //下拉框
                    DropDownListContractType.SelectedValue = c.ContractType.BaseTypeItemID.ToString();
                    DropDownListDepartment.SelectedValue = c.Department.DepartmentID.ToString();
                    DropDownListContractStatus.SelectedValue = c.ContractStatus.BaseTypeItemID.ToString();
                    //DropDownSupplier.SelectedValue = c.Supplier.SupplierID.ToString();
                    DropDownProject.SelectedValue = c.Project.ProjectID.ToString();

                    ContractDate.Value = c.ContractDate.ToString("yyyy-MM-dd");
                    TextBoxContractName.Text = c.ContractName;
                    TextBoxContractCode.Text = c.ContractCode;
                    TextBoxComment.Text = c.Comment;
                    TextBoxValue.Text = c.ContractValue.ToString("F");
                    TextBoxFiling.Text = c.Filing;

                    //供方
                    TextBoxSupplier.Text = c.SupplierName;

                    //ContractID
                    ContractID.Value = c.ContractID.ToString();
                    Version.Value = c.version.ToString();


                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";

                    //修改导航
                    smn.Title = "修改合同";//为当前节点title赋值 
                }
            }
        }


         //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.ContractInfo> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.ContractInfo>();
            mes.Value = new Models.ContractInfo();
            mes.retType = 1;

            //new contract needed
            if (DropDownListContractType.SelectedValue.CanConvertTo<int>() && 
                DropDownListDepartment.SelectedValue.CanConvertTo<int>() &&
                DropDownListContractStatus.SelectedValue.CanConvertTo<int>() &&
                //DropDownSupplier.SelectedValue.CanConvertTo<int>() &&
                DropDownProject.SelectedValue.CanConvertTo<int>() &&
                ContractDate.Value.CanConvertTo<DateTime>())
            {
                //项目
                mes.Value.Project = new Models.ProjectInfo();
                mes.Value.Project.ProjectID = DropDownListContractType.SelectedValue.ConvertTo<int>();
                //部门ID
                mes.Value.Department = new Models.Departments();
                mes.Value.Department.DepartmentID = DropDownListDepartment.SelectedValue.ConvertTo<int>();
                
                //合同状态
                mes.Value.ContractStatus = new Models.TypeItem();
                mes.Value.ContractStatus.BaseTypeItemID = DropDownListContractStatus.SelectedValue.ConvertTo<int>();

                //合同类型
                mes.Value.ContractType = new Models.TypeItem();
                mes.Value.ContractType.BaseTypeItemID = DropDownListContractType.SelectedValue.ConvertTo<int>();


                //供方
                //mes.Value.Supplier = new Models.SupplierInfo();
                mes.Value.SupplierName = TextBoxSupplier.Text;
                
                //所属项目
                mes.Value.Project = new Models.ProjectInfo();
                mes.Value.Project.ProjectID = DropDownProject.SelectedValue.ConvertTo<int>();


                //合同名称
                mes.Value.ContractName = TextBoxContractName.Text;

                //合同编码
                mes.Value.ContractCode = TextBoxContractCode.Text;

                //合同金额
                if (TextBoxValue.Text.CanConvertTo<double>())
                    mes.Value.ContractValue = TextBoxValue.Text.ConvertTo<double>();
                else mes.Value.ContractValue = 0.0;

                //备注
                mes.Value.Comment = TextBoxComment.Text;

                //归档
                mes.Value.Filing = TextBoxFiling.Text;

                //签约时间
                mes.Value.ContractDate = ContractDate.Value.ConvertTo<DateTime>();

                //创建人
                string username = Page.User.Identity.Name;
                if (String.IsNullOrEmpty(username))
                    mes.Value.Creator = "anonymous";
                else mes.Value.Creator = username;
                mes.Value.CreateDate = System.DateTime.Now;
                mes.retType = 100;
                mes.Message = "获取ProjectInfo对象成功!";
            }
            else
            {
                mes.retType = 1;
                mes.Message = "新增参数供方类型或状态错误";
            }


            //modify contract needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (ContractID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>())
                {
                    //隐藏控件－合同ID不能变－
                    mes.Value.ContractID = ContractID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();


                    //修改人
                    string username = Page.User.Identity.Name;
                    if (String.IsNullOrEmpty(username))
                        mes.Value.Modifier = "anonymous";
                    else mes.Value.Modifier = username;
                    mes.Value.ModifyDate = System.DateTime.Now;

                    mes.retType = 200;
                }
                else
                {
                    mes.retType = 2;
                    mes.Message = "修改无法获取供方ID和版本!";
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
                Services.ContractServices service = new Services.ContractServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.ContractServices service = new Services.ContractServices();
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

        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            ChangeObject(e.CommandArgument.ToString());   
        }

        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
            if (ChangeObject(e.CommandArgument.ToString()).retType == 100)
                LinkButtonBack_Command(sender, e); //保存成功!
        }

        protected void LinkButtonBack_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/CMSContract/ContractView.aspx");
        }



    }
}