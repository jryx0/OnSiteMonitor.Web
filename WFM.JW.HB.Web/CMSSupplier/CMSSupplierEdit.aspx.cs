using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSSupplier
{
    public partial class CMSSupplierEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.SupplierInfo>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                    InitControls(null);
                }
                else
                {//modify
                    InitControls(mes.Value.Value);
                }               
            }
        }

        
        //初始化控件
        private void InitControls(Models.SupplierInfo info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //new 类型绑定
            Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();

            Models.IParametersSpecification spec = new Models.CriteriaSpecification("CMS_CSCL_BaseType.[BaseTypeName]", Models.CriteriaOperator.Equal, "供方类型");
            spec = spec.And(new Models.CriteriaSpecification("A.Enable", Models.CriteriaOperator.Equal, true));
            var list = service.GetAllData(spec);

            SupplierTypeDropDownList.DataSource = list;
            SupplierTypeDropDownList.DataValueField = "BaseTypeItemID";
            SupplierTypeDropDownList.DataTextField = "ItemName";
            SupplierTypeDropDownList.DataBind();

            //修改导航
            SiteMapNode smn = SiteMap.Provider.FindSiteMapNode("~/CMSSupplier/CMSSupplierEdit.aspx");//获得当前节点
            smn.ReadOnly = false;//将当前节点设置为只读
            smn.Title = "新增供方";//为当前节点title赋值 
            
            if (info != null)
            {//modify init
                Services.SupplierService supplierserivce = new Services.SupplierService();
                spec = new Models.CriteriaSpecification("SupplierID", Models.CriteriaOperator.Equal, info.SupplierID);
                var supplierlist = supplierserivce.GetAllData(spec);

                if (supplierlist.Count() != 0)
                {
                    var supplier = supplierlist.First();
                    //id和version                    
                    SupplierID.Value = supplier.SupplierID.ToString();
                    Version.Value = supplier.version.ToString();

                    SupplierNameTextBox.Text = supplier.SupplierName;
                    SupplierTypeDropDownList.SelectedValue = supplier.SupplierType.BaseTypeItemID.ToString();

                    EnableDropDownList.Visible = true;
                    EnableDropDownList.SelectedIndex = supplier.Enable ? 1 : 0;
                    EnableLabel.Visible = true;

                    SupplierCommentTextBox.Text = supplier.Comment;


                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";

                    //修改导航
                    smn.Title = "修改供方";//为当前节点title赋值 
                }
            }
        }


        //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.SupplierInfo> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.SupplierInfo>();
            mes.Value = new Models.SupplierInfo();
            mes.retType = 1;
            
            //new supplier needed
            if (SupplierTypeDropDownList.SelectedValue.CanConvertTo<int>())
            {
                //供方类型ID
                mes.Value.SupplierType = new Models.TypeItem();
                mes.Value.SupplierType.BaseTypeItemID = SupplierTypeDropDownList.SelectedValue.ConvertTo<int>();

                //状态,禁用启用
                mes.Value.Enable = true;

                //供方名称
                mes.Value.SupplierName = SupplierNameTextBox.Text;
                //备注
                mes.Value.Comment = SupplierCommentTextBox.Text;

               
                //创建人
                string username = Page.User.Identity.Name;
                if (String.IsNullOrEmpty(username))
                    mes.Value.Creator = "anonymous";
                else mes.Value.Creator = username;
                mes.Value.CreateDate = System.DateTime.Now;
                mes.retType = 100;
                mes.Message = "获取supplier对象成功!";
            }
            else
            {
                mes.retType = 1;
                mes.Message = "新增参数供方类型或状态错误";
            }


            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (SupplierID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>() &&
                    EnableDropDownList.SelectedValue.CanConvertTo<bool>())
                {
                    //隐藏控件－供方ID不能变－
                    mes.Value.SupplierID = SupplierID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();

                    //状态,禁用启用
                    mes.Value.Enable = EnableDropDownList.SelectedValue.ConvertTo<bool>();

                    //修改人
                    string username = Page.User.Identity.Name;
                    if (String.IsNullOrEmpty(username))
                        mes.Value.Modifier = "anonymous";
                    else mes.Value.Modifier = username;
                    mes.Value.ModifyDate = System.DateTime.Now;

                    mes.retType = 200;
                }
                else
                {
                    mes.retType = 2;
                    mes.Message = "修改无法获取供方ID和版本!";
                }
            }

            return mes;            
        }
    
        private Models.Messaging<int> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;           
            var mes = GenerateObject(type);
           
            if (mes.retType == 100)
            {//新增对象获取成功
                Services.SupplierService service = new Services.SupplierService();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.SupplierService service = new Services.SupplierService();
                retMes = service.UpdateData(mes.Value);
            }
            else
            {//错误
                retMes = new Models.Messaging<int>();
                retMes.copy(mes);
            }
            LabelInfo.Text = retMes.Message;

            return retMes;
        }

        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            ChangeObject(e.CommandArgument.ToString());            
        }

        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
            if(ChangeObject(e.CommandArgument.ToString()).retType == 100)
                LinkButtonBack_Click(sender, e); //保存成功!
        }
        
        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/CMSSupplier/CMSSupplierView.aspx");
        }

   }
}