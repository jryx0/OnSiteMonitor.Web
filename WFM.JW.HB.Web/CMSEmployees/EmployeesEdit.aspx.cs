using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSEmployees
{
    public partial class EmployeesEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.Employees>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new

                    var id = GetUrlPara<int>("new", false);
                    if (mes.retType == 100)
                    {
                        Models.Employees employee = new Models.Employees();
                        employee.Department = new Models.Departments();
                        employee.Department.DepartmentID = id.Value;

                        InitControls(true, employee);
                    }
                    else
                        InitControls(true, null);
                }
                else
                {//modify
                    InitControls(false, mes.Value.Value);
                }
            }
        }

        //初始化控件
        private void InitControls(bool edittype, Models.Employees employee)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //new 部门绑定
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, true);
            Services.DepartmentsService service = new Services.DepartmentsService();
                      
            var departmentlist = service.GetAllData(spec);
            DropDownListDepartment.DataSource = departmentlist;
            DropDownListDepartment.DataValueField = "DepartmentID";
            DropDownListDepartment.DataTextField = "DepartmentName";
            DropDownListDepartment.DataBind();
            if (edittype)//news
            {
                if (employee != null && employee.Department != null && employee.Department.DepartmentID != 0)
                    DropDownListDepartment.SelectedValue = employee.Department.DepartmentID.ToString();
                else
                    DropDownListDepartment.Items.Insert(0, new ListItem("---", "0")); //动态插入指定序号的新项
            }  

            EnableDropDownList.SelectedValue = "1";

            if (Page.User.Identity.Name != "luwh" && Page.User.Identity.Name != "admin" && Page.User.Identity.Name != "zhanghao")
            {
                DropDownListUser.Visible = false;
                LabelSystemUser.Visible = false;
                LabelUserName.Visible = false;
                TextBoxUserName.Visible = false;
                EnableLabel.Visible = false;
                EnableDropDownList.Visible = false;
            }

            DropDownListUser.SelectedValue = "0";
        

            if (!edittype)
            {//修改

                Services.EmployeeServices employeeservice = new Services.EmployeeServices();
                spec = new Models.CriteriaSpecification("EmployeeID", Models.CriteriaOperator.Equal, employee.EmployeeID);
                var employeelist = employeeservice.GetAllData(spec);

                if (employeelist.Count() != 0)
                {
                    var firstemployee = employeelist.First();
                    //id和version
                    EmployeeID.Value = firstemployee.EmployeeID.ToString();
                    Version.Value = firstemployee.version.ToString();   
                    
                    //显示当前数据类型
                   // 
                    if(DropDownListDepartment.SelectedValue == "0")
                        DropDownListDepartment.Items.RemoveAt(0);
                    DropDownListDepartment.SelectedValue = firstemployee.Department.DepartmentID.ToString();
                          
                    TextBoxName.Text = firstemployee.EmployeeName;
                    EnableDropDownList.SelectedValue = firstemployee.Enable ? "1" : "0";
                   
                    //是否系统用户
                    DropDownListUser.SelectedValue = firstemployee.IsUser ? "1" : "0";
                    TextBoxUserName.Text = firstemployee.UserName;

                    if (Page.User.Identity.Name == "luwh" || Page.User.Identity.Name == "admin" || Page.User.Identity.Name == "zhanghao")
                    {
                        EnableDropDownList.Visible = true;
                        EnableLabel.Visible = true;
                    }
                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";
                }
            }
        }

        //生成ItemType对象
        //type  新增 or 修改
        private Models.Messaging<Models.Employees> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.Employees>();
            mes.Value = new Models.Employees();
            mes.retType = 1;

            //new  needed
            if (DropDownListDepartment.SelectedValue.CanConvertTo<int>() &&
                DropDownListUser.SelectedValue.CanConvertTo<bool>())
            {
                //类型ID
                mes.Value.Department = new Models.Departments();
                mes.Value.Department.DepartmentID = DropDownListDepartment.SelectedValue.ConvertTo<int>();

                //状态,禁用启用
                mes.Value.Enable = true;

                //用户名名称
                mes.Value.EmployeeName = TextBoxName.Text;
                mes.Value.IsUser = DropDownListUser.SelectedValue.ConvertTo<bool>();
                mes.Value.UserName = TextBoxUserName.Text;

                //创建人
                string username = Page.User.Identity.Name;
                if (String.IsNullOrEmpty(username))
                    mes.Value.Creator = "anonymous";
                else mes.Value.Creator = username;
                mes.Value.CreateDate = System.DateTime.Now;
                mes.retType = 100;
                mes.Message = "获取ItemType对象成功!";
            }
            else
            {
                mes.retType = 1;
                mes.Message = "新增参数用户类型或状态错误";
            }


            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (EmployeeID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>() &&
                    EnableDropDownList.SelectedValue.CanConvertTo<bool>())
                {
                    //隐藏控件－用户ID不能变－
                    mes.Value.EmployeeID = EmployeeID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();

                    //状态,禁用启用
                    mes.Value.Enable = EnableDropDownList.SelectedValue.ConvertTo<bool>();
                    



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
                    mes.Message = "修改无法获取用户ID和版本!";
                }
            }

            return mes;
        }


        private Models.Messaging<Models.Employees> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;
            var mes = GenerateObject(type);

            if (mes.retType == 100)
            {//新增对象获取成功
                Services.EmployeeServices service = new Services.EmployeeServices();
                retMes = service.InsertData(mes.Value);
                mes.Message = retMes.Message;
                mes.retType = retMes.retType;
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.EmployeeServices service = new Services.EmployeeServices();
                retMes = service.UpdateData(mes.Value);
                mes.Message = retMes.Message;
                mes.retType = retMes.retType;
            }
            LabelInfo.Text = mes.Message;

            return mes;
        }

              

        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            ChangeObject(e.CommandArgument.ToString());   
        }

        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
            var mes = ChangeObject(e.CommandArgument.ToString());
            if (mes.retType == 100)
            {
                LinkButtonBack_Click(mes.Value, e); //保存成功!
            }
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/CMSEmployees/EmployeesView.aspx");
        }
    }
}