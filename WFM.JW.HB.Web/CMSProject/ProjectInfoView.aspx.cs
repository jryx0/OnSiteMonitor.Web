using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSProject
{
    public partial class ProjectInfoView : CLBasePage
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
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();

            //项目类型
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "项目类型");
            var list = service.GetAllData(spec);

            DropDownListProjectType.DataSource = list;
            DropDownListProjectType.DataValueField = "BaseTypeItemID";
            DropDownListProjectType.DataTextField = "ItemName";
            DropDownListProjectType.DataBind();

            DropDownListProjectType.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

            //项目状态
            spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "项目状态");
            list = service.GetAllData(spec);


            DropDownListStatus.DataSource = list;
            DropDownListStatus.DataValueField = "BaseTypeItemID";
            DropDownListStatus.DataTextField = "ItemName";
            DropDownListStatus.DataBind();

            DropDownListStatus.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项

        }

        //初始化GridView
        void InitGridView(Models.IParametersSpecification spec)
        {
            Services.ProjectInfoServices service = new Services.ProjectInfoServices();
            CLGridViewProjectInfo.DataSource = service.GetAllData(spec);

            CLGridViewProjectInfo.DataBind();
        }

        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.ProjectInfo> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.ProjectInfo> mes = new Models.Messaging<Models.ProjectInfo>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewProjectInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewProjectInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int projectid = CLGridViewProjectInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.ProjectInfo();
                            mes.Value.ProjectID = projectid;
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

            if (this.TextBoxProjectName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("ProjectName", Models.CriteriaOperator.Like, this.TextBoxProjectName.Text));


            if (DropDownListStatus.SelectedValue.CanConvertTo<int>())
            {
                int DropValule = DropDownListStatus.SelectedValue.ConvertTo<int>();
                spec = spec.And(new Models.CriteriaSpecification("StatusID", Models.CriteriaOperator.Equal, DropValule));
            }

            if (DropDownListProjectType.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListProjectType.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("ProjectTypeID", Models.CriteriaOperator.Equal, DropValule));
            }

            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(CLGridViewProjectInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.ProjectInfoServices service = new Services.ProjectInfoServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            CLGridViewProjectInfo.DataSource = list;
            CLGridViewProjectInfo.DataBind();

            Pages1.PageReload();
        }

        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
        }

        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(0);

            Response.Redirect("~/CMSProject/ProjectInfoEdit.aspx?id=" + mes.Message);

        }
        
        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSProject/ProjectInfoEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的项目
                ShowDialog.Value = "请选择要修改的工程项目";
            }
        }

        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的项目";
            if (mes.retType == 200)
            {//OK删除
                Services.ProjectInfoServices service = new Services.ProjectInfoServices();
                var retMes = service.DeleteData(mes.Value);

                //InitGridView(null);
                GoPage(1);
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                ShowDialog.Value = mes.Message;
            }
        }

        protected void CLGridViewProjectInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonDetails_Click(null, null);
                //Response.Redirect(@"~/CMSContract/ContractView.aspx");
                
            }
        }

        protected void LinkButtonDetails_Click(object sender, EventArgs e)
        {
            if (selectedIndex.Value.CanConvertTo<int>())
            {
                var mes = GenereatePageParameter(1);

                if (mes.retType == 200)
                {//ok
                    Response.Redirect(@"~/CMSContract/ContractView.aspx?new=" + mes.Value.ProjectID.ToString());
                }
            }
             //   Response.Redirect(@"~/CMSContract/ContractView.aspx?new=" + selectedIndex.Value);
            Response.Redirect(@"~/CMSContract/ContractView.aspx");
                    
        }

    }
}