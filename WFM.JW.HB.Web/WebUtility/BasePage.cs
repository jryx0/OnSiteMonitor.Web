using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TB.ComponentModel;
using WFM.JW.HB.Models;
using System.Web.UI.WebControls;



namespace WFM.JW.HB.Web
{
    public class CLBasePage : System.Web.UI.Page
    {
        
        protected IEnumerable<Models.RegionUser> userList;
        protected IEnumerable<Models.Region> regionList;
        protected IEnumerable<Models.TypeItem> baseTypeList;
        protected Models.Region CurrentUserRegion;

        

        protected void GetPara<T>(string id, Messaging<T> mes)
        {
            if (Page.Session[id] == null)
            //if (Page.Cache[id] == null)
            {
                mes.retType = 2;
                mes.Message = "cache数据不存在或超时";
            }
            else
            {
                try
                {

                    //if (Page.Cache[id].CanConvertTo<T>())
                    //    mes.Value = Page.Cache[id].ConvertTo<T>();
                    //else
                    //    mes.Value = (T)Page.Cache[id];


                    if (Page.Session[id].CanConvertTo<T>())
                        mes.Value = Page.Session[id].ConvertTo<T>();
                    else
                        mes.Value = (T)Page.Session[id];

                    mes.retType = 100;
                    mes.Message = "参数获取成功";

                    Page.Session.Remove(id);
                }
                catch (Exception ex)
                {
                    mes.retType = 3;
                    mes.Message = "[非法参数]" + ex.Message;
                    mes.Value = default(T);
                }
            }
        }
        //cached=true 获取cache中的参数
        //cached=false 不获取
        protected Messaging<T> GetUrlPara<T>(string paraName, bool cached = true)
        {
            Messaging<T> mes = new Messaging<T>();
            mes.retType = -1;
            if (Request.QueryString.Count == 0 || String.IsNullOrEmpty(Request.QueryString[paraName]))
            {
                mes.retType = 1;
                mes.Message = "无参数传入";
            }
            else
            {
                string id = Request.QueryString[paraName];
                if (cached)
                {
                    GetPara<T>(id, mes);
                    //if (Page.Cache[id] == null)
                    //{
                    //    mes.retType = 2;
                    //    mes.Message = "cache数据不存在或超时";
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        mes.Value = Page.Cache[id].ConvertTo<T>(); 

                    //        mes.retType = 100;
                    //        mes.Message = "参数获取成功";

                    //        Page.Cache.Remove(id);
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        mes.retType = 3;
                    //        mes.Message = "[非法参数]" + ex.Message;
                    //    }
                    //}
                }
                else
                {
                    mes.retType = 100;
                    mes.Message = "参数获取成功";

                    if (id.CanConvertTo<T>())
                        mes.Value = id.ConvertTo<T>();
                    else
                    {
                        mes.retType = 4;
                        mes.Message = "参数无法转换";
                    }
                }
            }
            return mes;
        }

        protected override void OnInit(EventArgs e)
        {
            if (Session["userList"] == null)
            {
                Services.WFMRegionUserServices rus = new Services.WFMRegionUserServices();
                userList = rus.GetAllData();
                Session["userList"] = userList;
            }
            else
                try {
                    userList = (IEnumerable<Models.RegionUser>)Session["userList"];
                }catch
                {
                    Services.WFMRegionUserServices rus = new Services.WFMRegionUserServices();
                    userList = rus.GetAllData();
                    Session["userList"] = userList;
                }

            if (Application["baseTypeList"] == null)
            {
                Services.BaseTypeItemServices bs = new Services.BaseTypeItemServices();
                baseTypeList = bs.GetAllData();
                Application["baseTypeList"] = baseTypeList;
            }
            else
                try
                {
                    baseTypeList = (IEnumerable<Models.TypeItem>)Application["baseTypeList"];
                }
                catch
                {
                    Services.BaseTypeItemServices bs = new Services.BaseTypeItemServices();
                    baseTypeList = bs.GetAllData();
                    Application["baseTypeList"] = baseTypeList;
                }

            if (Application["regionList"] == null)
            {
                Services.WFMRegionServices rs = new Services.WFMRegionServices();
                regionList = rs.GetAllData();

                Application["regionList"] = regionList;
            }
            else
                try
                {
                    regionList = (IEnumerable<Models.Region>)Application["regionList"];
                }
                catch
                {
                    Services.WFMRegionServices rs = new Services.WFMRegionServices();
                    regionList = rs.GetAllData();

                    Application["regionList"] = regionList;
                }

            
            CurrentUserRegion = GetCurrentUserRegion();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            SiteMaster master = (SiteMaster)this.Master;
            master.CurrentRegion = CurrentUserRegion.ParentName + CurrentUserRegion.RegionName;
            base.OnLoad(e);
        }

        protected Models.Region GetCurrentUserRegion()
        {
            if (userList == null) return null;
            var ru = userList.ToList().Find(x => x.UserName == Page.User.Identity.Name);
            if (ru == null) return null;

            //获取用户区域
            if (regionList == null) return null;
           // var r = regionList.ToList().Find(x => x.RowID == ru.RegionID);
            var r = regionList.ToList().Find(x => x.RegionGuid ==  ru.RegionGuid);
            if (r == null) return null;

            return r;
        }

        protected bool IsRegionAuthentication(string RegionGuid)
        {
            if (regionList == null) return false;
            //自身
            if (RegionGuid == CurrentUserRegion.RegionGuid) return true;

            //查找父节点是否为currentRegion
            var r = regionList.ToList().Find(x => (x.RegionGuid == RegionGuid));
            if (r == null)
                return false;
            else if (r.ParentGuid == CurrentUserRegion.RegionGuid)
                return true;
            else return IsRegionAuthentication(r.ParentGuid);
        }

        protected String GenerateRegionPath(Region region)
        {
            string strRet = "";
            if (regionList == null)
                return region.RegionName;

            var r = regionList.ToList().Find(x => x.RegionGuid == region.ParentGuid);
            if (r == null)
                strRet = region.RegionName;
            else strRet = GenerateRegionPath(r) + "\\" + region.RegionName;

            return strRet;
        }

        protected List<Region> GenerateAllChild(List<Region> rList, Region region)
        {
            if (rList == null)
                rList = new List<Region>();

            if (region == null)
                return rList;

            var rs = regionList.ToList().FindAll(x => x.ParentGuid == region.RegionGuid);
            if (rs.Count == 0)
                rList.Add(region);
            else
            {
                foreach(var r in rs)
                    GenerateAllChild(rList, r);
            }

            return rList;
        }
    }

}

