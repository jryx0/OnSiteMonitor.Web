using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;

namespace UploaderFileMonitor
{
    public class Reports
    {

        public static String FillReportByRegion(DAL.MyDataBase sqlserver,  Region region)
        {            
            String strRet = "";

            String Sql = @"Insert into ClueDate_CountBy(RegionGuid, Contry, Items, Clues) 
                            SELECT @Guid  PersonRegion, table1, COUNT(DataGuid)AS TotalClues
                            FROM     ClueData_HBESLC
                            GROUP BY  PersonRegion, table1";
          
            try
            {
                Sql = Sql.Replace("@Guid", region.RegionGuid).Replace("@table", region.RegionCode);
                sqlserver.ExecuteNonQuery(Sql);

            }
            catch(Exception ex)
            {

            }
            return strRet;
        }

        public static String FillReportByItem(DAL.MyDataBase sqlserver, String TableName)
        {

            String strRet = "";
            try
            {


            }
            catch (Exception ex)
            {

            }
            return strRet;
        }


        public static String FillReport(DAL.MyDataBase sqlserver, String TableName)
        {

            String strRet = "";
            try
            {


            }
            catch (Exception ex)
            {

            }
            return strRet;
        }
    }
}
