using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSSupplier
{
    public partial class CMSSupplierView : Web.CLBasePage
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
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "供方类型");
            var list = service.GetAllData(spec);

            DropDownListSupplierType.DataSource = list;
            DropDownListSupplierType.DataValueField = "BaseTypeItemID";
            DropDownListSupplierType.DataTextField = "ItemName";
            DropDownListSupplierType.DataBind();

            DropDownListSupplierType.Items.Insert(0, new ListItem("全部", "all")); //动态插入指定序号的新项
        }


        //初始化GridView
        private void InitGridView(Models.IParametersSpecification spec)
        {
            Services.SupplierService service = new Services.SupplierService();
            CLGridViewSupplier.DataSource = service.GetAllData(spec);
                                            
            CLGridViewSupplier.DataBind();
        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);

            if (this.TextBoxSupplier.Text.Length != 0)
                spec = spec.And(new Models.CriteriaSpecification("SupplierName", Models.CriteriaOperator.Like, this.TextBoxSupplier.Text));


            if (DropDownListSupplierType.SelectedValue.CanConvertTo<int>())
            {
                int DropValule = DropDownListSupplierType.SelectedValue.ConvertTo<int>();
                spec = spec.And(new Models.CriteriaSpecification("SupplierTypeID", Models.CriteriaOperator.Equal, DropValule));
            }

            if (DropDownListEnable.SelectedValue.CanConvertTo<bool>())
            {
                bool DropValule = DropDownListEnable.SelectedValue.ConvertTo<bool>();
                spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, DropValule));
            }

            return spec;
        }
        
        //分页查询
        void GoPage(int CurrentPage)
        {

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewSupplier.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.SupplierService service = new Services.SupplierService();
            ps.TotalCount = service.GetPagedDataCount(spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(spec, ps.CurrentPageNo, ps.EachPageTotal);


            CLGridViewSupplier.DataSource = list;
            CLGridViewSupplier.DataBind();

            Pages1.PageReload();
        }

        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);
            //var spec = GetQuerySpec();

            //InitGridView(spec);
        }

        /**
         * type = 0 新增
         * type = 1 修改  
         * 
         * mes.Value = null 时 
         * */
        private Models.Messaging<Models.SupplierInfo> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.SupplierInfo> mes = new Models.Messaging<Models.SupplierInfo>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewSupplier.DataKeys.Count)
                    {// 检测gridview范围内
                        if (CLGridViewSupplier.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            int supplierid = CLGridViewSupplier.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.SupplierInfo();
                            mes.Value.SupplierID = supplierid;
                            mes.retType = 200;
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }

        //新增
        //绑定类型        
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(0);

            Response.Redirect("~/CMSSupplier/CMSSupplierEdit.aspx?id=" + mes.Message);
        }

        //修改
        //传递 参数,绑定类型
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType == 200)
            {
                Response.Redirect("~/CMSSupplier/CMSSupplierEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的供方
                //showWarning("请选择要修改的供方");
                ShowDialog.Value = "请选择要修改的供方";
            }
        }

        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);
            mes.Message = "请选择要删除的供方";
            if (mes.retType == 200)
            {//OK删除
                Services.SupplierService service = new Services.SupplierService();
                var retMes = service.DeleteData(mes.Value);
                mes.copy(retMes);
                //InitGridView(null);
                GoPage(1);
            }

            if (mes.retType != 100)
            {//警告删除出错
                //Response.Write("<script>alert('" + mes.Message + "');</script>");
                ShowDialog.Value = mes.Message;
               // showWarning(mes.Message);
            }

        }


        //双击
        protected void CLGridViewSupplier_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(null, null);

                selectedIndex.Value = "";
            }
        }
    }
}