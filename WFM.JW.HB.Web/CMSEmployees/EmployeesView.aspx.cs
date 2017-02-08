using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSEmployees
{
    public partial class EmployeesView : CLBasePage
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
                InitControls(null);
                GoPage(1);
            }
        }

        //初始化控件
        private void InitControls(Models.Employees employee)
        {
            Services.DepartmentsService service = new Services.DepartmentsService();
            
            DropDownListDepartment.DataSource = service.GetAllData(null);
            DropDownListDepartment.DataValueField = "DepartmentID";
            DropDownListDepartment.DataTextField = "DepartmentName";
            DropDownListDepartment.DataBind();
            
            DropDownListDepartment.Items.Insert(0, new ListItem("全部", "0")); //动态插入指定序号的新项

        }

        //获取查询参数
        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (DropDownListDepartment.SelectedValue.CanConvertTo<int>())
            {
                int DropValule = DropDownListDepartment.SelectedValue.ConvertTo<int>();
                if (DropValule != 0)
                    spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_Department.DepartmentID", Models.CriteriaOperator.Equal, DropValule));

            }

            if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, DropValule));
            }

            if (TextBoxUserName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("EmployeeName", Models.CriteriaOperator.Like, TextBoxUserName.Text));

            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewEmployees.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.EmployeeServices service = new Services.EmployeeServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            CLGridViewEmployees.DataSource = list;
            CLGridViewEmployees.DataBind();

            Pages1.PageReload();
        }


        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
        }

                
        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.Employees> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.Employees> mes = new Models.Messaging<Models.Employees>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();


            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取Employeesid
                    if (index >= 0 && index < CLGridViewEmployees.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewEmployees.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int basetypeitemid = CLGridViewEmployees.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.Employees();
                            mes.Value.EmployeeID = basetypeitemid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
           // Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }
        
        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(0);
            string para = "?id=" + mes.Message;
            //if (selectedIndex.Value.CanConvertTo<int>())
            //{
            //    para += "&new=" + selectedIndex.Value.ConvertTo<int>();
            //}
            //else 
            if (DropDownListDepartment.SelectedValue.CanConvertTo<int>())
            {
                para += "&new=" + DropDownListDepartment.SelectedValue.ConvertTo<int>();
            }
            Response.Redirect("~/CMSEmployees/EmployeesEdit.aspx" + para);
        }

        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSEmployees/EmployeesEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的数据类型
               
                ShowDialog.Value = "请选择要修改的数据类型";
            }
        }

        //双击修改
        protected void GridViewItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(null, null);

                selectedIndex.Value = "";

            }
        }


        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            if (Page.User.Identity.Name != "luwh" || Page.User.Identity.Name != "admin"|| Page.User.Identity.Name != "zhanghao")
            {
                ShowDialog.Value = "禁止删除";
                return;
            }

            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的数据项";
            if (mes.retType == 200)
            {//OK删除
                Services.EmployeeServices service = new Services.EmployeeServices();
                var retMes = service.DeleteData(mes.Value);
                mes.copy(retMes);
                GoPage(1);
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                ShowDialog.Value = mes.Message;
            }
        }




       
    }
}