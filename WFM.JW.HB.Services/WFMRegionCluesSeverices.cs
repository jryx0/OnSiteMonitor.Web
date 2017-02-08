using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMRegionClueSeverices
    {
        private String _regionCode;
        public WFMRegionClueSeverices(String RegionCode)
        {
            _regionCode = RegionCode;
        }

        public IEnumerable<Clues> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Clues> GetAllData(IParametersSpecification paraSpec)
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();

            return rcr.GetData(_regionCode, paraSpec);
        }

        public Messaging<int> InsertData(List<Clues> data)
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();
            return rcr.InsertData(_regionCode, data);
        }

        public Messaging<int> UpdateData(List<Clues> data)
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();
            return rcr.UpdateData(_regionCode, data);
        }

        public Messaging<int> InitTable()
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();
            return rcr.InitTable(_regionCode);
        }

        public int ExecuteNonQuery(String sql)
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();
            return rcr.ExecuteNonQuery(sql);
        }

        public Messaging<int> GenerateReport(String regioncode, String regionguid)
        {
            WFMRegionClueRepository rcr = new Repository.EntLib.WFMRegionClueRepository();
            return rcr.GenerateReport(regioncode, regionguid);

        }
    }
}
