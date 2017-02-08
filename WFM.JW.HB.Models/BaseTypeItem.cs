using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class TypeItem 
    {
        public int BaseTypeItemID { get; set; }
        public BaseType ItemParent { get; set; }
        
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public string ItemValueType { get; set; }
        public int ItemOrder { get; set; }
        public bool Enable { get; set; }
        
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Comment { get; set; }

        public Int64 version { get; set; }         
    }
}
