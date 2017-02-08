using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using TB.ComponentModel;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Web.WFMClues
{
    public partial class ClueSummaryRegion : CLBasePage
    {
        private Models.Region SelectedRegion;
        protected override void OnInit(EventArgs e)
        {
            this.Pages1.GoPage += new GoPageEventHandler(GoPage);
            base.OnInit(e);
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitDropList();

                SelectedRegion = CurrentUserRegion;                
                var mes = GetUrlPara<Models.Messaging<Models.Region>>("id");
                if (mes.Value != null && mes.Value.Value != null)
                {
                    if (mes.retType == 100)
                    {
                        //ShowDialog.Value = mes.Message;
                        SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == mes.Value.Value.RegionGuid.ToUpper());
                         
                        if (SelectedRegion == null || !base.IsRegionAuthentication(SelectedRegion.RegionGuid))
                            SelectedRegion = CurrentUserRegion;
                        
                        if (SelectedRegion.Level == 2 || SelectedRegion.DirectCity == 1)
                        {
                            DropDownListCity.SelectedValue = SelectedRegion.ParentGuid;
                            InitContryList();

                            DropDownListContry.SelectedValue = SelectedRegion.RegionGuid;
                        }                     
                    }
                }          
                GoPage(1);
            }
            else
            {
                //Response.Redirect("~/WFMClues/ClueReport.aspx");
            }
        }
        private void InitDropList( )
        {
            if (CurrentUserRegion == null) return;
           
            InitCityList(regionList, CurrentUserRegion);
            InitItem();
        }
        private void InitItem()
        {
            //Services.BaseTypeItemServices bi = new Services.BaseTypeItemServices();
            //var iList = bi.GetAllData();

            //DropDownListItem.DataSource = iList.Where(x=> x.ItemParent.BaseTypeID == 15).OrderBy( x => x.ItemOrder);
            //DropDownListItem.DataValueField = "ItemValue";
            //DropDownListItem.DataTextField = "ItemName";
            //DropDownListItem.DataBind();
            //DropDownListItem.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
        }
        private void InitCityList(IEnumerable<Models.Region> rlist, Models.Region r)
        {
            var citylist = rlist.ToList()
               .Where(x => (x.ParentGuid == CurrentUserRegion.RegionGuid || x.RegionGuid == CurrentUserRegion.RegionGuid));

            if (citylist == null) return;

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

            if (list == null) return;

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
            GetQuerySpec();

            GoPage(1);            
        }

        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {

            //var mes = GenereatePageParameter(0);

            //Response.Redirect("~/WFMUser/UserEdit.aspx?id=" + mes.Message);

        }    //修改
        //传递 参数,绑定类型
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            //var mes = GenereatePageParameter(1);

            //if (mes.retType == 200)
            //{
            //    Response.Redirect("~/WFMUser/UserEdit.aspx?id=" + mes.Message);
            //}
            //else
            //{//警告修改错误,请选择要修改的供方
            //    //showWarning("请选择要修改的供方");
            //    ShowDialog.Value = "请选择要修改的用户";
            //}
        }        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            
            //var mes = GenereatePageParameter(1);

            //mes.Message = "请选择要删除的用户";
            //if (mes.retType == 200)
            //{//OK删除

            //    if (mes.Value.UserName == Page.User.Identity.Name)
            //        mes.Message = "当前用户不允许删除！";
            //    else
            //    if (mes.Value.UserName == "admin")
            //        mes.Message = "admin 不允许删除！";
            //    else 
            //    {
            //        Services.WFMRegionUserServices service = new Services.WFMRegionUserServices();
            //        var retMes = service.DeleteData(mes.Value);
            //        mes.copy(retMes);

            //        GoPage(1);
            //    } 
            //}

            //if (mes.retType != 100)
            //{//警告删除出错
            //    //Response.Write("<script>alert('" + mes.Message + "');</script>");
            //    ShowDialog.Value = mes.Message;
            //    // showWarning(mes.Message);
            //}
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

            SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if(SelectedRegion == null) return Models.CriteriaSpecification.GetFalse();

            spec = spec.And(new CriteriaSpecification("a.RegionGuid", CriteriaOperator.Equal, SelectedRegion.RegionGuid));

            return spec;
        }

        void GoPage(int CurrentPage)
        {
            if (SelectedRegion == null)
                SelectedRegion = CurrentUserRegion;

            var spec = GetQuerySpec();

            if (SelectedRegion.Level == 1 && SelectedRegion.DirectCity == 1)
            {
                CLGridClueSummary.Columns[2].HeaderText = "市州区";
            }
            else if (SelectedRegion.Level == 2)
                CLGridClueSummary.Columns[2].HeaderText = "县市区";


            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridClueSummary.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

           
            Services.WFMClueReportService crs = new Services.WFMClueReportService();

            ps.TotalCount = crs.GetPagedDataCountByRegion(SelectedRegion.Level + SelectedRegion.DirectCity, spec);
            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);
             
            var summaryList = crs.GetPageDataByRegion(SelectedRegion.Level + SelectedRegion.DirectCity, spec, ps.CurrentPageNo, ps.EachPageTotal);

            if (summaryList == null) CLGridClueSummary.DataSource = null;
            else 
            CLGridClueSummary.DataSource = from l in summaryList
                                           join r in regionList on l.RegionGuid equals r.RegionGuid
                                           select new
                                           {
                                               RegionGuid = r.RegionGuid,
                                               RegionName = r.RegionName,
                                               townName = l.ContryName,
                                               TotalClues = l.TotalClues,
                                               InputErrors = l.InputErrors,
                                               Problems = l.Problems,
                                               Confirmed = l.Confirmed,
                                               Order = r.Seq
                                           };

            CLGridClueSummary.DataBind();
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
                    if (index >= 0 && index < CLGridClueSummary.DataKeys.Count)
                    {// 检测gridview范围内
                        //if (CLGridViewUserMgrInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            //int RowID = CLGridViewUserMgrInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.RegionUser();
                            mes.Value.RegionUserID = CLGridClueSummary.DataKeys[index].Value.ToString();

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
        protected void CLGridClueSummary_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
               // LinkButtonModify_Click(null, null);
              //  LinkButtonDetails_Click(null, null);

            }
        }

        ////分页查询
        //void GoPage(int CurrentPage)
        //{
        //    var ps = this.Pages1.GetPageSupport();
        //    if (ps == null)
        //    {
        //        ps = new PageSupport(CLGridClueSummary.PageSize);
        //        this.Pages1.SetPageSupport(ps);

        //        ps.SetStartIndex(0);
        //    }

        //    var spec = GetQuerySpec();

        //    Services.WFMClueSummaryServices css = new Services.WFMClueSummaryServices();

        //    var clueSum = css.GetAllData(new CriteriaSpecification("RegionGuid", CriteriaOperator.Equal, SelectedRegion.RegionGuid));
        //    ps.TotalCount = clueSum.ToList().Count;
        //    ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

        //    var showlist = clueSum.Skip(ps.CurrentPageNo * ps.EachPageTotal).Take(ps.EachPageTotal);

        //    var newlist = from l in showlist
        //                  join r in regionList on l.RegionGuid equals r.RegionGuid
        //                  join b in base.baseTypeList on l.ItemType equals b.ItemValue
        //                  join c in base.baseTypeList on l.ClueType equals c.ItemValue
        //                  where b.ItemParent.BaseTypeID == 15 && 
        //                        c.ItemParent.BaseTypeID == 16 &&
        //                        c.ItemValueType == b.ItemValue
        //                  select new
        //                  {
        //                      ID = l.ID,
        //                      RegionName = r.RegionName,
        //                      ContryName = l.ContryName,
        //                      ItemType = b.ItemName,
        //                      ClueType = c.ItemName,//l.ClueType,
        //                      TotalClues = l.TotalClues,
        //                      InputErrors = l.InputErrors,
        //                      Problems = l.Problems,
        //                      Confirmed = l.Confirmed,
        //                      Order = b.ItemOrder
        //                  };


        //    CLGridClueSummary.DataSource = newlist.OrderBy(x => x.ContryName).ThenBy(x => x.Order).ThenBy(x => x.ItemType); // showlist.ToList().OrderBy(x => x.Level).OrderBy(x => x.seq);
        //    CLGridClueSummary.DataBind();

        //    Pages1.PageReload();
        //}

    }
}