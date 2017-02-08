using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    public class WFMUploadTask
    { 
        public String TaskGuid{get; set;}
        public String UserRegionPath{get; set;}
        public String UserName{get; set;}
        public String FilePath{get; set;}
        public String FileName{get; set;}
        public int Status{get; set;}
        public int TStatus{get; set;}
        public DateTime CreateTime {get; set;}
        public DateTime OpTime{get; set;}
        public Int64 Version{get; set;}
        public String ErrorMessage{get; set;}
        public String ClientTaskName { get; set;}
        public String ClientTaskComment { get; set;}

        public String RegionName { get; set; }
        public String RegionCode { get; set; }
        public String ParentName { get; set; }

        public int TotalClues { get; set; }

        public String RegionGuid { get; set; }
        public String ParentGuid { get; set; }

       public String IsRead { get; set; }
    }
}
