using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMTask
{
    public partial class TaskEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LabelInfo.Text = "";
                var mes = GetUrlPara<Models.Messaging<Models.WFMUploadTask>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                    Response.Redirect("~/WFMTask/TaskMgr.aspx");
                }
                else
                {//modify
                    InitControls(mes.Value.Value);
                }
            }
        }


       
        //初始化控件
        private void InitControls(Models.WFMUploadTask info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            //LinkButtonSave.CommandArgument = "New";


            ////修改导航
            //SiteMapNode smn = SiteMap.Provider.FindSiteMapNode("~/CMSProject/ProjectInfoEdit.aspx");//获得当前节点
            //smn.ReadOnly = false;//将当前节点设置为只读
            //smn.Title = "新增项目";//为当前节点title赋值 

            //new 上级单位绑定
            InitCityList();

            if (info != null)
            {//modify                
                Services.WFMUploadTaskServices wts = new Services.WFMUploadTaskServices();
                var t = wts.GetDatabyGuid(info.TaskGuid);
                if(t == null)
                    Response.Redirect("~/WFMTask/TaskMgr.aspx");

                var r = userList.ToList().Find(x => x.UserName == t.UserName.Trim());
                if (r != null)
                {
                    if (r.ParentGuid == CurrentUserRegion.ParentGuid)
                    {//省
                        DropDownListCity.SelectedValue = r.RegionGuid;
                    }
                    else
                    {
                        DropDownListCity.DataSource = regionList.ToList(); ;
                        DropDownListCity.SelectedValue = r.ParentGuid;

                        InitContryList();

                        DropDownListContry.SelectedValue = r.RegionGuid;
                       
                    }

                    DropDownListCity.Enabled = false;
                    DropDownListContry.Enabled = false;

                    
                    DropDownListData.Items.Add(new ListItem("是", "1"));
                    DropDownListData.Items.Add(new ListItem("否", "0"));                   
                    
                    DropDownListData.SelectedValue = t.TStatus.ToString();


                    TextBoxCreateTime.Text = t.CreateTime.ToString();
                    TextBoxTaskName.Text = t.ClientTaskName;

                    string strRet = "";
                    switch (t.Status)
                    {
                        case 0:
                            strRet = "正在上传";
                            break;
                        case 5:
                            strRet = "上传失败";
                            break;
                        case 10:
                            strRet = "上传成功";
                            break;
                        case 15:
                            strRet = "数据分析错误";
                            break;
                        case 20:
                            strRet = "数据正常";
                            break;
                    }
                    TextBoxStatus.Text = strRet;

                    TextBoxCreateTime.Enabled = false;
                    TextBoxTaskName.Enabled = false;
                    TextBoxStatus.Enabled = false;

                    TaskGuid.Value = t.TaskGuid;
                    Version.Value = t.Version.ToString();

                    if (t.Status < 10)
                        LinkButtonApply.Enabled = false;


                    LinkButtonApply.CommandArgument = "Modify";
                  //  LinkButtonSave.CommandArgument = "Modify"; 
                   
                   // Version.Value = info.Version.ToString();
                }
            }

        }

        //绑定上级单位
        private void InitCityList()
        { 
            var citylist = regionList.ToList()
               .Where(x => (x.ParentGuid == CurrentUserRegion.RegionGuid || x.RegionGuid == CurrentUserRegion.RegionGuid));

            DropDownListCity.DataSource = citylist;

            DropDownListCity.DataValueField = "RegionGuid";
            DropDownListCity.DataTextField = "RegionName";
            DropDownListCity.DataBind();

            DropDownListCity.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
        }

        //绑定单位
        private void InitContryList()
        {
            if (DropDownListCity.SelectedIndex > 0)
            {
                string CityValule = DropDownListCity.SelectedValue;
                DropDownListContry.DataSource = regionList.ToList()
                                               .Where(x => x.ParentGuid == CityValule);

                DropDownListContry.DataValueField = "RegionGuid";
                DropDownListContry.DataTextField = "RegionName";
                DropDownListContry.DataBind();
                DropDownListContry.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
            }
            else
            {
                DropDownListContry.Items.Clear();
                DropDownListContry.DataSource = null;
            }
        }

        //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.WFMUploadTask> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.WFMUploadTask>();
            mes.Value = new Models.WFMUploadTask();
            mes.retType = 1;

            mes.Value.TaskGuid = TaskGuid.Value.ToString();

            Int64 ver = 0;
            Int64.TryParse(Version.Value.ToString(), out ver);
            mes.Value.Version = ver;

            int status = 0;
            int.TryParse(DropDownListData.SelectedValue, out status);
            mes.Value.TStatus = status;

            mes.retType = 200;// modify      
            return mes; 
        }

        private Models.Messaging<int> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;
            var mes = GenerateObject(type);

           if (mes.retType == 200)
            {//成功获取修改对象
                //Services.ContractServices service = new Services.ContractServices();
                //retMes = service.UpdateData(mes.Value);
                //only one TStatus = 10
                Services.WFMUploadTaskServices wts = new Services.WFMUploadTaskServices();
                retMes = wts.SetDefaultData(mes.Value);
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