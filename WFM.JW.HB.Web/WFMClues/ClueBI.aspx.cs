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
    public partial class ClueBI : CLBasePage      
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
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridClueReport.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridClueReport.DataKeys[index] != null )
                        {//获取gridview datakey中的值                            
                            mes.Value = new Models.WFMClueSummary();
                            mes.Value.RegionGuid = CLGridClueReport.DataKeys[index].Value.ToString();
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

            if (SelectedRegion.Level != 0)
            {
                //Models.IParametersSpecification newspec = new Models.CriteriaSpecification("RegionGUID", Models.CriteriaOperator.Equal, Guid);
                // newspec = newspec.Or(new Models.CriteriaSpecification("ParentGUID", Models.CriteriaOperator.Equal, Guid));
                //  spec = spec.And(newspec);
            }
            
            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridClueReport.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.WFMClueSummaryServices css = new Services.WFMClueSummaryServices();

            var Spec = new CriteriaSpecification("RegionGuid", CriteriaOperator.Equal, SelectedRegion.RegionGuid);

            //直管市 ？
            if (SelectedRegion.Level != 2)
                if(SelectedRegion.DirectCity == 0)
                Spec = null;


            //var clueSum = SelectedRegion.Level == 0 ? css.GetGroupData1(Spec) : css.GetGroupData2(Spec);
            var clueSum = css.GetGroupData2(Spec);

            List<Models.WFMClueSummary> List = new List<WFMClueSummary>();
            if (SelectedRegion.Level == 1 && SelectedRegion.DirectCity == 0)
            {
                //foreach (var c in clueSum)
                //    foreach (var r in regionList)
                //        if (r.RegionGuid == c.RegionGuid)
                //            if (r.ParentGuid == SelectedRegion.RegionGuid)
                //                List.Add(c);
                clueSum = from c in clueSum
                        join r in regionList on c.RegionGuid equals r.RegionGuid 
                        where r.ParentGuid == SelectedRegion.RegionGuid
                        select c;
            }
            
            ps.TotalCount = clueSum.ToList().Count;
            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);
            var showlist = clueSum.Skip((CurrentPage - 1) * ps.EachPageTotal).Take(ps.EachPageTotal);

            var newlist = from l in showlist
                          join r in regionList on l.RegionGuid equals r.RegionGuid
                          select new
                          {
                              ReginoGuid = r.RegionGuid,
                              RegionName = r.RegionName,
                              ParentName = r.ParentName,
                              TotalClues = l.TotalClues,
                              InputErrors = l.InputErrors,
                              Problems = l.Problems,
                              Confirmed = l.Confirmed,
                              Order = r.Seq
                          };


            CLGridClueReport.DataSource = newlist.OrderBy(x => x.Order); // showlist.ToList().OrderBy(x => x.Level).OrderBy(x => x.seq);
            CLGridClueReport.DataBind();

            Pages1.PageReload();
        }


        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (this.TextBoxContractName.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("ContractName", Models.CriteriaOperator.Like, this.TextBoxContractName.Text));

            //if (this.TextBoxContractCode.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("ContractCode", Models.CriteriaOperator.Like, this.TextBoxContractCode.Text));


            //if (DropDownProject.SelectedValue.CanConvertTo<int>())
            //    spec = spec.And(new Models.CriteriaSpecification("A.ProjectID", Models.CriteriaOperator.Equal, DropDownProject.SelectedValue.ConvertTo<int>()));

            //if (DropDownSupplier.SelectedValue.CanConvertTo<int>())
            //    spec = spec.And(new Models.CriteriaSpecification("A.SupplierID", Models.CriteriaOperator.Equal, DropDownSupplier.SelectedValue.ConvertTo<int>()));

            //if(DropDownListDepartment.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.DepartmentID", Models.CriteriaOperator.Equal, DropDownListDepartment.SelectedValue.ConvertTo<int>()));
            
            //if(DropDownListContractType.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.ContractTypeID", Models.CriteriaOperator.Equal, DropDownListContractType.SelectedValue.ConvertTo<int>()));


            //if(DropDownListContractStatus.SelectedValue.CanConvertTo<int>())
            //    spec =spec.And(new Models.CriteriaSpecification("A.ContractStatusID", Models.CriteriaOperator.Equal, DropDownListContractStatus.SelectedValue.ConvertTo<int>()));

            
            //InitGridView(spec);
        }

        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            //var mes = GenereatePageParameter(0);

            //Response.Redirect("~/WFMClues/ClueMgr.aspx?id=" + mes.Message);

        }

        //详细
        protected void LinkButtonDetail_Click(object sender, EventArgs e)
        {
            var mes = new Models.Messaging<Models.RegionUser>();
            mes.retType = 100;
            mes.Value = new Models.RegionUser();
            mes.Value.RegionGuid = "aed157d7-9cb9-4121-8791-4bb310ed48c9";

            mes.Message = System.Guid.NewGuid().ToString();
            Page.Session[mes.Message] = mes;
            Response.Redirect("~/WFMClues/ClueMgr.aspx?id=" + mes.Message);
        }


        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSContract/ContractEdit.aspx?id=" + mes.Message);
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
            //mes.Message = "请选择要删除的项目";
            //if (mes.retType == 200)
            //{//OK删除
            //    Services.ContractServices service = new Services.ContractServices();
            //    var retMes = service.DeleteData(mes.Value);

            //    if (retMes.retType == 100)
            //        // InitGridView(null);
            //        GoPage(1);
                
            //    mes.copy(retMes);
            //}
            
            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                //showWarning(mes.Message);
                ShowDialog.Value = mes.Message;
            }
        }

        protected void CLGridClueReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonDetails_Click(null, null);
            }
        }

        protected void LinkButtonDetails_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            if (mes.retType == 200)
            {
                Response.Redirect("~/WFMClues/ClueSummary.aspx?id=" + mes.Message);
            }
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