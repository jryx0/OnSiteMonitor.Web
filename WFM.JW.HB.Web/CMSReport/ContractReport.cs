using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarlosAg.ExcelXmlWriter;
using System.Xml.Linq;
using System.Collections;

namespace WFM.JW.HB.Web.CMSReport
{
    public class ContractReport
    {
        public Workbook SAReport { get; set; }

        public ContractReport()
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
            titlecell.Data.Text = "合同清单";
            titlecell.MergeAcross = 10;
            
           



            WorksheetRow HeadRow = sheet.Table.Rows.Add();

            HeadRow.Cells.Add("序号", DataType.String, "s1");
            HeadRow.Cells.Add("签约日期", DataType.String, "s1");
            HeadRow.Cells.Add("部门", DataType.String, "s1");
            HeadRow.Cells.Add("合同类别", DataType.String, "s1");
            HeadRow.Cells.Add("项目名称", DataType.String, "s1");
            HeadRow.Cells.Add("合同编号", DataType.String, "s1");
            HeadRow.Cells.Add("合同名称", DataType.String, "s1");
            HeadRow.Cells.Add("供方", DataType.String, "s1");
            HeadRow.Cells.Add("合同金额", DataType.String, "s1");
            HeadRow.Cells.Add("备注", DataType.String, "s1");
            HeadRow.Cells.Add("归档", DataType.String, "s1");
            

            int number = 0;           
            string style = "s1";

            ArrayList cvalue = new ArrayList();


            foreach (Models.ContractInfo sa in list)
            {
                WorksheetRow row = sheet.Table.Rows.Add();

                number++;
                
                cvalue.Add(number);

                row.Cells.Add(number.ToString(), DataType.String, "s1");
                //签约日期
                row.Cells.Add(sa.ContractDate.ToString("yyyy-MM"), DataType.String, style);

                //部门
                row.Cells.Add(sa.Department.DepartmentName, DataType.String, style);
                //合同类别
                row.Cells.Add(sa.ContractType.ItemName, DataType.String, style);
                //项目名称
                row.Cells.Add(sa.Project.ProjectName, DataType.String, style);
                //合同编号
                row.Cells.Add(sa.ContractCode, DataType.String, style);
                //合同名称
                row.Cells.Add(sa.ContractName, DataType.String, style);
                //供方 
                row.Cells.Add(sa.SupplierName, DataType.String, style);
                //合同金额 
                if (style == "s1")
                    row.Cells.Add(sa.ContractValue.ToString(), DataType.Number, "s1Number");
                else row.Cells.Add(sa.ContractValue.ToString(), DataType.Number, "s74Number");
                //备注
                row.Cells.Add(sa.Comment, DataType.String, style);

                 //归档
                row.Cells.Add(sa.Filing, DataType.String, style);

            }


            #region 统计信息
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

            WorksheetRow Row3 = sheet.Table.Rows.Add();


            var cell = Row3.Cells.Add();
            cell.StyleID = "totalNumber";
            cell.Data.Type = DataType.Number;
            cell.Index = 9;

            
            cell.Formula = "=SUM(" + formula + ")";
            #endregion


        }


    }
}