using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class ContractInfo
    {
        public int ContractID { get; set; }

        public string ContractCode { get; set; }
        public string ContractName { get; set; }
        public double ContractValue { get; set; }
        public double ContractSettle { get; set; }
        public TypeItem ContractStatus { get; set; }
        public TypeItem ContractType { get; set; }

        public DateTime ContractDate { get; set; }

        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }

        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }

        public string Comment { set; get; }
        public string Filing { set; get; }

        public Int64 version { get; set; }

        public Departments Department { set; get; }
       // public SupplierInfo Supplier { set; get; }
        public string SupplierName { set; get; }
        public ProjectInfo Project { set; get; }

        public IEnumerable<SettlementRecord> HaveSettlements { set; get; }
    }
}
