using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class BaseType 
    {
        public int BaseTypeID { get; set; }
        public string BaseTypeName { get; set; }
        public int TypeOrder { get; set; }
        public string Comment { get; set; }
        public bool Enable { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyDate
        {
            get;
            set;
        }
        public Int64 version { get; set; }


        public IEnumerable<TypeItem> HaveItem;
    }   
}
