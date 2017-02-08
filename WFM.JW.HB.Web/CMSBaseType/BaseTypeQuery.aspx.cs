using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

using WFM.JW.HB.Models;
using WFM.JW.HB.Services;
using TB.ComponentModel;

namespace WFM.JW.HB.Web
{
    public partial class BaseTypeQuery : CLBasePage
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

        }


        //private void InitGridView(Models.IParametersSpecification spec)
        //{
        //    Services.BaseTypeServices basetypeService = new Services.BaseTypeServices();
        //    var basetypelist = basetypeService.GetAllData(spec);

        //    BaseTypeGridView.DataSource = from b in basetypelist
        //                                  orderby b.TypeOrder
        //                                  select b;
        //    BaseTypeGridView.DataBind();
        //}

        //查询参数获取
        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (TextBoxBaseTypeName.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("BaseTypeName", Models.CriteriaOperator.Like, TextBoxBaseTypeName.Text));


            if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, DropValule));
            }

            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {

                ps = new PageSupport(BaseTypeGridView.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.BaseTypeServices service = new Services.BaseTypeServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);
            BaseTypeGridView.DataSource = from b in list
                                          orderby b.TypeOrder
                                          select b;
            BaseTypeGridView.DataBind();

            Pages1.PageReload();
        }

        #region 查询
        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (TextBoxBaseTypeName.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("BaseTypeName", Models.CriteriaOperator.Like, TextBoxBaseTypeName.Text));


            //if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            //{
            //    bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
            //    spec = spec.And(new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, DropValule));
            //}

            //InitGridView(spec);
            GoPage(1);
        }
        # endregion

        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.BaseType> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.BaseType> mes = new Models.Messaging<Models.BaseType>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();


            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < BaseTypeGridView.DataKeys.Count)
                    {// 检测gridview范围内
                        if (BaseTypeGridView.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int basetypeid = BaseTypeGridView.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.BaseType();
                            mes.Value.BaseTypeID = basetypeid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }

        //新建
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;

            var mes = GenereatePageParameter(0);
            Response.Redirect("~/CMSBaseType/BaseTypeEdit.aspx?id=" + mes.Message);
        }

        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSBaseType/BaseTypeEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的数据类型
                ShowDialog.Value = "请选择要修改的数据类型";
            }
        }

        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要禁用的数据类型";
            if (mes.retType == 200)
            {//OK删除
                Services.BaseTypeServices service = new Services.BaseTypeServices();
                var retMes = service.DeleteData(mes.Value);

                if (retMes.retType == 2)
                    mes.copy(retMes);

                //InitGridView(null);
                GoPage(1);
            }

            if (mes.retType == 2)//警告删除出错
                ShowDialog.Value = mes.Message;//"有其它数据引用此数据，无法删除数据！";
            //else if (mes.retType != 100)
            //{
            //    ShowDialog.Value = mes.Message;
            //}
        }

        //数据项
        #region 明细/双击
        //双击
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonDetails_Click(null, null);

                selectedIndex.Value = "";
            }
        }   
        
        //明细
        protected void LinkButtonDetails_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSBaseType/BaseTypeItemQuery.aspx?id=" + mes.Message);
            }           
        }
        #endregion
    }
}