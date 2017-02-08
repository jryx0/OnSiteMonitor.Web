using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class Departments
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public bool Enable { get; set; }
        
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }
        public Int64 version { get; set; }

        public IEnumerable<Employees> HaveEmployees;
    }
}
