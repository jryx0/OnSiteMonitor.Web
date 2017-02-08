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
    public partial class CluesEdit : CLBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var mes = GetUrlPara<Models.Messaging<Models.Clues>>("id");

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
        private void InitControls(Models.Clues info)
        {
            if (info == null) return;
            Services.WFMClueServices cs = new Services.WFMClueServices();

            var r = regionList.ToList().Find(x => x.RegionGuid == info.RegionGuid);

            if (r == null) return;
            if (!IsRegionAuthentication(r.RegionGuid)) return;

            var list = cs.GetAllData(r.RegionCode, new Models.CriteriaSpecification("DataGuid", Models.CriteriaOperator.Equal, info.ClueGuid));
            if (list == null) return;

            var clue = list.First();
            if (clue == null) return;


            this.tbRegionName.Text = r.RegionName;
            this.tbID.Text = clue.ID;
            this.tbName.Text = clue.Name;
            this.tbTown.Text = clue.Region;
            this.tbAddr.Text = clue.Addr;

            this.tbType.Text = clue.Type;

            if (clue.IsConfirmed == 1)
            {
                this.tbIsClueTrue.Text = clue.IsClueTrue == 1 ? "属实" : "不属实";
                this.tbIsCP.Text = clue.IsCP == 1 ? "党员" : "群众";
                this.tbIllegalMoney.Text = clue.IllegalMoney.ToString();
                this.tbFact.Text = clue.Fact;
                this.tbCheckName.Text = clue.CheckByName1;
                this.tbCheckDate.Text = clue.CheckDate.ToString("yyyyMMdd");
            }

        }

       


        // //生成supplierinfo对象
        ////type  新增 or 修改
        //private Models.Messaging<Models.ContractInfo> GenerateObject(string type)
        //{
        //    var mes = new Models.Messaging<Models.ContractInfo>();
        //    mes.Value = new Models.ContractInfo();
        //    mes.retType = 1;

        //    //new contract needed
        //    if (DropDownListContractType.SelectedValue.CanConvertTo<int>() && 
        //        DropDownListDepartment.SelectedValue.CanConvertTo<int>() &&
        //        DropDownListContractStatus.SelectedValue.CanConvertTo<int>() &&
        //        //DropDownSupplier.SelectedValue.CanConvertTo<int>() &&
        //        DropDownProject.SelectedValue.CanConvertTo<int>() &&
        //        ContractDate.Value.CanConvertTo<DateTime>())
        //    {
        //        //项目
        //        mes.Value.Project = new Models.ProjectInfo();
        //        mes.Value.Project.ProjectID = DropDownListContractType.SelectedValue.ConvertTo<int>();
        //        //部门ID
        //        mes.Value.Department = new Models.Departments();
        //        mes.Value.Department.DepartmentID = DropDownListDepartment.SelectedValue.ConvertTo<int>();

        //        //合同状态
        //        mes.Value.ContractStatus = new Models.TypeItem();
        //        mes.Value.ContractStatus.BaseTypeItemID = DropDownListContractStatus.SelectedValue.ConvertTo<int>();

        //        //合同类型
        //        mes.Value.ContractType = new Models.TypeItem();
        //        mes.Value.ContractType.BaseTypeItemID = DropDownListContractType.SelectedValue.ConvertTo<int>();


        //        //供方
        //        //mes.Value.Supplier = new Models.SupplierInfo();
        //        mes.Value.SupplierName = TextBoxSupplier.Text;

        //        //所属项目
        //        mes.Value.Project = new Models.ProjectInfo();
        //        mes.Value.Project.ProjectID = DropDownProject.SelectedValue.ConvertTo<int>();


        //        //合同名称
        //        mes.Value.ContractName = TextBoxContractName.Text;

        //        //合同编码
        //        mes.Value.ContractCode = TextBoxContractCode.Text;

        //        //合同金额
        //        if (TextBoxValue.Text.CanConvertTo<double>())
        //            mes.Value.ContractValue = TextBoxValue.Text.ConvertTo<double>();
        //        else mes.Value.ContractValue = 0.0;

        //        //备注
        //        mes.Value.Comment = TextBoxComment.Text;

        //        //归档
        //        mes.Value.Filing = TextBoxFiling.Text;

        //        //签约时间
        //        mes.Value.ContractDate = ContractDate.Value.ConvertTo<DateTime>();

        //        //创建人
        //        string username = Page.User.Identity.Name;
        //        if (String.IsNullOrEmpty(username))
        //            mes.Value.Creator = "anonymous";
        //        else mes.Value.Creator = username;
        //        mes.Value.CreateDate = System.DateTime.Now;
        //        mes.retType = 100;
        //        mes.Message = "获取ProjectInfo对象成功!";
        //    }
        //    else
        //    {
        //        mes.retType = 1;
        //        mes.Message = "新增参数供方类型或状态错误";
        //    }


        //    //modify contract needed
        //    if (mes.retType == 100 && type == "Modify")
        //    {
        //        if (ContractID.Value.CanConvertTo<int>() && Version.Value.CanConvertTo<Int64>())
        //        {
        //            //隐藏控件－合同ID不能变－
        //            mes.Value.ContractID = ContractID.Value.ConvertTo<int>();
        //            //隐藏控件－版本
        //            mes.Value.version = Version.Value.ConvertTo<Int64>();


        //            //修改人
        //            string username = Page.User.Identity.Name;
        //            if (String.IsNullOrEmpty(username))
        //                mes.Value.Modifier = "anonymous";
        //            else mes.Value.Modifier = username;
        //            mes.Value.ModifyDate = System.DateTime.Now;

        //            mes.retType = 200;
        //        }
        //        else
        //        {
        //            mes.retType = 2;
        //            mes.Message = "修改无法获取供方ID和版本!";
        //        }
        //    }

        //    return mes; 
        //}


        //private Models.Messaging<int> ChangeObject(string type)
        //{
        //    Models.Messaging<int> retMes;
        //    var mes = GenerateObject(type);

        //    if (mes.retType == 100)
        //    {//新增对象获取成功
        //        Services.ContractServices service = new Services.ContractServices();
        //        retMes = service.InsertData(mes.Value);
        //    }
        //    else if (mes.retType == 200)
        //    {//修改对象获取成功
        //        Services.ContractServices service = new Services.ContractServices();
        //        retMes = service.UpdateData(mes.Value);
        //    }
        //    else
        //    {//错误
        //        retMes = new Models.Messaging<int>();
        //        retMes.copy(mes);
        //    }
        //    LabelInfo.Text = retMes.Message;

        //    return retMes;
        //}

        //protected void LinkButtonApply_Command(object sender, CommandEventArgs e)
        //{
        //    ChangeObject(e.CommandArgument.ToString());   
        //}

        //protected void LinkButtonSave_Command(object sender, CommandEventArgs e)
        //{
        //    if (ChangeObject(e.CommandArgument.ToString()).retType == 100)
        //        LinkButtonBack_Command(sender, e); //保存成功!
        //}



        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WFMClues/ClueMgr.aspx");
        }
    }
}