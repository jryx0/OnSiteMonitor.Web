using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarlosAg.ExcelXmlWriter;
using System.Collections;

namespace WFM.JW.HB.Web.CMSReport
{
    public class CSReport
    {
        public Workbook SAReport { get; set; }

        public CSReport()
        {
            SAReport = new Workbook();

            GenerateStyle(SAReport.Styles);
        }

        protected void GenerateStyle(WorksheetStyleCollection styles)
        {
            WorksheetStyle Default = styles.Add("Default");
            Default.Name = "Normal";
            Default.Font.FontName = "宋体";
            Default.Font.Size = 12;
            Default.Font.Color = "#000000";
            Default.Alignment.Vertical = StyleVerticalAlignment.Center;


            WorksheetStyle Border_1 = styles.Add("s1");
            Border_1.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            Border_1.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            Border_1.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            Border_1.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);


            WorksheetStyle s74 = styles.Add("s74");
            s74.Font.FontName = "宋体";
            s74.Font.Size = 11;
            s74.Font.Color = "#A6A6A6";
            s74.Interior.Color = "#D9D9D9";
            s74.Interior.Pattern = StyleInteriorPattern.Solid;
            s74.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s74.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s74.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s74.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);


            WorksheetStyle s74Number = styles.Add("s74Number");
            s74Number.Font.FontName = "宋体";
            s74Number.Font.Size = 11;
            s74Number.Font.Color = "#A6A6A6";
            s74Number.Interior.Color = "#D9D9D9";
            s74Number.Interior.Pattern = StyleInteriorPattern.Solid;
            s74Number.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s74Number.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s74Number.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s74Number.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            s74Number.NumberFormat = "\"¥\"#,##0.00;\"¥\"\\-#,##0.00";


            WorksheetStyle s1Number = styles.Add("s1Number");
            s1Number.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s1Number.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s1Number.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s1Number.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            s1Number.NumberFormat = "\"¥\"#,##0.00;\"¥\"\\-#,##0.00";

            WorksheetStyle totalNumber = styles.Add("totalNumber");
            totalNumber.NumberFormat = "\"¥\"#,##0.00;\"¥\"\\-#,##0.00";

            WorksheetStyle title = styles.Add("title");
            title.Font.FontName = "宋体";
            title.Font.Size = 15;
            title.Font.Bold = true;
            title.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            title.Alignment.Vertical = StyleVerticalAlignment.Center;
            

        }

        public void GenerateReport(string sheetname, IEnumerable list)
        {
            Worksheet sheet = SAReport.Worksheets.Add(sheetname);

            WorksheetRow TitleRow = sheet.Table.Rows.Add();

            WorksheetCell titlecell = TitleRow.Cells.Add();
            titlecell.StyleID = "title";
            titlecell.Data.Type = DataType.String;
            titlecell.Data.Text = "签约情况";
            titlecell.MergeAcross = 6;

            WorksheetCell titlecell1 = TitleRow.Cells.Add();
            titlecell1.StyleID = "title";
            titlecell1.Data.Type = DataType.String;
            titlecell1.Data.Text = "结算情况";
            titlecell1.MergeAcross = 9;


            WorksheetRow HeadRow = sheet.Table.Rows.Add();

            HeadRow.Cells.Add("序号", DataType.String, "s1");
            HeadRow.Cells.Add("签约日期", DataType.String, "s1");
            

            HeadRow.Cells.Add("项目名称", DataType.String, "s1");            

            HeadRow.Cells.Add("合同名称", DataType.String, "s1");
            
            HeadRow.Cells.Add("合同金额", DataType.String, "s1");
           
            
            HeadRow.Cells.Add("结算日期", DataType.String, "s1");
            HeadRow.Cells.Add("结算编号", DataType.String, "s1");
            //HeadRow.Cells.Add("增加变更", DataType.String, "s1");
            HeadRow.Cells.Add("报审金额", DataType.String, "s1");
            HeadRow.Cells.Add("审核金额", DataType.String, "s1");
            HeadRow.Cells.Add("审减金额", DataType.String, "s1");
            HeadRow.Cells.Add("审核人", DataType.String, "s1");
            HeadRow.Cells.Add("对账人", DataType.String, "s1");
            HeadRow.Cells.Add("履约情况", DataType.String, "s1");
            HeadRow.Cells.Add("结算状态", DataType.String, "s1");
            HeadRow.Cells.Add("供方", DataType.String, "s1");
            HeadRow.Cells.Add("备注", DataType.String, "s1");
            HeadRow.Cells.Add("归档", DataType.String, "s1");

            int number = 0;
            int contractid = 0;
            string style = "s1";

            ArrayList cvalue = new ArrayList();


            foreach (Models.SettlementRecord sa in list)
            {
                WorksheetRow row = sheet.Table.Rows.Add();

                number++;
                if (contractid == sa.Contract.ContractID)
                {
                    style = "s74";
                }
                else
                {
                    style = "s1";
                    cvalue.Add(number);
                }
                contractid = sa.Contract.ContractID;


                row.Cells.Add(number.ToString(), DataType.String, "s1");
                //签约日期
                row.Cells.Add(sa.Contract.ContractDate.ToString("yyyy-MM"), DataType.String, style);

                
                //项目名称
                row.Cells.Add(sa.Contract.Project.ProjectName, DataType.String, style);
                

                //合同名称
                row.Cells.Add(sa.Contract.ContractName, DataType.String, style);
               
                //合同金额 
                if (style == "s1")
                    row.Cells.Add(sa.Contract.ContractValue.ToString(), DataType.Number, "s1Number");
                else row.Cells.Add(sa.Contract.ContractValue.ToString(), DataType.Number, "s74Number");
                

                //结算日期 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.SettlementDate.ToString("yyyy-MM") : "", DataType.String, "s1");
                //结算编号
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.SettlementCode : "", DataType.String, "s1");
                //增加变更 
              //  row.Cells.Add(sa.SettlementInfoID != 0 ? sa.AmountChanged.ToString() : "", sa.SettlementInfoID != 0 ? DataType.Number : DataType.String, "s1Number");
                //报审金额 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.AmountDeclared.ToString() : "", sa.SettlementInfoID != 0 ? DataType.Number : DataType.String, "s1Number");
                //审核金额 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.AmountAudited.ToString() : "", sa.SettlementInfoID != 0 ? DataType.Number : DataType.String, "s1Number");
                //审减金额 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.AuditDeduction.ToString() : "", sa.SettlementInfoID != 0 ? DataType.Number : DataType.String, "s1Number");
                //审核人 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.Auditor : "", DataType.String, "s1");
                //对账人 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.Statementor : "", DataType.String, "s1");
                //履约情况
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.Contract.ContractStatus.ItemName : "", DataType.String, style);
                //结算状态
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.SettlementStatus.ItemName : "", DataType.String, style);
                //供方 
                row.Cells.Add(sa.Contract.SupplierName, DataType.String, style);
                //备注 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.Comment : "", DataType.String, "s1");
                //归档 
                row.Cells.Add(sa.SettlementInfoID != 0 ? sa.Filing : "", DataType.String, "s1");
            }


            #region 合同总金额
            WorksheetRow Row3 = sheet.Table.Rows.Add();
            var cell = Row3.Cells.Add();
            cell.StyleID = "totalNumber";
            cell.Data.Type = DataType.Number;
            cell.Index = 5;

            string formula = null;

            number++;
            foreach (int i in cvalue)
            {
                if (String.IsNullOrEmpty(formula))
                {
                    formula = "R[" + (i - number).ToString() + "]C";
                }
                else formula += "+ R[" + (i - number).ToString() + "]C";

            }
            cell.Formula = "=SUM(" + formula + ")";
            #endregion


            formula = "R[" + (0 - number + 1).ToString() + "]C:R[-1]C";
            
            cell = Row3.Cells.Add();
            cell.StyleID = "totalNumber";
            cell.Data.Type = DataType.Number;
            cell.Index = 8;
            cell.Formula = "=SUM(" + formula + ")";

            cell = Row3.Cells.Add();
            cell.StyleID = "totalNumber";
            cell.Data.Type = DataType.Number;
            cell.Index = 9;
            cell.Formula = "=SUM(" + formula + ")";

            cell = Row3.Cells.Add();
            cell.StyleID = "totalNumber";
            cell.Data.Type = DataType.Number;
            cell.Index = 10;
            cell.Formula = "=SUM(" + formula + ")";

            //cell = Row3.Cells.Add();
            //cell.StyleID = "totalNumber";
            //cell.Data.Type = DataType.Number;
            //cell.Index = 18;
            //cell.Formula = "=SUM(" + formula + ")";
        }
    }
}