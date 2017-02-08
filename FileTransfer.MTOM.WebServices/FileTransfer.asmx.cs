using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;

using WFM.JW.HB.Services;
using WFM.JW.HB.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.MTOM.WebServices
{
    /// <summary>
    /// FileTransfer 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://FileTransfer.MTOM.WebServices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class FileTransferWebService : System.Web.Services.WebService
    {
        string UploadRootPath = "";
        public FileTransferWebService()
        {
            UploadRootPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        }


        #region Login
        [WebMethod(EnableSession = true)]
        public string Login(string logintoken, string UserName, string Password)
        {
            if (logintoken == null)
                return "";

            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri != null && uri.UserName == UserName)                
                return logintoken;

            string loginToken = System.Guid.NewGuid().ToString();
            WFMRegionUserServices rus = new WFMRegionUserServices();

            //RegionUser rur = new RegionUser();
            //rur.UserName = UserName;
            //rur.Password = Password;
            //rur.RegionGuid = "FA7F4A73-0D8D-4716-82A1-4381C9933D85";
            //rur.Createby = UserName;
            //rur.ParentGuid = "FA7F4A73-0D8D-4716-82A1-4381C9933D85";
            //rus.InsertData(rur);

            var list = rus.GetAllData(new CriteriaSpecification("a.UserName", CriteriaOperator.Equal, UserName));
            if (list.ToList().Count == 0)
                return "";

            var ru = list.First();

            if (ru != null && ru.Password == Password)
            {
                UserInfoHelper ui = new UserInfoHelper();
                var ri = ui.GetRegionInfo(UserName);
                if (ri != null)
                {
                    ri.UserName = UserName;
                    Session[loginToken] = ri;
                    return loginToken;
                }
            }

            return "";           
        }
        #endregion

        #region UpdateCheckData
        [WebMethod]
        public string Ping()
        {
            return "hello";
        }

        [WebMethod(EnableSession = true)]
        public String GetDataStatus(string logintoken, string dbInfo)
        {
            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri == null)
                return "";

            WFMUploadTaskServices wts = new WFMUploadTaskServices();
             
            var list = wts.GetAllData(new CriteriaSpecification("a.FileName", CriteriaOperator.Equal, dbInfo));
            if (list == null || list.Count() == 0)
                return "";

            var t = list.First();


            var rName = uri.RegionNameString.Split(';');
            if(rName.Length == 2)
                if(rName[1] == t.RegionName)
                    return t.Status.ToString() +";" +t.TStatus.ToString();

            return "";
        }

        [WebMethod(EnableSession = true)]
        public String GetUpLoadInfo(string logintoken)
        {
            String strRet = "";
            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri == null)
                return "请登录！";

            WFMUploadTaskServices wts = new WFMUploadTaskServices();

            IParametersSpecification spec = new  CriteriaSpecification("a.Status", CriteriaOperator.Equal, 20);
            var list = wts.GetAllData(spec.And(new CriteriaSpecification("a.UserName", CriteriaOperator.Equal, uri.UserName)));

            foreach(var t in list)
            {
                strRet += t.FilePath + ";" + t.FileName + ";" + t.ClientTaskName + ";" + t.ClientTaskComment + ";"
                    + t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + t.UserName + ";" + t.Status + ";" + t.TStatus + "^";
            }

            return strRet;
        }

        [WebMethod(EnableSession = true)]
        public String SetDefaultData(string logintoken, string dbInfo)
        {
            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri == null)
                return "请登录！";

            WFMUploadTaskServices wts = new WFMUploadTaskServices();
            var mes = wts.SetDefaultData(uri.UserName, dbInfo);

            if (mes.retType == 100)
                mes.Message = "";

            return mes.Message;
        }

        [WebMethod(EnableSession = true)]
        public byte[] GetClues(string logintoken, int RowID, String ID)
        {
            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri == null)
                return null;

            WFMRegionClueSeverices rcs = new WFMRegionClueSeverices(uri.RegionCode.Trim());
            IParametersSpecification spec = new CriteriaSpecification("RowID", CriteriaOperator.Equal, RowID);
            spec = spec.And(new CriteriaSpecification("PersonID", CriteriaOperator.Equal, ID));

            var cList = rcs.GetAllData(spec);
            if (cList == null || cList.Count() == 0)
                return null;

            
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, cList.ToList());//序列化对象，写入ms流中  
            ms.Position = 0;
            //byte[] bytes = new byte[ms.Length];//这个有错误
            byte[] bytes = ms.GetBuffer();
            var compressedBuffer = DataHelper.Compress(bytes);

            /*
            var decompressBuf = DataHelper.Decompress(compressedBuffer);

            MemoryStream dms = new MemoryStream(decompressBuf);
            var newlist = (IEnumerable<Clues>)formatter.Deserialize(dms);
            */     

            return compressedBuffer;
        }


        [WebMethod(EnableSession = true)]
        public byte[] UpdateReCheckData(string logintoken, byte[] buffer)
        {
            return null;
        }

        [WebMethod(EnableSession = true)]
        public byte[] UpdateCheckData(string logintoken, byte[] buffer)
        {
            UserRegionInfo uri = (UserRegionInfo)Session[logintoken];
            if (uri == null)
                return null;            

            var newbuf = DataHelper.Decompress(buffer);
            MemoryStream dms = new MemoryStream(newbuf);
            BinaryFormatter formatter = new BinaryFormatter();

            dms.Position = 0;
            var newlist = (IEnumerable<WFM.JW.HB.Models.Clues>)formatter.Deserialize(dms);

            WFMRegionClueSeverices rcs = new WFMRegionClueSeverices(uri.RegionCode.Trim());
            
            var mes = rcs.UpdateData(newlist.ToList());

            if(mes.retType != 100)
            {
                string log = "[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss sss") + "][" + uri.RegionPathString + "]";
                CustomSoapException("上传错误", log　+ mes.Message);
            }

            MemoryStream ms = new MemoryStream();
            //创建序列化的实例            
            formatter.Serialize(ms, newlist.ToList());//序列化对象，写入ms流中  
            ms.Position = 0;
            //byte[] bytes = new byte[ms.Length];//这个有错误
            byte[] bytes = ms.GetBuffer();
            var compressedBuffer = DataHelper.Compress(bytes);

            return compressedBuffer;
        }

        #endregion

        #region preParam     
        [WebMethod(EnableSession = true)]
        public string GetCurrentUserPath(string loginToken)
        {
            string strPath = "";
            try
            {
                var ui = (UserRegionInfo)Session[loginToken];
                if(ui != null)
                    strPath = ui.RegionPathString;                 
            }
            catch (Exception ex)
            {
            }
                        
            return strPath;
        }

        [WebMethod(EnableSession = true)]
        public string GetCurrentUserInfo(string loginToken)
        {
            string strPath = "";
            try
            {
                var ui = (UserRegionInfo)Session[loginToken];
                if (ui != null)
                    strPath = ui.RegionNameString;
            }
            catch (Exception ex)
            {
            }

            return strPath;
        } 
        #endregion

        #region Upload
        [WebMethod(EnableSession = true)]
        public string GetUploadToke(string loginToken, string FileName)
        {
            string UploaderToken = "";
            lock(this)
            {
                WFMUploadTask task = new WFMUploadTask();                
                 
                UserRegionInfo uri = (UserRegionInfo)Session[loginToken];

                if (uri != null)
                {
                    UploaderToken = System.Guid.NewGuid().ToString();
                    Session[UploaderToken] = FileName;
                    task.TaskGuid = UploaderToken;
                    task.UserName = uri.UserName;
                    task.FileName = FileName;
                    task.UserRegionPath = uri.RegionPathString;
                    task.FilePath = UploadRootPath + "\\" + uri.RegionPathString;
                    task.CreateTime = System.DateTime.Now;
                    task.OpTime = System.DateTime.Now;
                    task.Status = 0;
                    task.TStatus = 0;

                    WFMUploadTaskServices uts = new WFMUploadTaskServices();
                    var mes = uts.InsertData(task);

                    if (mes.retType != 100)
                        UploaderToken = ""; //insert task error

                   // Session[UploaderToken] = FileName;
                }
            }

            return UploaderToken;
        }


        /// <summary>
        /// Append a chunk of bytes to a file.
        /// The client should ensure that all messages are sent in sequence. 
        /// This method always overwrites any existing file with the same name
        /// </summary>
        /// <param name="FileName">The name of the file that this chunk belongs to, e.g. Vista.ISO</param>
        /// <param name="buffer">The byte array, i.e. the chunk being transferred</param>
        /// <param name="Offset">The offset at which to write the buffer to</param>
        [WebMethod(EnableSession = true)]
        public bool AppendChunk(string loginToken, string uploaderToken, byte[] buffer, long Offset)
        {             
            try {

                //string CurrentUser = HttpContext.Current.User.Identity.Name;
                //UserInfoHelper fh = (UserInfoHelper)HttpContext.Current.Session[CurrentUser];
                UserRegionInfo uri = (UserRegionInfo)Session[loginToken];
                if(uri == null)
                    CustomSoapException("上传标志获取失败AppendChunk()", "上传标志获取失败");

                //String FilePath = UploadRootPath + "\\" + fh.GenerateRegionPath(CurrentUser) + "\\"; //+Session[FileName].ToString();
                String FilePath = UploadRootPath + "\\" + uri.RegionPathString + "\\"; //+Session[FileName].ToString();
                MakeSureDirectory(FilePath);

                if(Session[uploaderToken] == null)
                    CustomSoapException("上传文件获取失败AppendChunk()", "上传文件获取失败");


                FilePath += Session[uploaderToken].ToString();


                if (Offset == 0)    // new file, create an empty file
                    File.Create(FilePath).Close();

                // open a file stream and write the buffer.  Don't open with FileMode.Append because the transfer may wish to start a different point
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    fs.Seek(Offset, SeekOrigin.Begin);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }catch(Exception ex)
            {
                CustomSoapException("写文件错误AppendChunk():", ex.Message);
                return false;
            }

            

            return true;
        }


        //[WebMethod]
        //public bool EndAppendChunk(string Token)
        //{
        //    bool bRet = false;
        //    WFMUploadTask task = new WFMUploadTask();

        //    task.TaskGuid = Token;
        //    task.OpTime = System.DateTime.Now;
        //    task.Status = 10;
        //    task.ErrorMsg = "";

        //    WFMUploadTaskServices uts = new WFMUploadTaskServices();
        //    var mes = uts.UpdateData(task);

        //    if (mes.retType == 100)
        //    { 
        //        bRet = true;
        //    }
        //    return bRet;
        //}

        [WebMethod(EnableSession = true)]
        public bool EndAppendChunk(string uploaderToken, int Status, String InfoMsg, String ErrorMsg)
        {
            bool bRet = false;
            if (Session[uploaderToken] == null)
                CustomSoapException("结束标志获取失败EndAppendChunk()", "结束标志获取失败");

            // WFMUploadTask task = new WFMUploadTask();

            WFMUploadTaskServices uts = new WFMUploadTaskServices();
            WFMUploadTask task = uts.GetDatabyGuid(uploaderToken);

            if (task == null)
                CustomSoapException("获取任务失败！", "获取任务失败");

            // task.TaskGuid = uploaderToken;
            task.OpTime = System.DateTime.Now;
            task.Status = Status;

            if (ErrorMsg != null)
                if (ErrorMsg.Length > 490)
                    task.ErrorMessage = ErrorMsg.Substring(0, 490);
                else task.ErrorMessage = ErrorMsg;
            task.ClientTaskName = "";
            task.ClientTaskComment = "";
            task.TStatus = 0;

            if (InfoMsg != null)
            {
                var info = InfoMsg.Split('$');
                if (info.Length > 0 && info[0] != null)
                    if (info[0].Length > 490)
                        task.ClientTaskName = info[0].Substring(0, 490);
                    else task.ClientTaskName = info[0];
                else if (info.Length > 1 && info[1] != null)
                    if (info[1].Length > 490)
                        task.ClientTaskComment = info[1].Substring(0, 498);
                    else task.ClientTaskName = info[1];
            }
            //WFMUploadTaskServices uts = new WFMUploadTaskServices();
            var mes = uts.UpdateData(task);

            if (mes.retType == 100)
            {
                bRet = true;
            }
            else
                CustomSoapException("更新任务失败！" + mes.Message, "更新任务失败！");

            return bRet;
        }
        #endregion

        #region Setting
        #region file hashing
        [WebMethod(EnableSession = true)]
        public string CheckFileHash(string loginToken, string UploadToken)
        {
            string hashString = "";
            try
            {
                UserRegionInfo uri = (UserRegionInfo)Session[loginToken];
                if (uri == null)
                    CustomSoapException("上传标志获取失败AppendChunk()", "上传标志获取失败");

                // String FilePath = UploadRootPath + "\\" + fh.GenerateRegionPath(CurrentUser) + "\\"; //+Session[FileName].ToString();     
                String FilePath = UploadRootPath + "\\" + uri.RegionPathString + "\\"; //+Session[FileName].ToString();                           
                FilePath += Session[UploadToken].ToString();

                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] hash;
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                {
                    hash = md5.ComputeHash(fs);
                    hashString = BitConverter.ToString(hash);
                }
            }
            catch (Exception ex)
            {
                CustomSoapException("校验数据生成错误！CheckFileHash():", ex.Message);
            }        
            
            return hashString;
        }

       
        #endregion

        /// <summary>
        /// The winforms client needs to know what is the max size of chunk that the server 
        /// will accept.  this is defined by MaxRequestLength, which can be overridden in
        /// web.config.
        /// </summary>
        [WebMethod]
        public long GetMaxRequestLength()
        {
            try
            {
                return (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection).MaxRequestLength;
            }
            catch (Exception ex)
            {
                CustomSoapException(ex.GetType().Name, ex.Message);
                return 4096;
            }
        }
        [WebMethod]
        public string UpdateInfo(string currentversion)
        {
            string strRet = "";
            double version = 0.0f;
            double oldversion = 0.0f;
            if (double.TryParse(ConfigurationManager.AppSettings["version"].ToString(), out version) &&
                double.TryParse(currentversion, out oldversion))
            {
                if (version > oldversion)
                    strRet = ConfigurationManager.AppSettings["UpDateInfo"].ToString();
            }
            else
            {
                strRet = "升级错误！";
            }
            return strRet;
        }
        #endregion

        #region Exception Handling
        /// <summary>
        /// Throws a soap exception.  It is formatted in a way that is more readable to the client, after being put through the xml serialisation process
        /// Typed exceptions don't work well across web services, so these exceptions are sent in such a way that the client
        /// can determine the 'name' or type of the exception thrown, and any message that went with it, appended after a : character.
        /// </summary>
        /// <param name="exceptionName"></param>
        /// <param name="message"></param>
        public static void CustomSoapException(string exceptionName, string message)
        {
            string logfile = ConfigurationManager.AppSettings["UploadPath"].ToString() + "\\log.txt";
            using (FileStream fs = new FileStream(
                   logfile, FileMode.Create | FileMode.Append, FileAccess.Write))
            {

                byte[] arrWriteData = Encoding.Default.GetBytes(message + "\r\n");
                fs.Write(arrWriteData, 0, arrWriteData.Length);

                fs.Close();
            }



            throw new System.Web.Services.Protocols.SoapException(exceptionName + ": " + message, new System.Xml.XmlQualifiedName("BufferedUpload"));
        }

        #endregion        

        #region public function

        protected static bool MakeSureDirectory(string dir)
        {
            bool bRet = true;
            try
            {
                if (!System.IO.Directory.Exists(dir))
                    MakeSureDirectoryPathExists(dir);
            }
            catch (Exception ex)
            {
                bRet = false;
                CustomSoapException("创建上传目录错误", ex.Message);
            }
            return bRet;
        }
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);
        #endregion

    }
}
