using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSProject
{
    public partial class ProjectInfoEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.ProjectInfo>>("id");

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
        private void InitControls(Models.ProjectInfo info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //new 类型绑定
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "项目类型");
            spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true));
            var list = service.GetAllData(spec);

            ProjectType.DataSource = list;
            ProjectType.DataValueField = "BaseTypeItemID";
            ProjectType.DataTextField = "ItemName";
            ProjectType.DataBind();

            spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "项目状态");
            spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true));
            list = service.GetAllData(spec);

            ProjectStatus.DataSource = list;
            ProjectStatus.DataValueField = "BaseTypeItemID";
            ProjectStatus.DataTextField = "ItemName";
            ProjectStatus.DataBind();

            //修改导航
            SiteMapNode smn = SiteMap.Provider.FindSiteMapNode("~/CMSProject/ProjectInfoEdit.aspx");//获得当前节点
            smn.ReadOnly = false;//将当前节点设置为只读
            smn.Title = "新增项目";//为当前节点title赋值 

            if (info != null)
            {//modify init
                Services.ProjectInfoServices project = new Services.ProjectInfoServices();
                spec = new Models.CriteriaSpecification("ProjectID", Models.CriteriaOperator.Equal, info.ProjectID);
                var projectlist = project.GetAllData(spec);

                if (projectlist.Count() != 0)
                { 
                    var projectinfo = projectlist.First();

                    //id和version
                    ProjectID.Value = projectinfo.ProjectID.ToString();
                    Version.Value = projectinfo.version.ToString();

                    ProjectNameTextBox.Text = projectinfo.ProjectName;
                    ProjectShortNameTextBox.Text = projectinfo.ProjectShortName;
                    ProjectType.SelectedValue = projectinfo.ProjectType.BaseTypeItemID.ToString();
                    ProjectStatus.SelectedValue = projectinfo.Status.BaseTypeItemID.ToString();

                    //时间
                    ProjectStartDate.Value = projectinfo.ProjectStartDate.ToShortDateString();
                    ProjectEndDate.Value = projectinfo.ProjectEndDate.ToShortDateString();

                    ProjectCommentTextBox.Text = projectinfo.Comment;


                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";

                    //修改导航
                    smn.Title = "修改项目";//为当前节点title赋值 
                }
            }
        }


         //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.ProjectInfo> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.ProjectInfo>();
            mes.Value = new Models.ProjectInfo();
            mes.retType = 1;

            //new supplier needed
            if (ProjectType.SelectedValue.CanConvertTo<int>() && ProjectStatus.SelectedValue.CanConvertTo<int>()
                && ProjectStartDate.Value.CanConvertTo<DateTime>() && ProjectEndDate.Value.CanConvertTo<DateTime>())
            {
                //项目类型ID
                mes.Value.ProjectType = new Models.TypeItem();
                mes.Value.ProjectType.BaseTypeItemID = ProjectType.SelectedValue.ConvertTo<int>();
                //项目状态ID
                mes.Value.Status = new Models.TypeItem();
                mes.Value.Status.BaseTypeItemID = ProjectStatus.SelectedValue.ConvertTo<int>();
                
                //项目名称
                mes.Value.ProjectName = ProjectNameTextBox.Text;
                mes.Value.ProjectShortName = ProjectShortNameTextBox.Text;
                //备注
                mes.Value.Comment = ProjectCommentTextBox.Text;

                //时间
                mes.Value.ProjectStartDate = ProjectStartDate.Value.ConvertTo<DateTime>();
                mes.Value.ProjectEndDate = ProjectEndDate.Value.ConvertTo<DateTime>();

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
                mes.Message = "新增参数项目类型或状态错误";
            }


            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if(ProjectID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>())
                {
                    //隐藏控件－项目ID不能变－
                    mes.Value.ProjectID = ProjectID.Value.ConvertTo<int>();
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
                    mes.Message = "修改无法获取项目ID和版本!";
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
                Services.ProjectInfoServices service = new Services.ProjectInfoServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.ProjectInfoServices service = new Services.ProjectInfoServices();
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
            Response.Redirect("~/CMSProject/ProjectInfoView.aspx");
        }



    }
}