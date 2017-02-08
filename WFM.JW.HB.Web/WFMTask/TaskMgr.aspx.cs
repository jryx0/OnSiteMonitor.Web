using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMTask
{
    public partial class TaskMgr : CLBasePage
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
                var mes = GetUrlPara<int>("new", false);
                if (mes.retType == 100)
                { 
                   // InitControls(mes.Value);
                   // InitGridView(new Models.CriteriaSpecification("A.ProjectID", Models.CriteriaOperator.Equal, mes.Value));
                   
                }
                else
                { //初始化绑定        
                    var cmes = GetUrlPara<Models.Messaging<Models.ContractInfo>>("id", true);

                    InitDropList( );
                    //InitGridView(null);
                }
                GoPage(1);

                
            }
        }

        #region 下拉列表
        //初始化控件
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
        #endregion

        
        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.WFMUploadTask> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.WFMUploadTask> mes = new Models.Messaging<Models.WFMUploadTask>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewTaskInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        //if (CLGridViewUserMgrInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            //int RowID = CLGridViewUserMgrInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.WFMUploadTask();
                            mes.Value.TaskGuid = CLGridViewTaskInfo.DataKeys[index].Value.ToString();
                            
                            //var r = userList.ToList().Find(x => x.UserName == mes.Value.RegionUserID);
                            if (IsDataAuthentication(mes.Value.TaskGuid))
                            {
                                mes.retType = 200; 
                            }
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
                //if (CurrentUserRegion.Level == 0) return spec;
                Guid = CurrentUserRegion.RegionGuid;
            }

            if (!base.IsRegionAuthentication(Guid)) //权限控制
                return Models.CriteriaSpecification.GetFalse();

            Models.Region SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if (SelectedRegion == null) return Models.CriteriaSpecification.GetFalse();

            if (SelectedRegion.Level != 0)
            {
                Models.IParametersSpecification newspec = new Models.CriteriaSpecification("d.RegionGUID", Models.CriteriaOperator.Equal, SelectedRegion.RegionGuid);
                newspec = newspec.Or(new Models.CriteriaSpecification("c.RegionGUID", Models.CriteriaOperator.Equal, SelectedRegion.RegionGuid));
                spec = spec.And(newspec);
            }

            return spec; 
        }

        //分页查询
        //void GoPage(int CurrentPage)
        //{

        //    var ps = this.Pages1.GetPageSupport();
        //    if (ps == null)
        //    {
        //        ps = new PageSupport(CLGridViewTaskInfo.PageSize);
        //        this.Pages1.SetPageSupport(ps);

        //        ps.SetStartIndex(0);
        //    }

        //    var spec = GetQuerySpec();

        //    Services.ContractServices service = new Services.ContractServices();
        //    //ps.TotalCount = service.GetPagedDataCount(spec);
        //    ///---------------
        //    var ds = service.GetGroupData(spec, "count(*), sum(contractValue), sum(ContractSettle)");
        //    try
        //    {
        //        ps.TotalCount = (int)ds.Tables[0].Rows[0][0]; //TotalNumber 
        //        ContractTotalNumber.Text = ps.TotalCount.ToString();
        //        ContractTotalValue.Text = ds.Tables[0].Rows[0][1].ToString(); 
        //        if(ContractTotalValue.Text.CanConvertTo<double>())
        //            ContractTotalValue.Text = ContractTotalValue.Text.ConvertTo<double>().ToString("C");
        //        else ContractTotalValue.Text = "￥0.00";

        //        ContractTotalSettmentValue.Text = ds.Tables[0].Rows[0][2].ToString();
        //        if (ContractTotalSettmentValue.Text.CanConvertTo<double>())
        //            ContractTotalSettmentValue.Text = ContractTotalSettmentValue.Text.ConvertTo<double>().ToString("C");
        //        else ContractTotalSettmentValue.Text = "￥0.00";
        //    }
        //    catch
        //    {
        //    }
        //    ///-----------------------------------       

        //    ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

        //    var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);

        //    //-------------------------------
        //    ContractNumber.Text = list.Count().ToString();
        //    ContractValue.Text = list.Sum(c => c.ContractValue).ToString("C");
        //    ContractSettmentValue.Text = list.Sum(cs => cs.ContractSettle).ToString("C");

        //    //------------------------------


        //    CLGridViewTaskInfo.DataSource = list;
        //    CLGridViewTaskInfo.DataBind();

        //    Pages1.PageReload();
        //}

        //查询

        void GoPage(int CurrentPage)
        {

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewTaskInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.WFMUploadTaskServices service = new Services.WFMUploadTaskServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);

            //Services.WFMReportService wrs = new Services.WFMReportService();
            //var rlist = wrs.GetAllData();

            //list.ToList().ForEach(x => x.TotalClues = rlist.ToList().Find(y => y.RegionName == x.RegionName).TotalClues);
              


            CLGridViewTaskInfo.DataSource = list;
            CLGridViewTaskInfo.DataBind();

            Pages1.PageReload();
        }

        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);            
        }
        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/WFMTask/TaskEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的供方
                ShowDialog.Value = "请选择要修改的工程项目";
                //showWarning("请选择要修改的工程项目");
            }
        }

        protected bool IsDataAuthentication(string TaskGuid)
        {

            // taskguid -> task -> username -> userregion -> currentuser is authenticated?
            //
            //this task t owned by t.UserName
            Services.WFMUploadTaskServices service = new Services.WFMUploadTaskServices();            
            var t = service.GetDatabyGuid(TaskGuid);
            if (t == null) return false;
            
            var u  = userList.ToList().Find(x => x.UserName == t.UserName.Trim());
            if (u == null) return false;

            return base.IsRegionAuthentication(u.RegionGuid);
        }

        protected void CLGridViewTaskInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(sender, e);
            }
        }

        public  string  SetStatus(string Status)
        {
            string 
                strRet = "上传错误";

            switch(Status)
            {
                case "0":
                    strRet = "正在上传";
                    break;
                case "5":
                    strRet = "上传失败";
                    break;
                case "10":
                    strRet = "上传成功";
                    break;
                case "15":
                    strRet = "数据分析错误";
                    break;
                case "20":
                    strRet = "导入成功";
                    break; 
            }
            return strRet;
        }
    }
}