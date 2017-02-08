using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Web.WFMClues
{
    public partial class ClueReportItem : CLBasePage      
    {
        private int Status = 0;
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
                GoPage(1);
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
        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.WFMClueSummary> GenereatePageParameter(int type)
        {

            Models.Messaging<Models.WFMClueSummary> mes = new Models.Messaging<Models.WFMClueSummary>();
            mes.retType = 1;
            mes.Message = System.Guid.NewGuid().ToString();

            if (CurrentUserRegion == null) return mes;
            SelectedRegion = CurrentUserRegion;
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

            SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if (SelectedRegion == null || !base.IsRegionAuthentication(Guid)) //权限控制
                return mes;

            WFMClueSummary cs = new WFMClueSummary();
            cs.RegionGuid = SelectedRegion.RegionGuid;

            if (selectedIndex.Value.CanConvertTo<int>())
            {//界面点击行号从1开始
                int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0                                                                   
                if (index >= 0 && index < CLGridClueReport.DataKeys.Count) // 检测gridview范围内
                    if (CLGridClueReport.DataKeys[index] != null)
                        if (CLGridClueReport.DataKeys[index].Value.CanConvertTo<int>())
                            cs.ItemType = CLGridClueReport.DataKeys[index].Value.ToString();
            }

            mes.Value = cs;           

            Page.Session[mes.Message] = mes;            
            return mes;
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

            if (!base.IsRegionAuthentication(Guid)) //权限控制
                return Models.CriteriaSpecification.GetFalse();

            SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if (SelectedRegion == null) return Models.CriteriaSpecification.GetFalse();
             
            return spec;
        }
        private Models.IParametersSpecification GetClickSpec()
        {
            IParametersSpecification spec = null;
            SelectedRegion = CurrentUserRegion;
            if (selectedIndex.Value.CanConvertTo<int>())
            {//界面点击行号从1开始
                int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0                                                                   
                if (index >= 0 && index < CLGridClueReport.DataKeys.Count) // 检测gridview范围内
                    if (CLGridClueReport.DataKeys[index] != null)
                    {
                        if (CLGridClueReport.DataKeys[index].Value.ToString().CanConvertTo<int>())
                        { 
                            var DataItem = CLGridClueReport.DataKeys[index].Value.ConvertTo<int>();
                            spec = new CriteriaSpecification("DataItem", CriteriaOperator.Equal, DataItem);
                        }
                        //SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == CLGridClueReport.DataKeys[index].Value.ToString());
                    }
            }

            if(!base.IsRegionAuthentication(SelectedRegion.RegionGuid) || spec == null)
                return Models.CriteriaSpecification.GetFalse();


            return spec;
        }

        void GoPage(int CurrentPage)
        {
            var spec = CriteriaSpecification.GetTrue();
            if (Status == 0)
                spec = GetQuerySpec();
            else spec = GetClickSpec();

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridClueReport.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            CLGridClueReport.Columns[3].HeaderText = "省";
            if (SelectedRegion.Level == 1)
            {
                CLGridClueReport.Columns[3].HeaderText = "市州";
                spec = spec.And(new CriteriaSpecification("RegionGuid", CriteriaOperator.Equal, SelectedRegion.RegionGuid));
            }

            if (SelectedRegion.Level == 2)
            {
                CLGridClueReport.Columns[3].HeaderText = "区县";
                spec = spec.And(new CriteriaSpecification("vw_wfm_ReportByCountyItem.RegionGuid", CriteriaOperator.Equal, SelectedRegion.RegionGuid));
            }
              
            Services.WFMClueReportService crs = new Services.WFMClueReportService();
            ps.TotalCount = crs.GetPagedDataCountByItem(SelectedRegion.Level, spec);
            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);
            var reportlist = crs.GetPageDataByItem(SelectedRegion.Level, spec, ps.CurrentPageNo, ps.EachPageTotal);

            if (reportlist == null) CLGridClueReport.DataSource = null;
            else CLGridClueReport.DataSource = (from l in reportlist
                                           join b in baseTypeList on l.ItemType equals b.ItemValue
                                           where b.ItemParent.BaseTypeID == 15
                                           select new
                                           {                     
                                               RegionName = SelectedRegion.RegionName,                         
                                               DataItem = l.ItemType,
                                               DataName = b.ItemName,
                                               TotalClues = l.TotalClues,
                                               InputErrors = l.InputErrors,
                                               Problems = l.Problems,
                                               Confirmed = l.Confirmed,
                                               Order =b.ItemOrder
                                           
                                           }).OrderBy(x => x.Order);


            CLGridClueReport.DataBind();
            Pages1.PageReload();
        }
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            Status = 0;
            GoPage(1);
        }        
        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            //var mes = GenereatePageParameter(0);

            //Response.Redirect("~/WFMClues/ClueMgr.aspx?id=" + mes.Message);

        }        //详细
        protected void LinkButtonDetail_Click(object sender, EventArgs e)
        {
           
        }
        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                
            }
            else
            {//警告修改错误,请选择要修改的供方
                ShowDialog.Value = "请选择要修改的工程项目";
                //showWarning("请选择要修改的工程项目");
            }
        }
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            if (mes.retType != 100)
            { 
                ShowDialog.Value = mes.Message;
            }
        }
        protected void CLGridClueReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                //Status = 1;
                //GoPage(1);

                //var mes = GenereatePageParameter(1);
                //Response.Redirect("~/WFMClues/ClueSummaryItem.aspx?id="+mes.Message);               
            }
        }
        protected void LinkButtonDetails_Click(object sender, EventArgs e)
        {
           
        }
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AppendHeader("content-disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("合同清单" + System.DateTime.Now.ToShortDateString(), System.Text.Encoding.UTF8) + ".xls\"");
            Response.ContentType = "Application/ms-excel";

            Services.ContractServices service = new Services.ContractServices();
            var spec = GetQuerySpec();
            var list = service.GetAllData(spec);


            CMSReport.ContractReport report = new CMSReport.ContractReport();
            report.GenerateReport("合同清单", list);


            report.SAReport.Save(Response.OutputStream);
            Response.End();
        }
    }
}