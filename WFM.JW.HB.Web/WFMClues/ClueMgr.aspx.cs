using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using TB.ComponentModel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using System.Text;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace WFM.JW.HB.Web.WFMClues
{
    public partial class ClueMgr : CLBasePage
    {
        private Models.Region SelectedRegion;
        protected override void OnInit(EventArgs e)
        {
            this.Pages1.GoPage += new GoPageEventHandler(GoPage);
            base.OnInit(e);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitDropList();

                SelectedRegion = CurrentUserRegion;
                var mes = GetUrlPara<Models.Messaging<Models.RegionUser>>("id");
                if (mes.Value != null && mes.Value.Value != null)
                {
                    if (mes.retType == 100)
                    {
                        //ShowDialog.Value = mes.Message;
                        SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == mes.Value.Value.RegionGuid.ToUpper());

                        if (SelectedRegion == null || !base.IsRegionAuthentication(SelectedRegion.RegionGuid))
                            SelectedRegion = CurrentUserRegion;


                        if (SelectedRegion.Level == 2)
                        {
                            DropDownListCity.SelectedValue = SelectedRegion.ParentGuid;
                            InitContryList();

                            DropDownListContry.SelectedValue = SelectedRegion.RegionGuid;
                        }
                        else
                        {
                            DropDownListCity.SelectedValue = SelectedRegion.RegionGuid;
                            InitContryList();
                        }

                    }
                }
                GoPage(1);
            }
            else
            {

            }
        }

        private void InitDropList()
        {
            if (CurrentUserRegion == null) return;

            InitCityList(regionList, CurrentUserRegion);
            InitItem();

            //if (CurrentUserRegion.Level == 1)
            //    this.Label3.Text = "市州";
            //else if (CurrentUserRegion.Level == 2)
            //    this.Label3.Text = "县市区";

        }

        private void InitItem()
        {
            Services.BaseTypeItemServices bi = new Services.BaseTypeItemServices();
            var iList = bi.GetAllData();

            DropDownListItem.DataSource = iList.Where(x => x.ItemParent.BaseTypeID == 15).OrderBy(x => x.ItemOrder);
            DropDownListItem.DataValueField = "ItemValue";
            DropDownListItem.DataTextField = "ItemName";
            DropDownListItem.DataBind();
            DropDownListItem.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 
        }

        private void InitCityList(IEnumerable<Models.Region> rlist, Models.Region r)
        {
            //var citylist = rlist.ToList()
            //   .Where(x => (x.ParentGuid == CurrentUserRegion.RegionGuid));//|| x.RegionGuid == CurrentUserRegion.RegionGuid));

            var citylist = rlist.ToList()
                .Where(x => CurrentUserRegion.Level + CurrentUserRegion.DirectCity < 2 ?
                                  x.ParentGuid == CurrentUserRegion.RegionGuid :
                                  x.RegionGuid == CurrentUserRegion.RegionGuid);

            DropDownListCity.DataSource = citylist;

            DropDownListCity.DataValueField = "RegionGuid";
            DropDownListCity.DataTextField = "RegionName";
            DropDownListCity.DataBind();

            DropDownListCity.Items.Insert(0, new ListItem("请选择...", "all")); //动态插入指定序号的新项 

            if (citylist.Count() == 1)
                DropDownListCity.SelectedIndex = 1;
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

        //查询
        protected void LinkButtonQuery_Click(object sender, EventArgs e)
        {
            GoPage(1);

        }

        //新增
        protected void LinkButtonNew_Click(object sender, EventArgs e)
        {



        }

        //修改
        //传递 参数,绑定类型
        protected void LinkButtonModify_Click(object sender, EventArgs e)
        {
            var mes = GenereatePageParameter(1);

            if (mes.retType != 200) return;

            GetQuerySpec();
            mes.Value.RegionGuid = SelectedRegion.RegionGuid;

            if (mes.retType == 200)
            {
                Response.Redirect("~/WFMClues/CluesEdit.aspx?id=" + mes.Message);
            }
            else
            {//警告修改错误,请选择要修改的供方
                //showWarning("请选择要修改的供方");
                ShowDialog.Value = "请选择要修改的用户";
            }
        }

        //删除
        protected void LinkButtonDelete_Click(object sender, EventArgs e)
        {

        }

        private Models.IParametersSpecification GetQuerySpec()
        {
            if (CurrentUserRegion == null) return Models.CriteriaSpecification.GetFalse();

            //Models.IParametersSpecification spec = new Models.CriteriaSpecification("[InputError]", Models.CriteriaOperator.Equal, 0);
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
                Guid = CurrentUserRegion.RegionGuid;
            }

            if (!base.IsRegionAuthentication(Guid)) //权限控制
                return Models.CriteriaSpecification.GetFalse();

            SelectedRegion = regionList.ToList().Find(x => x.RegionGuid == Guid);
            if (SelectedRegion == null) return Models.CriteriaSpecification.GetFalse();

            if (SelectedRegion.Level != 0)
            {
                //Models.IParametersSpecification newspec = new Models.CriteriaSpecification("RegionGUID", Models.CriteriaOperator.Equal, Guid);
                // newspec = newspec.Or(new Models.CriteriaSpecification("ParentGUID", Models.CriteriaOperator.Equal, Guid));
                //  spec = spec.And(newspec);
            }

            if (tbID.Text.Length > 6)
                spec = spec.And(new Models.CriteriaSpecification("PersonID", Models.CriteriaOperator.Like, tbID.Text));

            if (DropDownList1.SelectedIndex != 0)
                spec = spec.And(new Models.CriteriaSpecification("Confirmed", Models.CriteriaOperator.Equal, DropDownList1.SelectedValue));

            if (DropDownList2.SelectedIndex != 0)
                spec = spec.And(new Models.CriteriaSpecification("IsClueTrue", Models.CriteriaOperator.Equal, DropDownList2.SelectedValue));

            if (DropDownListItem.SelectedIndex != 0)
                spec = spec.And(new Models.CriteriaSpecification("Table1", Models.CriteriaOperator.Equal, DropDownListItem.SelectedValue));

            if (DropDownList3.SelectedIndex != 0)
            { // 录入问题
                if (DropDownList3.SelectedIndex == 2)
                {
                    spec = spec.And(new Models.CriteriaSpecification("ClueType", Models.CriteriaOperator.Like, "不一致"));
                    spec = spec.And(new Models.CriteriaSpecification("ClueType", Models.CriteriaOperator.NotLike, "+"));
                }
                else if (DropDownList3.SelectedIndex == 1)
                {
                    Models.IParametersSpecification spectmp = new Models.CriteriaSpecification("ClueType", Models.CriteriaOperator.NotLike, "不一致");
                    spectmp = spectmp.Or(new Models.CriteriaSpecification("ClueType", Models.CriteriaOperator.Like, "+"));

                    spec = spec.And(spectmp);
                }


            }
            return spec;
        }

        //分页查询
        void GoPage(int CurrentPage)
        {
            CLGridViewClueMgrInfo.DataSource = null;
            CLGridViewClueMgrInfo.DataBind();

            var ps = this.Pages1.GetPageSupport();
            if (ps == null)
            {
                ps = new PageSupport(CLGridViewClueMgrInfo.PageSize);
                this.Pages1.SetPageSupport(ps);

                ps.SetStartIndex(0);
            }

            var spec = GetQuerySpec();
            Services.WFMClueServices service = new Services.WFMClueServices();
            ps.TotalCount = service.GetPagedDataCount(SelectedRegion.RegionCode, spec);

            ps.SetStartIndex((CurrentPage - 1) * ps.EachPageTotal);

            var list = service.GetPagedData(SelectedRegion.RegionCode, spec, ps.CurrentPageNo, ps.EachPageTotal);
            if (list == null) return;

            var typelist = new Services.BaseTypeItemServices().GetAllData();

            if (typelist == null) return;

            var showlist = from l in list
                           join r in typelist.Where(x => (x.ItemParent.BaseTypeID == 15)) on l.Table1.ToString() equals r.ItemValue
                           select new
                           {
                               CluesGuid = l.ClueGuid,
                               RegionName = SelectedRegion.RegionName,
                               PersonRegion = l.Region,
                               ID = l.ID,
                               Name = l.Name,
                               Confirmed = l.IsConfirmed,
                               ClueTrue = l.IsClueTrue,
                               Addr = l.Addr,
                               ClueType = l.Type,
                               DateRange = l.DateRange,
                               Table1 = r.ItemName,
                               Comment = l.Comment,
                               Seq = r.ItemOrder,
                               InputError = l.InputError
                           };
            if (showlist == null) return;

            CLGridViewClueMgrInfo.DataSource = showlist.OrderByDescending(x => x.RegionName).ThenBy(x => x.Seq);//.OrderBy(x => x.RegionName).ThenBy(x =>x.Seq); // showlist.ToList().OrderBy(x => x.Level).OrderBy(x => x.seq);
            CLGridViewClueMgrInfo.DataBind();

            Pages1.PageReload();
        }

        /**
        * type = 0 新增
        * type = 1 修改  
        * 
        * mes.Value = null 时 
        * */
        private Models.Messaging<Models.Clues> GenereatePageParameter(int type)
        {
            Models.Messaging<Models.Clues> mes = new Models.Messaging<Models.Clues>();
            mes.retType = 100;
            mes.Message = Guid.NewGuid().ToString();

            if (type == 1)
            {//修改       
                mes.retType = 1;
                if (selectedIndex.Value.CanConvertTo<int>())
                {//界面点击行号从1开始
                    int index = selectedIndex.Value.ConvertTo<int>(); //gridview start with 0
                    //获取supplierid
                    if (index >= 0 && index < CLGridViewClueMgrInfo.DataKeys.Count)
                    {// 检测gridview范围内
                        //if (CLGridViewUserMgrInfo.DataKeys[index].Value.CanConvertTo<int>())
                        {//获取gridview datakey中的值
                            //int RowID = CLGridViewUserMgrInfo.DataKeys[index].Value.ConvertTo<int>();
                            mes.Value = new Models.Clues();
                            mes.Value.ClueGuid = CLGridViewClueMgrInfo.DataKeys[index].Value.ToString();

                            mes.retType = 200;
                            //var r = userList.ToList().Find(x => x.RegionUserID == mes.Value.RegionUserID);
                            //if(r != null) 
                            //if(base.IsRegionAuthentication(r.RegionGuid))
                            //{
                            //    mes.retType = 200;
                            //    mes.Value.version = r.version;
                            //    mes.Value.UserName = r.UserName;
                            //}
                        }
                    }
                }
            }

            Page.Session[mes.Message] = mes;
            //Page.Cache.Insert(mes.Message, mes, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            return mes;
        }

        protected void CLGridViewRegionUserInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {//To contract details
                selectedIndex.Value = e.CommandArgument.ToString();
                LinkButtonModify_Click(null, null);
                //  LinkButtonDetails_Click(null, null);

            }
        }

        protected void LinkButtonImport_Click(object sender, EventArgs e)
        {
            var spec = GetQuerySpec();
            Services.WFMClueServices service = new Services.WFMClueServices();

            var list = service.GetAllData(SelectedRegion.RegionCode, spec);
            if (list == null) return;

            var typelist = new Services.BaseTypeItemServices().GetAllData();

            if (typelist == null) return;
            var showlist = from l in list
                           join r in typelist.Where(x => (x.ItemParent.BaseTypeID == 15)) on l.Table1.ToString() equals r.ItemValue
                           select new
                           {
                               CluesGuid = l.ClueGuid,
                               RegionName = SelectedRegion.RegionName,
                               PersonRegion = l.Region,
                               ID = l.ID,
                               Name = l.Name,
                               Confirmed = l.IsConfirmed,
                               ClueTrue = l.IsClueTrue,
                               Addr = l.Addr,
                               ClueType = l.Type,
                               DateRange = l.DateRange,
                               Table1 = r.ItemName,
                               Fact = l.Fact,
                               Seq = r.ItemOrder,
                               InputError = l.InputError,
                               IsCP = l.IsCP,
                               CheckDate = l.CheckDate,
                               CheckName = l.CheckByName1
                           };
            if (showlist == null) return;

            var num = showlist.Count();

            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet = null;



            int ReadCount = 60000;
            int sheetNum = 0;
            foreach (var l in showlist)
            {
                if (ReadCount >= 60000)
                {
                    sheet = book.CreateSheet("问题线索" + sheetNum.ToString());

                    //NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
                    //row.CreateCell(0).SetCellValue("问题线索汇总表");
                    var row = sheet.CreateRow(0);
                    row.CreateCell(0).SetCellValue("乡镇街道");
                    row.CreateCell(1).SetCellValue("项目名称");
                    row.CreateCell(2).SetCellValue("身份证号");
                    row.CreateCell(3).SetCellValue("姓名");
                    row.CreateCell(4).SetCellValue("地址");
                    row.CreateCell(5).SetCellValue("线索类型");
                    row.CreateCell(6).SetCellValue("简要事实");
                    row.CreateCell(7).SetCellValue("是否属实");
                    row.CreateCell(8).SetCellValue("是否党员");
                    row.CreateCell(9).SetCellValue("核查时间");
                    row.CreateCell(10).SetCellValue("核查人");


                    sheetNum++;
                    ReadCount = 0;
                }
                ReadCount++;

                NPOI.SS.UserModel.IRow rowData = sheet.CreateRow(ReadCount);
                rowData.CreateCell(0).SetCellValue(l.PersonRegion);
                rowData.CreateCell(1).SetCellValue(l.Table1);
                rowData.CreateCell(2).SetCellValue(l.ID);
                rowData.CreateCell(3).SetCellValue(l.Name);
                rowData.CreateCell(4).SetCellValue(l.Addr);
                rowData.CreateCell(5).SetCellValue(l.ClueType);

                if (l.Confirmed == 0)
                    continue;

                rowData.CreateCell(6).SetCellValue(l.Fact);

                rowData.CreateCell(7).SetCellValue(l.ClueTrue == 0 ? "不属实": "属实");
                rowData.CreateCell(8).SetCellValue(l.IsCP == 0 ? "否": "是");
                rowData.CreateCell(9).SetCellValue(l.CheckDate.ToString("yyyy-MM-dd"));
                rowData.CreateCell(10).SetCellValue(l.CheckName);
            }

            // 写入到客户端    
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);


            String FileName = HttpUtility.UrlEncode(SelectedRegion.RegionName + "问题线索") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", FileName));
            HttpContext.Current.Response.AddHeader("Content-Length", ms.Length.ToString());
            HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("gb2312");

            Response.BinaryWrite(ms.ToArray());
            book = null;
            ms.Close();
            ms.Dispose();

        }



        

    }
}