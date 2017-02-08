using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    public class Region
    {
        public int RowID { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }

        public int ParentID { get; set; }
        public string ParentName { get; set; }

        public int DirectCity { get; set; }
        public int Level { get; set; }
        public int Seq { get; set; }
        public int Status { get; set; }

        public string RegionGuid { get; set; }
        public string ParentGuid { get; set; }

        public Int64 version { get; set; }        
    }


    public class RegionUser
    {
        public int RowID { get; set; }
        public string RegionUserID { get; set; }
        public int RegionID { get; set; }
        public int ParentID { get; set; }
        public string RegionGuid { get; set; }
        public string ParentGuid { get; set; }
        //public int RegionLevel { get; set; }

        public String UserName { get; set; }
        public Int64 version { get; set; }

        public DateTime LastActivityDate { get; set; }

        public String Password { get; set; }
        public String Createby { get; set; }  
       
        
    }
}
