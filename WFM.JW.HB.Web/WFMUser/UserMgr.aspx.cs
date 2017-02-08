using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMUser
{
    public partial class UserMgr : CLBasePage
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
                //var mes = GetUrlPara<Models.Messaging<Models.RegionUser>>("ShowMessage");
                //if (mes.Value != null && mes.Value.Value != null)
                //    if(mes.retType == 100)
                //        ShowDialog.Value = mes.Message;

                InitDropList();
                GoPage(1);
            }
            else
            {
                
            }
        }

        private void InitDropList()
        {
            if (CurrentUserRegion == null) return;
           
            InitCityList(regionList, CurrentUserRegion);
        }

        private void InitCityList(IEnumerable<Models.Region> rlist, Models.Region r)
        {
            var citylist = rlist.ToList()
               .Where(x => (x.ParentGuid == CurrentUserRegion.RegionGuid || x.RegionGuid == CurrentUserRegion.RegionGuid));

            DropDownListCity.DataSource = citylist;

            DropDownListCity.DataValueField = "RegionGuid";
            DropDownListCity.DataTextField = "RegionName";
            DropDownListCity.DataBind();

            DropDownListCity.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
        }        

        private void InitContryList()
        {
            if (DropDownListCity.SelectedIndex == 0)
            {
                DropDownListContry.Items.Clear();
                DropDownListContry.DataSource = null;

                return;
            }
            Services.WFMRegionServices rs = new Services.WFMRegionServices();
            var list = rs.GetAllData(new Models.CriteriaSpecification("a.ParentGuid", Models.CriteriaOperator.Equal, DropDownListCity.SelectedValue));

            DropDownListContry.DataSource = list.ToList().Where(x => x.RegionName != null).Where(x => x.RegionName.Length > 0);
            DropDownListContry.DataValueField = "RegionGuid";
            DropDownListContry.DataTextField = "RegionName";
            DropDownListContry.DataBind();
            DropDownListContry.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
        }

        //Droplist 联动
        protected void CityList_IndexChanged(object sender, EventArgs e)
        {
            InitContryList();
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

            Response.Redirect("~/WFMUser/UserEdit.aspx?id=" + mes.Message);

        }

        //修改
        //传递 参数,绑定类型
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/WFMUser/UserEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的供方
                //showWarning("请选择要修改的供方");
                ShowDialog.Value = "请选择要修改的用户";
            }
        }

        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            mes.Message = "请选择要删除的用户";
            if (mes.retType == 200)
            {//OK删除

                if (mes.Value.UserName == Page.User.Identity.Name)
                    mes.Message = "当前用户不允许删除！";
                else
                if (mes.Value.UserName == "admin")
                    mes.Message = "admin 不允许删除！";
                else 
                {
                    Services.WFMRegionUserServices service = new Services.WFMRegionUserServices();
                    var retMes = service.DeleteData(mes.Value);
                    mes.copy(retMes);

                    GoPage(1);
                } 
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                ShowDialog.Value = mes.Message;
                // showWarning(mes.Message);
            }
        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            if (CurrentUserRegion == null) return Models.CriteriaSpecification.GetFalse();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            string Guid = "";
            
            if (DropDownListCity.SelectedIndex > 0)
            {
                if (DropDownListContry.SelectedIndex > 0)
                    Guid = DropDownListContry.SelectedValue;
                else Guid = DropDownListCity.SelectedValue;
            }
            else
            {
                Guid = CurrentUserRegion.RegionGuid;
            }

            if(!base.IsRegionAuthentication(Guid)) //权限控制
                return Models.CriteriaSpecification.GetFalse();

            Models.Region SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if(SelectedRegion == null) return Models.CriteriaSpecification.GetFalse();

            if (SelectedRegion.Level != 0)
            {
                Models.IParametersSpecification newspec = new Models.CriteriaSpecification("RegionGUID", Models.CriteriaOperator.Equal, Guid);
                newspec = newspec.Or(new Models.CriteriaSpecification("ParentGUID", Models.CriteriaOperator.Equal, Guid));
                spec = spec.And(newspec);
            }

            return spec;
        } 

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewUserMgrInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.WFMRegionUserServices service = new Services.WFMRegionUserServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);

            var showlist = from l in list
                           join r in regionList on l.RegionGuid equals r.RegionGuid
                           select new
                           {
                               RegionGuid = l.RegionGuid,
                               RowID = l.RowID,
                               Version = l.version,
                               UserName = l.UserName,
                               ParentName = r.ParentName,
                               RegionName = r.RegionName,

                               LastActivityDate = l.LastActivityDate.ToLocalTime(),
                               RegionUserID = l.RegionUserID,

                               Level = r.Level,
                               seq = r.Seq,
                               Createby = l.Createby
                           } 
                           ;

            CLGridViewUserMgrInfo.DataSource = showlist.ToList().OrderBy(x => x.Level).OrderBy(x => x.seq);
            CLGridViewUserMgrInfo.DataBind();

            Pages1.PageReload();
        }

        /**
        * type = 0 新增
        * type = 1 修改  
        * 
        * mes.Value = null 时 
        * */
        private Models.Messaging<Models.RegionUser> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.RegionUser> mes = new Models.Messaging<Models.RegionUser>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewUserMgrInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        //if (CLGridViewUserMgrInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            //int RowID = CLGridViewUserMgrInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.RegionUser();
                            mes.Value.RegionUserID = CLGridViewUserMgrInfo.DataKeys[index].Value.ToString();

                            var r = userList.ToList().Find(x => x.RegionUserID == mes.Value.RegionUserID);
                            if(base.IsRegionAuthentication(r.RegionGuid))
                            {
                                mes.retType = 200;
                                mes.Value.version = r.version;
                                mes.Value.UserName = r.UserName;
                            }

                           
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }


        protected void CLGridViewRegionUserInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(null, null);
              //  LinkButtonDetails_Click(null, null);

            }
        }

        

    }
}