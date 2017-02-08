using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class ProjectInfo
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectShortName { get; set; }


        public string ProjectCode { get; set; }

        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { set; get; }
        
        
        public TypeItem ProjectType { get; set; }
        public TypeItem Status { get; set; }
        
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }

        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }

        public string Comment { set; get; }


        public double ContractsTotalValue { get; set; }
        public double ContractsNumbers { get; set; }
        public double ContractsTotalSettle { get; set; }



        public Int64 version { get; set; }

        public IEnumerable<ContractInfo> Contracts;
    }
}
