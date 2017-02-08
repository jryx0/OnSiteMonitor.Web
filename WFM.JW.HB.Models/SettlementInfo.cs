using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class SettlementRecord
    {
        public int SettlementInfoID { get; set; }
        public TypeItem SettlementStatus{ get; set; }
    
        public string SettlementCode { get; set; }
        public string SettlementInfo { get; set; }
        public DateTime SettlementDate { get; set; }
        
        public double AmountChanged { get; set; }
        public double AmountDeclared { get; set; }
        public double AmountAudited { get; set; }
        public double AuditDeduction { get; set; }
        
        public string Auditor { get; set; }
        public string Statementor { get; set; }
        public string Filing { get; set; }
        public string Comment { get; set; }
        
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime modifyDate { get; set; }

        public Int64 version { get; set; }

        public ContractInfo Contract{get; set;}
    }
}
