using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.BaseType
{
    public partial class BaseTypeItemQuery : CLBasePage
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
                var mes = base.GetUrlPara<Models.Messaging<Models.BaseType>>("id");
                if (mes.Value == null || mes.Value.Value == null)
                {//直接运行本页
                    InitControls(null);
                }
                else
                {//从主表转跳本页
                    InitControls(mes.Value.Value);
                }
            }
        }

        //初始化控件
        private void InitControls(Models.BaseType basetype)
        {
            Services.BaseTypeServices service = new Services.BaseTypeServices();
            
            DropDownListBaseType.DataSource = service.GetAllData(null);
            DropDownListBaseType.DataValueField = "BaseTypeID";
            DropDownListBaseType.DataTextField = "BaseTypeName";
            DropDownListBaseType.DataBind();

            DropDownListBaseType.Items.Insert(0, new ListItem("全部", "0")); //动态插入指定序号的新项
            if (basetype != null)
            {
                DropDownListBaseType.SelectedValue = basetype.BaseTypeID.ToString();
               
            }
            
            //Models.IParametersSpecification spec = null;
            //if (basetype != null)
            //    spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.BaseTypeID", Models.CriteriaOperator.Equal, basetype.BaseTypeID);
            //InitGridView(spec);
            GoPage(1);
        }

        //GridView绑定
        private void InitGridView(Models.IParametersSpecification spec)
        {
            Services.BaseTypeItemServices services = new Services.BaseTypeItemServices();

            var typeitemlist = services.GetAllData(spec);
            GridViewItem.DataSource = typeitemlist;
            //GridViewItem.DataSource = from i in typeitemlist
            //                          orderby i.ItemOrder ascending
            //                          select i;
            GridViewItem.DataBind();
        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (DropDownListBaseType.SelectedValue.CanConvertTo<int>())
            {
                int DropValule = DropDownListBaseType.SelectedValue.ConvertTo<int>();
                if (DropValule != 0)
                    spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeID]", Models.CriteriaOperator.Equal, DropValule));

            }

            if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, DropValule));
            }

            if (ItemNameTextBox.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("ItemName", Models.CriteriaOperator.Like, ItemNameTextBox.Text));
            

            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(GridViewItem.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            GridViewItem.DataSource = list;
            GridViewItem.DataBind();

            Pages1.PageReload();
        }


        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            //if (DropDownListBaseType.SelectedValue.CanConvertTo<int>())
            //{
            //    int DropValule = DropDownListBaseType.SelectedValue.ConvertTo<int>();
            //    if (DropValule != 0)
            //        spec = spec.And(new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeID]", Models.CriteriaOperator.Equal, DropValule));

            //}

            //if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            //{
            //    bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
            //    spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, DropValule));
            //}

            //if (ItemNameTextBox.Text.Length != 0)
            //    spec = spec.And(new Models.CriteriaSpecification("ItemName", Models.CriteriaOperator.Like, ItemNameTextBox.Text));
            

            //InitGridView(spec);
            GoPage(1);
        }

                
        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.TypeItem> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.TypeItem> mes = new Models.Messaging<Models.TypeItem>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();


            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < GridViewItem.DataKeys.Count)
                    {// 检测gridview范围内
                        if (GridViewItem.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int basetypeitemid = GridViewItem.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.TypeItem();
                            mes.Value.BaseTypeItemID = basetypeitemid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
           // Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }
        
        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;

            var mes = GenereatePageParameter(0);
            string para = "?id=" + mes.Message;
            if (DropDownListBaseType.SelectedValue.CanConvertTo<int>())
            {
                
                para += "&new=" + DropDownListBaseType.SelectedValue.ConvertTo<int>();
            }


            //if (selectedIndex.Value.CanConvertTo<int>())
            //{
            //    para += "&new=" + selectedIndex.Value.ConvertTo<int>();
            //}
            //else if (DropDownListBaseType.SelectedValue.CanConvertTo<int>())
            //{
            //    para += "&new=" + DropDownListBaseType.SelectedValue.ConvertTo<int>();
            //}

            
            Response.Redirect("~/CMSBaseType/BaseTypeItemEdit.aspx" + para);
        }

        //修改
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;

            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSBaseType/BaseTypeItemEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的数据类型
                ShowDialog.Value = "请选择要修改的数据类型";
            }
        }

        //双击修改
        protected void GridViewItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(null, null);

                selectedIndex.Value = "";

            }
        }


        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            if (CurrentUserRegion.RegionCode != "HB")
                return;

            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的数据项";
            if (mes.retType == 200)
            {//OK删除
                Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();
                var retMes = service.DeleteData(mes.Value);
                mes.copy(retMes);
                InitGridView(null);
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                ShowDialog.Value = mes.Message;
            }
        }


        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/CMSBaseType/BaseTypeQuery.aspx");
        }

       



       
    }
}