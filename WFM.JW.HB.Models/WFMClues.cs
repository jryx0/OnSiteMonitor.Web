using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    //public class WFMClues
    //{
    //    public String CluesGuid { get; set; }
    //    public String RegionName;
    //    public String PersonRegion { get; set; }
    //    public String ID { get; set; }
    //    public String Name { get; set; }
    //    public String Addr { get; set; }
    //    public String ClueType { get; set; }
        
    //    public String DateRange { get; set; }
    //    public String Table1 { get; set; }
    //    public int InputError { get; set; }
    //    public int IsConfirmed { get; set; }
    //    public String Comment { get; set; }
    //}

    [Serializable]
    public class Clues
    {        
        public int RowID { get; set; }               
        public String ClueGuid { get; set; }
        public String RegionGuid;
        public string ID { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public string Region { get; set; }
        public string Type { get; set; }
     //   public float Amount { get; set; }
        public string DateRange { get; set; }
        public int Table1 { get; set; }
       // public int Table2 { get; set; }
        public int IsConfirmed { get; set; }
        public int IsClueTrue { get; set; }
        public int IsCompliance { get; set; }
        public int IsCP { get; set; }
        public string Fact { get; set; }
        public float IllegalMoney { get; set; }
        public DateTime CheckDate { get; set; }
        public string CheckByName1 { get; set; }
        public string CheckByName2 { get; set; }
        public string ReCheckFact { get; set; }
        public int ReCheckType { get; set; }
        public string ReCheckByName1 { get; set; }
        public int InputError { get; set; }
        public String Comment { get; set; }

        public void Clear()
        {
            ClueGuid = "";
            RegionGuid = "";

            Name = "";
            Addr = "";
            Region = "";
            Type = "";
            
            DateRange = "";
            Table1 = 0;
            
            IsConfirmed = 0;
            IsClueTrue = 0;
            IsCompliance = 0;
            IsCP = 0;
            Fact = "";
            IllegalMoney = 0.0f;

            CheckByName1 = "";
            CheckByName2 = "";
            ReCheckFact = "";
            ReCheckType = 0;
            ReCheckByName1 = "";
        }
    }

    public class WFMClueSummary
    {
        //单位  乡镇 项目 线索类型  线索总数 录入错误 问题数  查实数
        public int ID { get; set; }
        public string RegionGuid { get; set; } //单位Guid
        public string RegionName { get; set; }  //单位名称
        public string ContryName { get; set; } //乡镇
        public string ItemType { get; set; } //项目      
        public string ClueType { get; set; } // 线索类型
        public int TotalClues { get; set; }
        public int InputErrors { get; set; }
        public int Problems { get; set; }
        public int Confirmed { get; set; }
        public int Seq { get; set; }
        public int IsTrue { get; set; }
    }
}
