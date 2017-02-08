using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TB.ComponentModel;

namespace WFM.JW.HB.Web.WFMRegion
{
    public partial class RegionEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.Region>>("id");

                if (mes.Value == null || mes.Value.Value == null)
                {//new
                   
                }
                else
                {//modify
                    InitControls(mes.Value.Value);
                }
            }
        }


         //初始化控件
        private void InitControls(Models.Region info)
        {
            //new 设置按钮事件
            LinkButtonApply.CommandArgument = "New";
            LinkButtonSave.CommandArgument = "New";


            ////修改导航
            //SiteMapNode smn = SiteMap.Provider.FindSiteMapNode("~/CMSProject/ProjectInfoEdit.aspx");//获得当前节点
            //smn.ReadOnly = false;//将当前节点设置为只读
            //smn.Title = "新增项目";//为当前节点title赋值 

            if (info != null)
            {//modify init
                Services.WFMRegionServices rs = new Services.WFMRegionServices();
                Models.IParametersSpecification spec = new Models.CriteriaSpecification("a.RowID", Models.CriteriaOperator.Equal, info.RowID);
                var rlist = rs.GetAllData(spec);
            
                if (rlist.Count() != 0)
                { 
                    var r = rlist.First();
                   // ParentNameTextBox.Text = r.ParentName;
                    RegionTextBox.Text = r.RegionName;

                    DCityTextBox.Text = r.DirectCity.ToString();


                    RegionID.Value = r.RowID.ToString();
                    Version.Value = r.version.ToString();



                    //modify 设置按钮事件
                    LinkButtonApply.CommandArgument = "Modify";
                    LinkButtonSave.CommandArgument = "Modify";
                }
            }
        }


        private Models.Messaging<int> ChangeObject(string type)
        {
            Models.Messaging<int> retMes;
            var mes = GenerateObject(type);

            if (mes.retType == 100)
            {//新增对象获取成功
                Services.WFMRegionServices service = new Services.WFMRegionServices();
                retMes = service.InsertData(mes.Value);
            }
            else if (mes.retType == 200)
            {//修改对象获取成功
                Services.WFMRegionServices service = new Services.WFMRegionServices();
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

          //生成supplierinfo对象
        //type  新增 or 修改
        private Models.Messaging<Models.Region> GenerateObject(string type)
        {
            var mes = new Models.Messaging<Models.Region>();
            mes.Value = new Models.Region();
            mes.retType = 100;

            //modify needed
            if (mes.retType == 100 && type == "Modify")
            {
                mes.Value.RowID = RegionID.Value.ConvertTo<int>();
                mes.Value.version = Version.Value.ConvertTo<Int64>();

                mes.Value.DirectCity = int.Parse(DCityTextBox.Text);


                mes.retType = 200;
            }
            else
            {
                mes.retType = 2;
                mes.Message = "修改无法获取项目ID和版本!";
            }
            
            return mes;
        }


        protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        {
            ChangeObject(e.CommandArgument.ToString());
        }
        
        protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        {
            if (ChangeObject(e.CommandArgument.ToString()).retType == 100)
                LinkButtonBack_Click(sender, e); //保存成功!
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WFMRegion/RegionView.aspx");
        }
    }
}