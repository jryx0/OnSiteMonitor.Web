using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMRegion
{
    public partial class RegionView : System.Web.UI.Page
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
                InitControls(0);
                 
                GoPage(1);
            }
            else InitContry();
        }

        private void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(CLGridViewRegionInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.WFMRegionServices service = new Services.WFMRegionServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            CLGridViewRegionInfo.DataSource = list;
            CLGridViewRegionInfo.DataBind();

            Pages1.PageReload();
        }



        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            spec = spec.And(new Models.CriteriaSpecification("A.Status", Models.CriteriaOperator.Equal, 1));

            if (DropDownListCity.SelectedIndex != 0)
                if (DropDownListContry.SelectedIndex == 0)
                    spec = spec.And(new Models.CriteriaSpecification("b.RowiD", Models.CriteriaOperator.Equal, DropDownListCity.SelectedValue));
                else
                    spec = spec.And(new Models.CriteriaSpecification("a.RowID", Models.CriteriaOperator.Equal, DropDownListCity.SelectedValue));

            
            return spec;
        }

        private void InitControls(int RowID)
        {
            InitCity();
            
        }


        protected void InitCity()
        {
            Services.WFMRegionServices service = new Services.WFMRegionServices();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            var rlist = service.GetAllData(spec);

            DropDownListCity.DataSource = from r in rlist
                                          where r.Level == 1
                                          select r;


            //DropDownListCity.DataSource = rlist;
            DropDownListCity.DataValueField = "RowID";
            DropDownListCity.DataTextField = "RegionName";
            DropDownListCity.DataBind();

            DropDownListCity.Items.Insert(0, new ListItem("请选择市州", "all")); //动态插入指定序号的新项
        }


        protected void InitContry( )
        {
            Services.WFMRegionServices service = new Services.WFMRegionServices();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            var rlist = service.GetAllData(spec);
                        

            //DropDownListContry.DataSource = from r in rlist
            //                                where r.ParentName == DropDownListCity.SelectedItem.Text                                             
            //                                select r;
             
            DropDownListContry.DataValueField = "RowID";
            DropDownListContry.DataTextField = "RegionName";
            DropDownListContry.DataBind();

            DropDownListContry.Items.Insert(0, new ListItem("请选择区县", "all")); //动态插入指定序号的新项 
        }

        public void InitGridView(Models.IParametersSpecification spec)
        {
            Services.WFMRegionServices service = new Services.WFMRegionServices();
            this.CLGridViewRegionInfo.DataSource = service.GetAllData(spec);

            this.CLGridViewRegionInfo.DataBind();
        }

        protected void CityList_IndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
        }

        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/WFMRegion/RegionEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的项目
                ShowDialog.Value = "请选择要修改的工程项目";
            }
        }

        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.Region> GenereatePageParameter(int type)
        {

            Models.Messaging<Models.Region> mes = new Models.Messaging<Models.Region>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewRegionInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewRegionInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int rowid = CLGridViewRegionInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.Region();
                            mes.Value.RowID = rowid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;

        }

        protected void CLGridViewRegionInfo_RowCommand(object sender, GridViewCommandEventArgs e)
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