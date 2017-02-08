using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.CMSBaseType
{
    public partial class CMSBaseTypeEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.BaseType>>("id");

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
        private void InitControls(Models.BaseType info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";
            

            if (info != null)
            {//modify init
                Services.BaseTypeServices basetypeservice = new Services.BaseTypeServices();
                var spec = new Models.CriteriaSpecification("BaseTypeID", Models.CriteriaOperator.Equal, info.BaseTypeID);
                var list = basetypeservice.GetAllData(spec);

                if (list.Count() != 0)
                {
                    var basetype = list.First();
                    //id和version
                    BaseTypeID.Value = basetype.BaseTypeID.ToString();
                    Version.Value = basetype.version.ToString();

                    BaseTypeNameTextBox.Text = basetype.BaseTypeName;    

                    EnableDropDownList.Visible = true;
                    EnableDropDownList.SelectedIndex = basetype.Enable ? 1 : 0;
                    EnableLabel.Visible = true;

                    CommentTextBox.Text = basetype.Comment;

                    //显示控件
                    OrderLabel.Visible = true;
                    BaseTypeOrderTextBox.Visible = true;
                    BaseTypeOrderTextBox.Text = basetype.TypeOrder.ToString();

                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";
                }
            }
        }


        //生成BaseType对象
        //type  新增 or 修改
        private Models.Messaging<Models.BaseType> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.BaseType>();
            mes.Value = new Models.BaseType();
            mes.retType = 1;

            mes.Value.BaseTypeName = BaseTypeNameTextBox.Text;
            mes.Value.Comment = CommentTextBox.Text;
            
            //状态,禁用启用
            mes.Value.Enable = true;
                
            //创建人
            string username = Page.User.Identity.Name;
            if (String.IsNullOrEmpty(username))
                mes.Value.Creator = "anonymous";
            else mes.Value.Creator = username;

            mes.Value.CreateDate = System.DateTime.Now;
            mes.retType = 100;
            mes.Message = "获取supplier对象成功!";
           

            //modify supplier needed
            if (mes.retType == 100 && type == "Modify")
            {
                if (EnableDropDownList.SelectedValue.CanConvertTo<bool>()&&
                    BaseTypeID.Value.CanConvertTo<int>() &&Version.Value.CanConvertTo<Int64>()&&
                    BaseTypeOrderTextBox.Text.CanConvertTo<int>())
                {
                    //隐藏控件－获取ID和版本
                    mes.Value.BaseTypeID = BaseTypeID.Value.ConvertTo<int>();
                    mes.Value.version = Version.Value.ConvertTo<Int64>();
   
                    //状态,禁用启用
                    mes.Value.Enable = EnableDropDownList.SelectedValue.ConvertTo<bool>();

                    //顺序
                    mes.Value.TypeOrder = BaseTypeOrderTextBox.Text.ConvertTo<int>();

                    //修改人
                    username = Page.User.Identity.Name;
                    if (String.IsNullOrEmpty(username))
                        mes.Value.Modifier = "anonymous";
                    else mes.Value.Modifier = username;
                    mes.Value.ModifyDate = System.DateTime.Now;
                    mes.retType = 200;
                }
                else
                {
                    mes.retType = 2;
                    mes.Message = "修改无法获取状态!";
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
                Services.BaseTypeServices service = new Services.BaseTypeServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.BaseTypeServices service = new Services.BaseTypeServices();
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
                LinkButtonBack_Click1(null, null); //保存成功!
        }

   
        protected void LinkButtonBack_Click1(object sender, EventArgs e)
        {
            Response.Redirect("~/CMSBaseType/BaseTypeQuery.aspx");
        }
    }
}