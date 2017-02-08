using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    [Serializable]
    public class Employees
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public bool Enable { get; set; }
        
        public Departments Department { get; set; }

        public bool IsUser { get; set; }
        public string UserName{get; set;}
       
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }

        public Int64 version { get; set; }
    }
}
