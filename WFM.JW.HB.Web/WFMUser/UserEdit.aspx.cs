using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMUser
{
    public partial class UserEdit : CLBasePage
    {       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.RegionUser>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                    InitControls(null);
                   // oldpass.Style.Add("display", "none");
                    PasswordTextBox.Text = "密码";
                }
                else
                {//modify
                    InitControls(mes.Value.Value);
                  //  oldpass.Style.Add("display", "block");
                    PasswordLabel.Text = "输入重置密码";
                }              
            }
        }


        //初始化控件
        private void InitControls(Models.RegionUser info)
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
                var r = userList.ToList().Find(x => x.RegionUserID == info.RegionUserID);
                if (r != null)
                {                    
                    if (r.ParentGuid == CurrentUserRegion.ParentGuid)
                    {//省
                        DropDownListCity.SelectedValue = r.RegionGuid;
                        UserNameTextBox.Text = r.UserName;
                    }
                    else
                    {
                        DropDownListCity.DataSource = regionList.ToList(); ;
                        DropDownListCity.SelectedValue = r.ParentGuid;

                        InitContryList();
                       
                        DropDownListContry.SelectedValue = r.RegionGuid;
                        UserNameTextBox.Text = r.UserName;
                    }

                    DropDownListCity.Enabled = false;
                    DropDownListContry.Enabled = false;
                    UserNameTextBox.Enabled = false;
                    LinkButtonApply.CommandArgument = "Modify";
                    // LinkButtonSave.CommandArgument = "Modify";

                    RegionUserGuid.Value = info.RegionUserID;
                    Version.Value = info.version.ToString();
                }
            }

        }              

        //绑定上级单位
        private void InitCityList()
        {
            //DropDownListCity.DataSource = regionList.ToList()
            //                                    .Where(x => (x.ParentID == CurrentUserRegion.RowID || x.RowID == CurrentUserRegion.RowID)
            //                                     );                  
            //DropDownListCity.DataValueField = "RowID";
            //DropDownListCity.DataTextField = "RegionName";
            //DropDownListCity.DataBind();

            //DropDownListCity.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 

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
            if(DropDownListCity.SelectedIndex > 0)
            {
                string CityValule = DropDownListCity.SelectedValue;
                DropDownListContry.DataSource = regionList.ToList()
                                               .Where(x => x.ParentGuid == CityValule);

                DropDownListContry.DataValueField = "RegionGuid";
                DropDownListContry.DataTextField = "RegionName";
                DropDownListContry.DataBind();
                DropDownListContry.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
            }


            //if (DropDownListCity.SelectedValue.CanConvertTo<int>())
            //{//query     
            //    int CityValule = DropDownListCity.SelectedValue.ConvertTo<int>();

            //    DropDownListContry.DataSource = regionList.ToList()
            //                                   .Where(x => x.ParentID == CityValule);
            //    DropDownListContry.DataValueField = "RowID";
            //    DropDownListContry.DataTextField = "RegionName";
            //    DropDownListContry.DataBind();
            //    DropDownListContry.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
            //}
            else
            {
                DropDownListContry.Items.Clear();
                DropDownListContry.DataSource = null;
            }
        }

        private Models.Messaging<int> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;
            var mes = GenerateObject(type);

            if (mes.retType == 100)
            {//新增对象获取成功
                Services.WFMRegionUserServices service = new Services.WFMRegionUserServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.WFMRegionUserServices service = new Services.WFMRegionUserServices();
                retMes = service.UpdateData(mes.Value);
            }
            else
            {//错误
                retMes = new Models.Messaging<int>();
                retMes.copy(mes);
            }
            //LabelInfo.Text = retMes.Message;

            return retMes;
        }

        //生成对象
        //type  新增 or 修改
        private Models.Messaging<Models.RegionUser> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.RegionUser>();
            mes.Value = new Models.RegionUser();
            mes.retType = -1;

            //获取当期页面上的数据：
            Models.RegionUser userRegion = new Models.RegionUser();
            userRegion.UserName = UserNameTextBox.Text.ToLower();
            userRegion.Password = PasswordTextBox.Text;
            userRegion.Createby = Page.User.Identity.Name;

            if (DropDownListContry.SelectedIndex > 0)
            {
                //单位   
                userRegion.RegionGuid = DropDownListContry.SelectedValue.ToString();
                userRegion.ParentGuid = DropDownListCity.SelectedValue.ToString();
                mes.retType = 100;
            }
            else if (DropDownListCity.SelectedIndex > 0)
            {                
                userRegion.RegionGuid = DropDownListCity.SelectedValue.ToString();
              //  if (base.IsRegionAuthentication(userRegion.RegionGuid))
                {
                    var r = regionList.ToList().Find(x => x.RegionGuid == userRegion.RegionGuid);
                    if (r != null)
                    {
                        userRegion.ParentGuid = r.ParentGuid;
                        mes.retType = 100;
                    }
                }
            }
            
            //modify needed
            if (mes.retType == 100 && type == "Modify")
            {
                mes.retType = 2;
                mes.Message = "修改无法获取项目ID和版本!";
                userRegion.RegionUserID = RegionUserGuid.Value;
                if (Version.Value.CanConvertTo<Int64>())
                { 
                    userRegion.version = Version.Value.ConvertTo<Int64>();
                    mes.retType = 200;
                    mes.Message = "成功获取用户区域";
                }
            }

            if (mes.retType == 200 || mes.retType == 100) //modify or new 
                if (!base.IsRegionAuthentication(userRegion.RegionGuid)) //权限控制
                    mes.retType = -1;

            mes.Value = userRegion;
            return mes;
        }

        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            var mes = ChangeObject(e.CommandArgument.ToString());

            LabelInfo.Text = mes.Message;
        }

        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
             var mes = ChangeObject(e.CommandArgument.ToString());
             LabelInfo.Text = mes.Message;
            if (mes.retType == 100)
            {
                Page.Session[mes.Message] = mes;                
                Response.Redirect("~/WFMUser/UserMgr.aspx?ShowMessage=" + mes.Message); //保存成功!
            }            
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WFMUser/UserMgr.aspx");
        }

        protected void CityList_IndexChanged(object sender, EventArgs e)
        {
            InitContryList();
        }
    }
}