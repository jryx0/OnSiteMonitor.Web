using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSDepartment
{
    public partial class DepartmentView : CLBasePage
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
                //初始化绑定                
                InitControls();
                //InitGridView(null);
                GoPage(1);
            }
        }

        //初始化控件
        private void InitControls()
        {            
        }

        //初始化GridView
        void InitGridView(Models.IParametersSpecification spec)
        {
            Services.DepartmentsService service = new Services.DepartmentsService();
            CLGridViewDepartment.DataSource = service.GetAllData(spec);

            CLGridViewDepartment.DataBind();
        }


        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (this.TextBoxDepartmentName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("DepartmentName", Models.CriteriaOperator.Like, this.TextBoxDepartmentName.Text));


            if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, DropValule));
            }
            return spec;

        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(CLGridViewDepartment.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.DepartmentsService service = new Services.DepartmentsService();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            CLGridViewDepartment.DataSource = list;
            CLGridViewDepartment.DataBind();

            Pages1.PageReload();
        }

        //查询Query
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (this.TextBoxDepartmentName.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("DepartmentName", Models.CriteriaOperator.Like, this.TextBoxDepartmentName.Text));


            //if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            //{
            //    bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
            //    spec = spec.And(new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, DropValule));
            //}
            //var spec = GetQuerySpec();
            //InitGridView(spec);
            GoPage(1);
        }



        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.Departments> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.Departments> mes = new Models.Messaging<Models.Departments>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewDepartment.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewDepartment.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int departmentid = CLGridViewDepartment.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.Departments();
                            mes.Value.DepartmentID = departmentid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }



        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(0);

            Response.Redirect("~/CMSDepartment/DepartmentEdit.aspx?id=" + mes.Message);
        }

        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSDepartment/DepartmentEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的部门
                //showWarning();
                ShowDialog.Value = "请选择要修改的部门";
            }
        }

        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的部门";
            if (mes.retType == 200)
            {//OK删除
                Services.DepartmentsService service = new Services.DepartmentsService();
                var retMes = service.DeleteData(mes.Value);

                mes.copy(retMes);
                //InitGridView(null);
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