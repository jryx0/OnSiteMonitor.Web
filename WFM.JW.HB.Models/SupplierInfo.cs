using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class SupplierInfo
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public TypeItem SupplierType { get; set; }
        public bool Enable { get; set; }
        public string Comment { get; set; }
       
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }

        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }

        public Int64 version { get; set; }

        //public IEnumerable<ContractInfo> Contract;
    }
}
