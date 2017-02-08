using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSDepartment
{
    public partial class DepartmentEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.Departments>>("id");

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
        private void InitControls(Models.Departments info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //new 类型绑定

            if (info != null)
            {//modify init
                Services.DepartmentsService project = new Services.DepartmentsService();
                var spec = new Models.CriteriaSpecification("DepartmentID", Models.CriteriaOperator.Equal, info.DepartmentID);
                var departmentlist = project.GetAllData(spec);

                if (departmentlist.Count() != 0)
                {
                    var departmentinfo = departmentlist.First();

                    //id和version
                    DepartmentID.Value = departmentinfo.DepartmentID.ToString();
                    Version.Value = departmentinfo.version.ToString();

                    DepartmentNameTextBox.Text = departmentinfo.DepartmentName;

                    DepartmentEnable.Visible = true;
                    DepartmentEnable.SelectedIndex = departmentinfo.Enable ? 1 : 0;
                    LabelEnable.Visible = true;


                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";
                }
            }
        }


        //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.Departments> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.Departments>();
            mes.Value = new Models.Departments();
            mes.retType = 1;

            //new supplier needed


            //部门名称
            mes.Value.DepartmentName = DepartmentNameTextBox.Text;

            //创建人
            string username = Page.User.Identity.Name;
            if (String.IsNullOrEmpty(username))
                mes.Value.Creator = "anonymous";
            else mes.Value.Creator = username;
            mes.Value.CreateDate = System.DateTime.Now;
            mes.retType = 100;
            mes.Message = "获取Department对象成功!";


            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (DepartmentID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>() &&
                    DepartmentEnable.SelectedValue.CanConvertTo<bool>() )
                {
                    //隐藏控件－部门ID不能变－
                    mes.Value.DepartmentID = DepartmentID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();

                    mes.Value.Enable = DepartmentEnable.SelectedValue.ConvertTo<bool>();

                    //修改人
                    username = Page.User.Identity.Name;
                    if (String.IsNullOrEmpty(username))
                        mes.Value.Modifier = "anonymous";
                    else mes.Value.Modifier = username;
                    mes.Value.ModifyDate = System.DateTime.Now;

                    mes.retType = 200;
                }
                else
                {
                    mes.retType = 2;
                    mes.Message = "修改无法获取部门ID和版本!";
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
                Services.DepartmentsService service = new Services.DepartmentsService();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.DepartmentsService service = new Services.DepartmentsService();
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
            Response.Redirect("~/CMSDepartment/DepartmentView.aspx");
        }

    }
}