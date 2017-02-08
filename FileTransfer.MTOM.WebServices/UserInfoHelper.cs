using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WFM.JW.HB.Services;
using WFM.JW.HB.Models;

namespace FileTransfer.MTOM.WebServices
{
    public class UserInfoHelper
    {
        private IEnumerable<Region> RegionList;
        private IEnumerable<RegionUser> UserList;
        public UserInfoHelper()
        {
            Init();
        }

        public UserRegionInfo GetRegionInfo(string username)
        {
            UserRegionInfo uri = new UserRegionInfo();

            uri.RegionNameString = GetUserRegion(username);
            uri.RegionPathString = GenerateRegionPath(username);
            uri.RegionCode = GetUserRegioinCode(username);

            var r = GetRegionbyName(username);
            if (r != null)
                uri.RegionGuid = r.RegionGuid;

            return uri;
        }

        private string GetUserRegioinCode(string username)
        {
            var r = GetRegionbyName(username);
            if (r == null)
                return "";
            return r.RegionCode;
        }

        public String GenerateRegionPath(string username)
        {
            //if (UserList == null || RegionList == null)
            //    return "";

            //var u = UserList.ToList().Find(x => x.UserName == username);
            //if (u == null) return "";

            //var r = RegionList.ToList().Find(x => x.RegionGuid == u.RegionGuid);

            var r = GetRegionbyName(username);
            if (r == null)
                return "";
            else
                return GenerateRegionPath(r);
        }

        public String GetUserRegion(string username)
        {
            var u = UserList.ToList().Find(x => x.UserName == username);
            if (u == null) return "";

            var r = RegionList.ToList().Find(x => x.RegionGuid == u.RegionGuid);
            if (r == null) return "";

            var p = RegionList.ToList().Find(x => x.ParentGuid == u.ParentGuid);
            if (p == null) return r.RegionName + ";";


            return p.RegionName + ";" + r.RegionName;
        }

        public Region GetRegionbyName(string username)
        {
            if (UserList == null || RegionList == null)
                return null;

            var u = UserList.ToList().Find(x => x.UserName == username);
            if (u == null) return null;

            var r = RegionList.ToList().Find(x => x.RegionGuid == u.RegionGuid);

            return r;
        }

        protected void Init()
        {           
            WFMRegionUserServices rus = new WFMRegionUserServices();
            UserList = rus.GetAllData();

            WFMRegionServices rs = new WFMRegionServices();
            RegionList = rs.GetAllData();
        }
        protected System.Guid CreateGuid()
        {
            return System.Guid.NewGuid();
        } 
        protected String GenerateRegionPath(Region region)
        {
            string strRet = "";
            var r = RegionList.ToList().Find(x => x.RegionGuid == region.ParentGuid);
            if (r == null)
                strRet =  region.RegionName;
            else strRet = GenerateRegionPath(r) + "\\" + region.RegionName;

            return strRet;
        }
    }
}