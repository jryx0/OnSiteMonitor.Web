using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSBaseType
{
    public partial class BaseTypeItemEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.TypeItem>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                    var id = GetUrlPara<int>("new", false);
                    if (mes.retType == 100)
                    {
                        Models.TypeItem item = new Models.TypeItem();
                        item.ItemParent = new Models.BaseType();
                        item.ItemParent.BaseTypeID = id.Value;

                        InitControls(true, item);
                    }
                }
                else
                {//modify
                    InitControls(false, mes.Value.Value);
                }
            }
        }

        //初始化控件
        private void InitControls(bool edittype, Models.TypeItem item)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";

            //new 类型绑定
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, true);
            Services.BaseTypeServices service = new Services.BaseTypeServices();
                      
            var basetypelist = service.GetAllData(spec);
            DropDownListBaseType.DataSource = basetypelist;
            DropDownListBaseType.DataValueField = "BaseTypeID";
            DropDownListBaseType.DataTextField = "BaseTypeName";
            DropDownListBaseType.DataBind();

            if (item != null && item.ItemParent != null)
                DropDownListBaseType.SelectedValue = item.ItemParent.BaseTypeID.ToString();
            
            if (edittype)
                DropDownListBaseType.Items.Insert(0, new ListItem("---", "0")); //动态插入指定序号的新项
           

            if (!edittype)
            {//修改

                Services.BaseTypeItemServices itemservice = new Services.BaseTypeItemServices();
                spec = new Models.CriteriaSpecification("BaseTypeItemID", Models.CriteriaOperator.Equal, item.BaseTypeItemID);
                var itemlist = itemservice.GetAllData(spec);

                if (itemlist.Count() != 0)
                {
                    var firstitem = itemlist.First();
                    //id和version
                    BaseTypeItemID.Value = firstitem.BaseTypeItemID.ToString();
                    Version.Value = firstitem.version.ToString();   
                    
                    //显示当前数据类型

                    DropDownListBaseType.DataSource = from b in basetypelist
                                                      where b.BaseTypeID == firstitem.ItemParent.BaseTypeID
                                                      select b;
                    DropDownListBaseType.DataValueField = "BaseTypeID";
                    DropDownListBaseType.DataTextField = "BaseTypeName";
                    DropDownListBaseType.DataBind();


                    ItemName.Text = firstitem.ItemName;
                    ItemValue.Text = firstitem.ItemValue;
                    ItemType.Text = firstitem.ItemValueType;
                    ItemOrder.Text = firstitem.ItemOrder.ToString();

                    LabelOrder.Visible = true;
                    ItemOrder.Visible = true;

                    EnableDropDownList.Visible = true;
                    EnableLabel.Visible = true;

                    EnableDropDownList.SelectedIndex = firstitem.Enable ? 1 : 0;
                    CommentTextBox.Text = firstitem.Comment;
                    
                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";
                }
            }
        }

        //生成ItemType对象
        //type  新增 or 修改
        private Models.Messaging<Models.TypeItem> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.TypeItem>();
            mes.Value = new Models.TypeItem();
            mes.retType = 1;

            //new  needed
            if (DropDownListBaseType.SelectedValue.CanConvertTo<int>())
            {
                //类型ID
                mes.Value.ItemParent = new Models.BaseType();
                mes.Value.ItemParent.BaseTypeID = DropDownListBaseType.SelectedValue.ConvertTo<int>();

                //状态,禁用启用
                mes.Value.Enable = true;

                //项目名称
                mes.Value.ItemName = ItemName.Text;

                //项目名称
                mes.Value.ItemValue = ItemValue.Text;

                //备注
                mes.Value.Comment = CommentTextBox.Text;


                //创建人
                string username = Page.User.Identity.Name;
                if (String.IsNullOrEmpty(username))
                    mes.Value.Creator = "anonymous";
                else mes.Value.Creator = username;
                mes.Value.CreateDate = System.DateTime.Now;
                mes.retType = 100;
                mes.Message = "获取ItemType对象成功!";
            }
            else
            {
                mes.retType = 1;
                mes.Message = "新增参数供方类型或状态错误";
            }


            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (BaseTypeItemID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>() &&
                    EnableDropDownList.SelectedValue.CanConvertTo<bool>()&&
                    ItemOrder.Text.CanConvertTo<int>())
                {
                    //隐藏控件－供方ID不能变－
                    mes.Value.BaseTypeItemID = BaseTypeItemID.Value.ConvertTo<int>();
                    //隐藏控件－版本
                    mes.Value.version = Version.Value.ConvertTo<Int64>();

                    //状态,禁用启用
                    mes.Value.Enable = EnableDropDownList.SelectedValue.ConvertTo<bool>();

                    mes.Value.ItemOrder = ItemOrder.Text.ConvertTo<int>();

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
                Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.BaseTypeItemServices service = new Services.BaseTypeItemServices();
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
            if (ChangeObject(e.CommandArgument.ToString()).retType == 100)
            {   
                LinkButtonBack_Click(sender, e); //保存成功!
            }
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {   
            //TODO:返回根据传入的BasetypeID， 
            var mes = GenerateObject("New");

            if (mes.Value !=null && mes.Value.ItemParent.BaseTypeID != 0)
            {

                Models.Messaging<Models.BaseType> mestype = new Models.Messaging<Models.BaseType>();
                mestype.Value = mes.Value.ItemParent;

                var id = Guid.NewGuid();
                Page.Session[id.ToString()] = mestype;

                Response.Redirect("~/CMSBaseType/BaseTypeItemQuery.aspx?id=" + id.ToString());
            }
            else
                Response.Redirect("~/CMSBaseType/BaseTypeItemQuery.aspx");
        }
    }
}