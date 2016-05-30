using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMDB.DAL;
using CMDB.Models;
using CMDB.ViewModels;
using CMDB.SystemClass;
using TWCAlib;
using LinqToExcel;

namespace CMDB.Controllers
{
    public class ImportCIController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: ImportCI
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string ImportType, HttpPostedFileBase file)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "Upload";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            JObject jo = new JObject();
            string result = string.Empty;

            SF.logandshowInfo("上傳檔案開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            if (file == null)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("上傳檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("上傳檔案失敗，錯誤訊息[無上傳檔案]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "上傳檔案失敗，錯誤訊息[無上傳檔案]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                jo.Add("Result", false);
                jo.Add("Msg", "錯誤訊息[無上傳檔案]");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }
            if (file.ContentLength <= 0)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("上傳檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("上傳檔案失敗，錯誤訊息[檔案大小為0]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "上傳檔案失敗，錯誤訊息[檔案大小為0]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                jo.Add("Result", false);
                jo.Add("Msg", "錯誤訊息[檔案大小為0]");

                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }

            string fileExtName = Path.GetExtension(file.FileName).ToLower();
            SF.logandshowInfo("上傳檔案名稱:" + file.FileName, log_Info);
            SF.logandshowInfo("上傳檔案副檔名:" + fileExtName, log_Info);

            if (!fileExtName.Equals(".xls", StringComparison.OrdinalIgnoreCase)
                &&
                !fileExtName.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("上傳檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("上傳檔案失敗，錯誤訊息[檔案格式錯誤]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "上傳檔案失敗，錯誤訊息[檔案格式錯誤]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                jo.Add("Result", false);
                jo.Add("Msg", "錯誤訊息[請上傳 .xls 或 .xlsx 格式的檔案]");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }

            try
            {
                var uploadResult = this.FileUploadHandler(file);
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("上傳檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("上傳檔案成功，檔案名稱[" + uploadResult.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 1;
                SL.FailCount = 0;
                SL.Result = true;
                SL.Msg = "上傳檔案成功，檔案名稱[" + uploadResult.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                //解析Excel
                string fileName =Server.MapPath(Configer.UploadPath + uploadResult.ToString());
                result =AnalyticsCIData(fileName, ImportType);

                List<string> Results = StringProcessor.SplitString2Array(result, Configer.SplitSymbol.ToCharArray());

                jo.Add("Result", Convert.ToBoolean(Results[0]));
                jo.Add("Msg", !string.IsNullOrWhiteSpace(Results[1]) ? Results[1] : "");

                result = JsonConvert.SerializeObject(jo);
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("上傳檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("上傳檔案失敗，錯誤訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "上傳檔案失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                jo.Add("Result", false);
                jo.Add("Msg", "錯誤訊息[" + ex.ToString() + "]");
                result = JsonConvert.SerializeObject(jo);
            }
            return Content(result, "application/json");
        }

        /// <summary>
        /// Files the upload handler.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">file;上傳失敗：沒有檔案！</exception>
        /// <exception cref="System.InvalidOperationException">上傳失敗：檔案沒有內容！</exception>
        private string FileUploadHandler(HttpPostedFileBase file)
        {
            //初始化系統參數
            Configer.Init();

            string result;

            if (file == null)
            {
                throw new ArgumentNullException("file", "上傳失敗：沒有檔案！");
            }
            if (file.ContentLength <= 0)
            {
                throw new InvalidOperationException("上傳失敗：檔案沒有內容！");
            }

            try
            {
                string virtualBaseFilePath = Url.Content(Configer.UploadPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string newFileName = string.Concat(
                    DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    Path.GetExtension(file.FileName).ToLower());

                string fullFilePath = Path.Combine(Server.MapPath(Configer.UploadPath), newFileName);
                file.SaveAs(fullFilePath);

                result = newFileName;
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 解析Excel資料並匯入資料庫
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ImportType"></param>
        /// <returns></returns>
        private string AnalyticsCIData(string fileName, string ImportType)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "AnalyticsCIData";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            string Result = string.Empty;

            SF.logandshowInfo("解析Excel檔案開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {
                Result = "解析Excel檔案失敗，錯誤訊息[找不到指定檔案:" + fileName + "]";
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("解析Excel檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[找不到指定檔案:" + fileName + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "解析Excel檔案失敗，錯誤訊息[找不到指定檔案:" + fileName + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return Result;
            }
            else
            {
                string ImportAttributesResult = string.Empty;
                List<string> TmpImportAttributesResultList;
                string ImportProfilesResult = string.Empty;
                List<string> TmpImportProfilesResult;
                string ImportObjectsResult = string.Empty;
                List<string> TmpImportObjectsResult;

                switch (ImportType)
                {
                    case "Init":
                        ImportAttributesResult = ImportAttributes(fileName);
                        TmpImportAttributesResultList = StringProcessor.SplitString2Array(ImportAttributesResult, Configer.SplitSymbol.ToCharArray());
                        ImportProfilesResult = ImportProfiles(fileName);
                        TmpImportProfilesResult = StringProcessor.SplitString2Array(ImportProfilesResult, Configer.SplitSymbol.ToCharArray());
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo("解析Excel檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                        if (Convert.ToBoolean(TmpImportAttributesResultList[0]) == true && Convert.ToBoolean(TmpImportProfilesResult[0]))
                        {
                            Result = "true" + Configer.SplitSymbol + "解析Excel檔案成功";
                            SF.logandshowInfo("解析Excel檔案成功", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Result = true;
                            SL.Msg = "解析Excel檔案成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + ";" + TmpImportProfilesResult[1].ToString() + "]";
                            SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息["+ TmpImportAttributesResultList [1].ToString()+ ";" + TmpImportProfilesResult[1].ToString() + "]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + ";" + TmpImportProfilesResult[1].ToString() + "]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }

                        break;
                    case "All":
                        ImportAttributesResult = ImportAttributes(fileName);
                        TmpImportAttributesResultList = StringProcessor.SplitString2Array(ImportAttributesResult, Configer.SplitSymbol.ToCharArray());
                        ImportProfilesResult = ImportProfiles(fileName);
                        TmpImportProfilesResult = StringProcessor.SplitString2Array(ImportProfilesResult, Configer.SplitSymbol.ToCharArray());
                        ImportObjectsResult = ImportObjects(fileName);
                        TmpImportObjectsResult = StringProcessor.SplitString2Array(ImportObjectsResult, Configer.SplitSymbol.ToCharArray());
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo("解析Excel檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                        if (Convert.ToBoolean(TmpImportAttributesResultList[0]) == true && Convert.ToBoolean(TmpImportProfilesResult[0])==true && Convert.ToBoolean(TmpImportObjectsResult[0]) == true)
                        {
                            Result = "true" + Configer.SplitSymbol + "解析Excel檔案成功";
                            SF.logandshowInfo("解析Excel檔案成功", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Result = true;
                            SL.Msg = "解析Excel檔案成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + ";" + TmpImportProfilesResult[1].ToString() + ";" + TmpImportObjectsResult[1].ToString() + "]";
                            SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + ";" + TmpImportProfilesResult[1].ToString() + ";"+ TmpImportObjectsResult [1].ToString()+ "]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + ";" + TmpImportProfilesResult[1].ToString() + ";" + TmpImportObjectsResult[1].ToString() + "]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        break;
                    case "Attribute":
                        ImportAttributesResult = ImportAttributes(fileName);
                        TmpImportAttributesResultList = StringProcessor.SplitString2Array(ImportAttributesResult, Configer.SplitSymbol.ToCharArray());
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo("解析Excel檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        if (Convert.ToBoolean(TmpImportAttributesResultList[0]) == true )
                        {
                            Result = "true" + Configer.SplitSymbol + "解析Excel檔案成功";
                            SF.logandshowInfo("解析Excel檔案成功", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Result = true;
                            SL.Msg = "解析Excel檔案成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + "]";
                            SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() + "]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = "解析Excel檔案失敗，錯誤訊息[" + TmpImportAttributesResultList[1].ToString() +"]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        break;
                    case "Profile":
                        ImportProfilesResult = ImportProfiles(fileName);
                        TmpImportProfilesResult = StringProcessor.SplitString2Array(ImportProfilesResult, Configer.SplitSymbol.ToCharArray());
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo("解析Excel檔案結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        if (Convert.ToBoolean(TmpImportProfilesResult[0]) == true)
                        {
                            Result = "true" + Configer.SplitSymbol + "解析Excel檔案成功";
                            SF.logandshowInfo("解析Excel檔案成功", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Result = true;
                            SL.Msg = "解析Excel檔案成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[" + TmpImportProfilesResult[1].ToString() + "]";
                            SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[" + TmpImportProfilesResult[1].ToString() + "]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = "解析Excel檔案失敗，錯誤訊息[" + TmpImportProfilesResult[1].ToString() + "]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        break;
                    case "Object":
                        ImportObjectsResult = ImportObjects(fileName);
                        SL.EndTime = DateTime.Now;
                        TmpImportObjectsResult = StringProcessor.SplitString2Array(ImportObjectsResult, Configer.SplitSymbol.ToCharArray());
                        if (Convert.ToBoolean(TmpImportObjectsResult[0]) == true)
                        {
                            Result = "true" + Configer.SplitSymbol + "解析Excel檔案成功";
                            SF.logandshowInfo("解析Excel檔案成功", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Result = true;
                            SL.Msg = "解析Excel檔案成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[" + TmpImportObjectsResult[1].ToString() + "]";
                            SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[" + TmpImportObjectsResult[1].ToString() + "]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = "解析Excel檔案失敗，錯誤訊息[" + TmpImportObjectsResult[1].ToString() + "]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        break;
                    default:
                        Result = "false" + Configer.SplitSymbol + "解析Excel檔案失敗，錯誤訊息[無此匯入類型:" + ImportType + "]";
                        SF.logandshowInfo("解析Excel檔案失敗，錯誤訊息[無此匯入類型:" + ImportType + "]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = "解析Excel檔案失敗，錯誤訊息[無此匯入類型:"+ ImportType + "]";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        break;
                }

                return Result;
            }
        }

        /// <summary>
        /// 匯入屬性資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ImportAttributes(string fileName)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "ImportAttributes";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            var excelFile = new ExcelQueryFactory(fileName);
            string Result = "";

            try
            {
                SF.logandshowInfo("匯入屬性資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                IEnumerable<string> sheetsName = excelFile.GetWorksheetNames();

                if (! sheetsName.Contains("Attributes"))
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入屬性資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo("匯入屬性資料失敗，錯誤訊息[找不到指定Sheet:Attrubutes]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = "匯入屬性資料失敗，錯誤訊息[找不到指定Sheet:Attributes]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    Result = "false" + Configer.SplitSymbol + "匯入屬性資料失敗，錯誤訊息[找不到指定Sheet:Attributes]";

                    return Result;
                }
                else
                {
                    //getSheet
                   var excelContent = excelFile.Worksheet<CI_Attributes>("Attributes");
                    //var excelContent = excelFile.Worksheet("Attributes");

                    //欄位對映
                    excelFile.AddMapping<CI_Attributes>(x => x.AttributeID, "AttributeID");
                    excelFile.AddMapping<CI_Attributes>(x => x.AttributeName, "AttributeName");
                    excelFile.AddMapping<CI_Attributes>(x => x.AttributeTypeID, "AttributeTypeID");
                    excelFile.AddMapping<CI_Attributes>(x => x.DropDownValues, "DropDownValues");
                    excelFile.AddMapping<CI_Attributes>(x => x.Description, "Description");

                    SL.TotalCount = excelContent.Count();
                    SF.logandshowInfo("本次共需匯入[" + SL.TotalCount.ToString() + "]筆資料", log_Info);

                    int i = 1;
                    foreach (var r in excelContent)
                    {
                        var Attr = new CI_Attributes();

                        if (r.AttributeID <= 0)
                        {
                            SL.FailCount += 1;
                            SF.logandshowInfo("匯入屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeID格式錯誤，應該大於0]", log_Info);
                        }
                        else
                        {
                            int RepCount = context.CI_Attributes.Where(b => b.AttributeID == r.AttributeID).Count();
                            //判斷是否重複
                            if (RepCount > 0)
                            {
                                SL.FailCount += 1;
                                SF.logandshowInfo("匯入屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料重複]", log_Info);
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(r.AttributeName))
                                {
                                    SL.FailCount += 1;
                                    SF.logandshowInfo("匯入屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeName為空白]", log_Info);
                                }
                                else
                                {
                                    if (r.AttributeTypeID <= 0)
                                    {
                                        SL.FailCount += 1;
                                        SF.logandshowInfo("匯入屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeTypeID格式錯誤，應該大於0]", log_Info);
                                    }
                                    else
                                    {
                                        //通過檢查
                                        StringBuilder PlainText = new StringBuilder();

                                        Attr.AttributeID = r.AttributeID;
                                        Attr.AttributeName = r.AttributeName;
                                        PlainText.Append(Attr.AttributeName + Configer.SplitSymbol);

                                        Attr.Description = r.Description;
                                        PlainText.Append(Attr.Description + Configer.SplitSymbol);

                                        Attr.AttributeTypeID = r.AttributeTypeID;
                                        PlainText.Append(Attr.AttributeTypeID.ToString() + Configer.SplitSymbol);

                                        Attr.DropDownValues = r.DropDownValues;
                                        PlainText.Append(Attr.DropDownValues + Configer.SplitSymbol);

                                        Attr.CreateAccount = nowUser;
                                        PlainText.Append(Attr.CreateAccount + Configer.SplitSymbol);

                                        Attr.CreateTime = DateTime.Now;
                                        PlainText.Append(Attr.CreateTime.ToString() + Configer.SplitSymbol);

                                        Attr.UpdateAccount = nowUser;
                                        PlainText.Append(Attr.UpdateAccount + Configer.SplitSymbol);

                                        Attr.UpdateTime = DateTime.Now;
                                        PlainText.Append(Attr.UpdateTime.ToString());

                                        //計算HASH值
                                        SF.logandshowInfo("匯入屬性子程序-計算第[" + i.ToString() + "]筆屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        Attr.HashValue = SF.getHashValue(PlainText.ToString());
                                        SF.logandshowInfo("匯入屬性子程序-計算第[" + i.ToString() + "]筆屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo("匯入屬性子程序-計算第[" + i.ToString() + "]筆屬性HASH值結果:明文:[" + PlainText.ToString() + "];HASH[" + Attr.HashValue + "]", log_Info);

                                        //新增屬性
                                        SF.logandshowInfo("匯入屬性子程序-儲存第[" + i.ToString() + "]筆屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Attributes.Add(Attr);
                                        context.SaveChanges();
                                        SF.logandshowInfo("匯入屬性子程序-儲存第[" + i.ToString() + "]筆屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SL.SuccessCount += 1;
                                    }
                                }
                            }
                        }

                        i += 1;
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (SL.FailCount == 0)
                    {
                        SL.Result = true;
                        SL.Msg = "匯入屬性完成，本次作業全部成功";
                        SF.logandshowInfo("匯入屬性完成，本次作業全部成功", log_Info);
                        Result = "true" + Configer.SplitSymbol + "匯入屬性完成，本次作業全部成功";
                    }
                    else
                    {
                        SL.Result = false;
                        SL.Msg = "匯入屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                        SF.logandshowInfo("匯入屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗", log_Info);
                        Result = "false" + Configer.SplitSymbol + "匯入屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    }
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("匯入屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                //SL.TotalCount = 1;
                //SL.SuccessCount = 0;
                //SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "匯入屬性失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Result = "false" + Configer.SplitSymbol + "匯入屬性失敗，錯誤訊息[" + ex.ToString() + "]";

                return Result;
            }
        }

        /// <summary>
        /// 匯入範本資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ImportProfiles(string fileName)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "ImportProfiles";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            var excelFile = new ExcelQueryFactory(fileName);
            string Result = "";

            try
            {
                SF.logandshowInfo("匯入範本資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                IEnumerable<string> sheetsName = excelFile.GetWorksheetNames();

                if (!sheetsName.Contains("Profiles"))
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入範本資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo("匯入範本資料失敗，錯誤訊息[找不到指定Sheet:Profiles]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = "匯入範本資料失敗，錯誤訊息[找不到指定Sheet:Profiles]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    Result = "false" + Configer.SplitSymbol + "匯入範本資料失敗，錯誤訊息[找不到指定Sheet:Profiles]";

                    return Result;
                }
                else
                {
                    //SheetName
                    var excelContent = excelFile.Worksheet<CI_Proflies>("Profiles");

                    //欄位對映
                    excelFile.AddMapping<CI_Proflies>(x => x.ProfileID, "ProfileID");
                    excelFile.AddMapping<CI_Proflies>(x => x.ProfileName, "ProfileName");
                    excelFile.AddMapping<CI_Proflies>(x => x.ImgID, "ImgID");
                    excelFile.AddMapping<CI_Proflies>(x => x.Description, "Description");

                    SL.TotalCount = excelContent.Count();
                    SF.logandshowInfo("本次共需匯入[" + SL.TotalCount.ToString() + "]筆資料", log_Info);

                    int i = 1;
                    foreach (var r in excelContent)
                    {
                        var Pro = new CI_Proflies();

                        if (r.ProfileID <= 0)
                        {
                            SL.FailCount += 1;
                            SF.logandshowInfo("匯入範本資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ProfileID格式錯誤，應該大於0]", log_Info);
                        }
                        else
                        {
                            int RepCount = context.CI_Proflies.Where(b => b.ProfileID == r.ProfileID).Count();
                            //判斷是否重複
                            if (RepCount > 0)
                            {
                                SL.FailCount += 1;
                                SF.logandshowInfo("匯入範本資料失敗，錯誤訊息[第" + i.ToString() + "筆資料重複]", log_Info);
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(r.ProfileName))
                                {
                                    SL.FailCount += 1;
                                    SF.logandshowInfo("匯入範本資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ProfileName為空白]", log_Info);
                                }
                                else
                                {
                                    if (r.ImgID <= 0)
                                    {
                                        SL.FailCount += 1;
                                        SF.logandshowInfo("匯入範本資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ImgID格式錯誤，應該大於0]", log_Info);
                                    }
                                    else
                                    {
                                        //通過檢查
                                        StringBuilder PlainText = new StringBuilder();

                                        Pro.ProfileID = r.ProfileID;
                                        Pro.ProfileName = r.ProfileName;
                                        PlainText.Append(Pro.ProfileName + Configer.SplitSymbol);

                                        Pro.ImgID = r.ImgID;
                                        PlainText.Append(Pro.ImgID.ToString() + Configer.SplitSymbol);

                                        Pro.Description = r.Description;
                                        PlainText.Append(Pro.Description + Configer.SplitSymbol);

                                        Pro.CreateAccount = nowUser;
                                        PlainText.Append(Pro.CreateAccount + Configer.SplitSymbol);

                                        Pro.CreateTime = DateTime.Now;
                                        PlainText.Append(Pro.CreateTime.ToString() + Configer.SplitSymbol);

                                        Pro.UpdateAccount = nowUser;
                                        PlainText.Append(Pro.UpdateAccount + Configer.SplitSymbol);

                                        Pro.UpdateTime = DateTime.Now;
                                        PlainText.Append(Pro.UpdateTime.ToString());

                                        //計算HASH值
                                        SF.logandshowInfo("匯入範本子程序-計算第[" + i.ToString() + "]筆屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        Pro.HashValue = SF.getHashValue(PlainText.ToString());
                                        SF.logandshowInfo("匯入範本子程序-計算第[" + i.ToString() + "]筆屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo("匯入範本子程序-計算第[" + i.ToString() + "]筆屬性HASH值結果:明文:[" + PlainText.ToString() + "];HASH[" + Pro.HashValue + "]", log_Info);

                                        //新增屬性
                                        SF.logandshowInfo("匯入範本子程序-儲存第[" + i.ToString() + "]筆範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Proflies.Add(Pro);
                                        context.SaveChanges();
                                        SF.logandshowInfo("匯入範本子程序-儲存第[" + i.ToString() + "]筆範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SL.SuccessCount += 1;
                                    }
                                }
                            }
                        }

                        i += 1;
                    }
                  
                    //if (SL.SuccessCount == 0)
                    //{
                    //    SL.Result = true;
                    //    SL.Msg = "匯入範本完成，本次作業全部成功";
                    //    SF.logandshowInfo("匯入範本完成，本次作業全部成功", log_Info);
                    //    //Result = "true" + Configer.SplitSymbol + "匯入範本完成，本次作業全部成功";
                    //}
                    //else
                    //{
                    //    SL.Result = false;
                    //    SL.Msg = "匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    //    SF.logandshowInfo("匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗", log_Info);
                    //    //Result = "false" + Configer.SplitSymbol + "匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    //}
                    //SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    //return Result;

                    //匯入範本屬性
                    string TmpResult = ImportProfileAttributes(fileName);

                    List<string> ResultList = StringProcessor.SplitString2Array(TmpResult, Configer.SplitSymbol.ToCharArray());

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入範本資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (Convert.ToBoolean(ResultList[0]) == true && SL.Result == true)
                    {
                        Result = "true" + Configer.SplitSymbol + "匯入範本完成，本次作業全部成功";
                        SL.Result = true;
                        SL.Msg = Result;
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    }
                    else
                    {
                        if (Convert.ToBoolean(ResultList[0]) == true && SL.Result == false)
                        {
                            Result = "false" + Configer.SplitSymbol + "匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                            SL.Result = false;
                            SL.Msg = Result;
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            if (Convert.ToBoolean(ResultList[0]) == false && SL.Result == true)
                            {
                                Result = "false" + Configer.SplitSymbol + "匯入範本完成，本次作業全部成功。但匯入範本屬性時發生失敗";
                                SL.Result = false;
                                SL.Msg = Result;
                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                            }
                            else
                            {
                                if (Convert.ToBoolean(ResultList[0]) == false && SL.Result == false)
                                {
                                    Result = "false" + Configer.SplitSymbol + "匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗。且匯入範本屬性時發生失敗";
                                    SL.Result = false;
                                    SL.Msg = Result;
                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                                }
                            }
                        }
                    }

                    return Result;
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("匯入範本資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                //SL.TotalCount = 1;
                //SL.SuccessCount = 0;
                //SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "匯入範本失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Result = "false" + Configer.SplitSymbol + "匯入範本失敗，錯誤訊息[" + ex.ToString() + "]";

                return Result;
            }
        }

        /// <summary>
        /// 匯入範本屬性資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ImportProfileAttributes(string fileName)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "ImportProfileAttributes";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            var excelFile = new ExcelQueryFactory(fileName);
            string Result = "";

            try
            {
                SF.logandshowInfo("匯入範本屬性資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                IEnumerable<string> sheetsName = excelFile.GetWorksheetNames();

                if (!sheetsName.Contains("ProfileAttributes"))
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入範本屬性資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[找不到指定Sheet:ProfileAttributes]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = "匯入範本屬性資料失敗，錯誤訊息[找不到指定Sheet:ProfileAttributes]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    Result = "false" + Configer.SplitSymbol + "匯入範本屬性資料失敗，錯誤訊息[找不到指定Sheet:ProfileAttributes]";

                    return Result;
                }
                else
                {
                    //getSheet
                    var excelContent = excelFile.Worksheet<CI_Proflie_Attributes>("ProfileAttributes");

                    //欄位對映
                    excelFile.AddMapping<CI_Proflie_Attributes>(x => x.ProfileID, "ProfileID");
                    excelFile.AddMapping<CI_Proflie_Attributes>(x => x.AttributeID, "AttributeID");
                    excelFile.AddMapping<CI_Proflie_Attributes>(x => x.AttributeOrder, "AttributeOrder");

                    SL.TotalCount = excelContent.Count();
                    SF.logandshowInfo("本次共需匯入[" + SL.TotalCount.ToString() + "]筆資料", log_Info);

                    int i = 1;
                    foreach (var r in excelContent)
                    {
                        var ProAttrs = new CI_Proflie_Attributes();

                        if (r.ProfileID <= 0)
                        {
                            SL.FailCount += 1;
                            SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ProfileID格式錯誤，應該大於0]", log_Info);
                        }
                        else
                        {
                            int RepCount = context.CI_Proflies.Where(b => b.ProfileID == r.ProfileID).Count();
                            //判斷系統是否已經有此範本
                            if (RepCount <= 0)
                            {
                                SL.FailCount += 1;
                                SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料錯誤，系統無此範本，ProfileID:" + r.ProfileID.ToString() + "]", log_Info);
                            }
                            else
                            {
                                if (r.AttributeID <= 0)
                                {
                                    SL.FailCount += 1;
                                    SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeID格式錯誤，應該大於0]", log_Info);
                                }
                                else
                                {
                                    int RepCount1 = context.CI_Attributes.Where(b => b.AttributeID == r.AttributeID).Count();
                                    if (RepCount1 <= 0)
                                    {
                                        SL.FailCount += 1;
                                        SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料錯誤，系統無此屬性，ProfileID:" + r.ProfileID.ToString() + "]", log_Info);
                                    }
                                    else
                                    {
                                        //判斷範本是否已經有此屬性
                                        int RepCount2 = context.CI_Proflie_Attributes.Where(b => b.ProfileID == r.ProfileID).Where(b => b.AttributeID == r.AttributeID).Count();
                                        if (RepCount2 > 0)
                                        {
                                            SL.FailCount += 1;
                                            SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料重複]", log_Info);
                                        }
                                        else
                                        {
                                            if (r.AttributeOrder <= 0)
                                            {
                                                SL.FailCount += 1;
                                                SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeOrder格式錯誤，應該大於0]", log_Info);
                                            }
                                            else
                                            {
                                                //通過檢查
                                                StringBuilder PlainText = new StringBuilder();

                                                ProAttrs.ProfileID = r.ProfileID;
                                                PlainText.Append(ProAttrs.ProfileID.ToString() + Configer.SplitSymbol);

                                                ProAttrs.AttributeID = r.AttributeID;
                                                PlainText.Append(ProAttrs.AttributeID.ToString() + Configer.SplitSymbol);

                                                ProAttrs.AttributeOrder = r.AttributeOrder;
                                                PlainText.Append(ProAttrs.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                ProAttrs.CreateAccount = nowUser;
                                                PlainText.Append(ProAttrs.CreateAccount + Configer.SplitSymbol);

                                                ProAttrs.CreateTime = DateTime.Now;
                                                PlainText.Append(ProAttrs.CreateTime.ToString() + Configer.SplitSymbol);

                                                ProAttrs.UpdateAccount = nowUser;
                                                PlainText.Append(ProAttrs.UpdateAccount + Configer.SplitSymbol);

                                                ProAttrs.UpdateTime = DateTime.Now;
                                                PlainText.Append(ProAttrs.UpdateTime.ToString());

                                                //計算HASH值
                                                SF.logandshowInfo("匯入範本屬性子程序-計算第[" + i.ToString() + "]筆範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                ProAttrs.HashValue = SF.getHashValue(PlainText.ToString());
                                                SF.logandshowInfo("匯入範本屬性子程序-計算第[" + i.ToString() + "]筆範本屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo("匯入範本屬性子程序-計算第[" + i.ToString() + "]筆範本屬性HASH值結果:明文:[" + PlainText.ToString() + "];HASH[" + ProAttrs.HashValue + "]", log_Info);

                                                //新增屬性
                                                SF.logandshowInfo("匯入範本屬性子程序-儲存第[" + i.ToString() + "]筆範本屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                context.CI_Proflie_Attributes.Add(ProAttrs);
                                                context.SaveChanges();
                                                SF.logandshowInfo("匯入範本屬性子程序-儲存第[" + i.ToString() + "]筆範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SL.SuccessCount += 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        i += 1;
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入範本屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (SL.FailCount == 0)
                    {
                        SL.Result = true;
                        SL.Msg = "匯入範本屬性完成，本次作業全部成功";
                        SF.logandshowInfo("匯入範本屬性完成，本次作業全部成功", log_Info);
                        Result = "true" + Configer.SplitSymbol + "匯入範本屬性完成，本次作業全部成功";
                    }
                    else
                    {
                        SL.Result = false;
                        SL.Msg = "匯入範本屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                        SF.logandshowInfo("匯入範本屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗", log_Info);
                        Result = "false" + Configer.SplitSymbol + "匯入範本屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    }
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return Result;

                }

            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("匯入範本屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                //SL.TotalCount = 1;
                //SL.SuccessCount = 0;
                //SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "匯入範本屬性失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Result = "false" + Configer.SplitSymbol + "匯入範本屬性失敗，錯誤訊息[" + ex.ToString() + "]";

                return Result;
            }
        }

        /// <summary>
        /// 匯入物件資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ImportObjects(string fileName)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "ImportObjects";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            var excelFile = new ExcelQueryFactory(fileName);
            string Result = "";

            try
            {
                SF.logandshowInfo("匯入物件資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                IEnumerable<string> sheetsName = excelFile.GetWorksheetNames();

                if (!sheetsName.Contains("Objects"))
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入物件資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo("匯入物件資料失敗，錯誤訊息[找不到指定Sheet:Objects]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = "匯入物件資料失敗，錯誤訊息[找不到指定Sheet:Objects]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    Result = "false" + Configer.SplitSymbol + "匯入物件資料失敗，錯誤訊息[找不到指定Sheet:Objects]";

                    return Result;
                }
                else
                {
                    //getSheet
                    var excelContent = excelFile.Worksheet<CI_Objects>("Objects");

                    //欄位對映
                    excelFile.AddMapping<CI_Objects>(x => x.ObjectID, "ObjectID");
                    excelFile.AddMapping<CI_Objects>(x => x.ObjectName, "ObjectName");
                    excelFile.AddMapping<CI_Objects>(x => x.ProfileID, "ProfileID");
                    excelFile.AddMapping<CI_Objects>(x => x.Description, "Description");

                    SL.TotalCount = excelContent.Count();
                    SF.logandshowInfo("本次共需匯入[" + SL.TotalCount.ToString() + "]筆資料", log_Info);

                    int i = 1;
                    foreach (var r in excelContent)
                    {
                        var Obj = new CI_Objects();

                        if (r.ObjectID <= 0)
                        {
                            SL.FailCount += 1;
                            SF.logandshowInfo("匯入物件資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ObjectID格式錯誤，應該大於0]", log_Info);
                        }
                        else
                        {
                            int RepCount = context.CI_Objects.Where(b => b.ObjectID == r.ObjectID).Count();
                            //判斷是否重複
                            if (RepCount > 0)
                            {
                                SL.FailCount += 1;
                                SF.logandshowInfo("匯入物件資料失敗，錯誤訊息[第" + i.ToString() + "筆資料重複]", log_Info);
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(r.ObjectName))
                                {
                                    SL.FailCount += 1;
                                    SF.logandshowInfo("匯入物件資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ObjectName為空白]", log_Info);
                                }
                                else
                                {
                                    int RepCount1= context.CI_Proflies.Where(b => b.ProfileID == r.ProfileID).Count();
                                    if (RepCount1 <= 0)
                                    {
                                        SL.FailCount += 1;
                                        SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料錯誤，系統無此範本，ProfileID:" + r.ProfileID.ToString() + "]", log_Info);
                                    }
                                    else
                                    {
                                        //通過檢查
                                        StringBuilder PlainText = new StringBuilder();

                                        Obj.ObjectID = r.ObjectID;

                                        Obj.ProfileID = r.ProfileID;
                                        PlainText.Append(Obj.ProfileID.ToString() + Configer.SplitSymbol);

                                        Obj.ObjectName = r.ObjectName;
                                        PlainText.Append(Obj.ObjectName + Configer.SplitSymbol);

                                        Obj.Description = r.Description;
                                        PlainText.Append(Obj.Description + Configer.SplitSymbol);

                                        Obj.ProfileID = r.ProfileID;
                                        PlainText.Append(Obj.ProfileID.ToString() + Configer.SplitSymbol);

                                        Obj.CreateAccount = nowUser;
                                        PlainText.Append(Obj.CreateAccount + Configer.SplitSymbol);

                                        Obj.CreateTime = DateTime.Now;
                                        PlainText.Append(Obj.CreateTime.ToString() + Configer.SplitSymbol);

                                        Obj.UpdateAccount = nowUser;
                                        PlainText.Append(Obj.UpdateAccount + Configer.SplitSymbol);

                                        Obj.UpdateTime = DateTime.Now;
                                        PlainText.Append(Obj.UpdateTime.ToString());

                                        //計算HASH值
                                        SF.logandshowInfo("匯入物件子程序-計算第[" + i.ToString() + "]筆物件HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        Obj.HashValue = SF.getHashValue(PlainText.ToString());
                                        SF.logandshowInfo("匯入物件子程序-計算第[" + i.ToString() + "]筆物件HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo("匯入物件子程序-計算第[" + i.ToString() + "]筆物件HASH值結果:明文:[" + PlainText.ToString() + "];HASH[" + Obj.HashValue + "]", log_Info);

                                        //新增屬性
                                        SF.logandshowInfo("匯入物件子程序-儲存第[" + i.ToString() + "]筆物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Objects.Add(Obj);
                                        context.SaveChanges();
                                        SF.logandshowInfo("匯入物件子程序-儲存第[" + i.ToString() + "]筆物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SL.SuccessCount += 1;
                                    }
                                }
                            }
                        }

                        i += 1;
                    }
                 

                    //if (SL.FailCount == 0)
                    //{
                    //    SL.Result = true;
                    //    SL.Msg = "匯入物件完成，本次作業全部成功";
                    //    SF.logandshowInfo("匯入物件完成，本次作業全部成功", log_Info);
                    //    //Result = "true" + Configer.SplitSymbol + "匯入範本完成，本次作業全部成功";
                    //}
                    //else
                    //{
                    //    SL.Result = false;
                    //    SL.Msg = "匯入物件完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    //    SF.logandshowInfo("匯入物件完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗", log_Info);
                    //    //Result = "false" + Configer.SplitSymbol + "匯入範本完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    //}

                    //匯入物件屬性
                    string TmpResult = ImportObjectAttributes(fileName);

                    List<string> ResultList = StringProcessor.SplitString2Array(TmpResult, Configer.SplitSymbol.ToCharArray());

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入物件資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (Convert.ToBoolean(ResultList[0]) == true && SL.Result == true)
                    {
                        Result = "true" + Configer.SplitSymbol + "匯入物件完成，本次作業全部成功";
                        SL.Result = true;
                        SL.Msg = Result;
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    }
                    else
                    {
                        if (Convert.ToBoolean(ResultList[0]) == true && SL.Result == false)
                        {
                            Result = "false" + Configer.SplitSymbol + "匯入物件完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                            SL.Result = false;
                            SL.Msg = Result;
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        }
                        else
                        {
                            if (Convert.ToBoolean(ResultList[0]) == false && SL.Result == true)
                            {
                                Result = "false" + Configer.SplitSymbol + "匯入物件完成，本次作業全部成功。但匯入物件屬性時發生失敗";
                                SL.Result = false;
                                SL.Msg = Result;
                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                            }
                            else
                            {
                                if (Convert.ToBoolean(ResultList[0]) == false && SL.Result == false)
                                {
                                    Result = "false" + Configer.SplitSymbol + "匯入物件完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗。且匯入物件屬性時發生失敗";
                                    SL.Result = false;
                                    SL.Msg = Result;
                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                                }
                            }
                        }
                    }

                    return Result;
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("匯入物件資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                //SL.TotalCount = 1;
                //SL.SuccessCount = 0;
                //SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "匯入物件失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Result = "false" + Configer.SplitSymbol + "匯入物件失敗，錯誤訊息[" + ex.ToString() + "]";

                return Result;
            }
        }

        /// <summary>
        /// 匯入物件屬性資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ImportObjectAttributes(string fileName)
        {
            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            //int nowFunction = 15;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ImportCIController";
            SL.Action = "ImportObjectAttributes";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            var excelFile = new ExcelQueryFactory(fileName);
            string Result = "";

            try
            {
                SF.logandshowInfo("匯入物件屬性資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                IEnumerable<string> sheetsName = excelFile.GetWorksheetNames();

                if (!sheetsName.Contains("ObjectAttributes"))
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入物件屬性資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo("匯入物件屬性資料失敗，錯誤訊息[找不到指定Sheet:ObjectAttributes]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = "匯入物件屬性資料失敗，錯誤訊息[找不到指定Sheet:ObjectAttributes]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                    Result = "false" + Configer.SplitSymbol + "匯入物件屬性資料失敗，錯誤訊息[找不到指定Sheet:ObjectAttributes]";

                    return Result;
                }
                else
                {
                    //getSheet
                    var excelContent = excelFile.Worksheet<CI_Object_Data>("ObjectAttributes");

                    //欄位對映
                    excelFile.AddMapping<CI_Object_Data>(x => x.ObjectID, "ObjectID");
                    excelFile.AddMapping<CI_Object_Data>(x => x.AttributeID, "AttributeID");
                    excelFile.AddMapping<CI_Object_Data>(x => x.AttributeValue, "AttributeValue");
                    excelFile.AddMapping<CI_Object_Data>(x => x.AttributeOrder, "AttributeOrder");

                    SL.TotalCount = excelContent.Count();
                    SF.logandshowInfo("本次共需匯入[" + SL.TotalCount.ToString() + "]筆資料", log_Info);

                    int i = 1;
                    foreach (var r in excelContent)
                    {
                        var ObjAttrs = new CI_Object_Data();

                        if (r.ObjectID <= 0)
                        {
                            SL.FailCount += 1;
                            SF.logandshowInfo("匯入物件屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料ObjectID格式錯誤，應該大於0]", log_Info);
                        }
                        else
                        {
                            int RepCount = context.CI_Objects.Where(b => b.ObjectID == r.ObjectID).Count();
                            //判斷系統是否已經有此物件
                            if (RepCount <= 0)
                            {
                                SL.FailCount += 1;
                                SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料錯誤，系統無此物件，ObjectID:" + r.ObjectID.ToString() + "]", log_Info);
                            }
                            else
                            {
                                if (r.AttributeID <= 0)
                                {
                                    SL.FailCount += 1;
                                    SF.logandshowInfo("匯入物件屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料AttributeID格式錯誤，應該大於0]", log_Info);
                                }
                                else
                                {
                                    int RepCount1 = context.CI_Attributes.Where(b => b.AttributeID == r.AttributeID).Count();
                                    if (RepCount1 <= 0)
                                    {
                                        SL.FailCount += 1;
                                        SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料錯誤，系統無此屬性，AttributeID:" + r.AttributeID.ToString() + "]", log_Info);
                                    }
                                    else
                                    {
                                        int RepCount2 = context.CI_Object_Data.Where(b => b.ObjectID == r.ObjectID).Where(b => b.AttributeID == r.AttributeID).Where(b=>b.AttributeValue==r.AttributeValue).Count();
                                        if (RepCount2 > 0)
                                        {
                                            SL.FailCount += 1;
                                            SF.logandshowInfo("匯入範本屬性資料失敗，錯誤訊息[第" + i.ToString() + "筆資料重複]", log_Info);
                                        }
                                        else
                                        {
                                            //通過檢查
                                            StringBuilder PlainText = new StringBuilder();

                                            ObjAttrs.ObjectID = r.ObjectID;
                                            PlainText.Append(ObjAttrs.ObjectID.ToString() + Configer.SplitSymbol);

                                            ObjAttrs.AttributeID = r.AttributeID;
                                            PlainText.Append(ObjAttrs.AttributeID.ToString() + Configer.SplitSymbol);

                                            if (string.IsNullOrEmpty(r.AttributeValue))
                                            {
                                                r.AttributeValue = "無";
                                            }

                                            ObjAttrs.AttributeValue = r.AttributeValue;
                                            PlainText.Append(ObjAttrs.AttributeValue + Configer.SplitSymbol);

                                            ObjAttrs.AttributeOrder = r.AttributeOrder;
                                            PlainText.Append(ObjAttrs.AttributeOrder.ToString() + Configer.SplitSymbol);

                                            ObjAttrs.CreateAccount = nowUser;
                                            PlainText.Append(ObjAttrs.CreateAccount + Configer.SplitSymbol);

                                            ObjAttrs.CreateTime = DateTime.Now;
                                            PlainText.Append(ObjAttrs.CreateTime.ToString() + Configer.SplitSymbol);

                                            ObjAttrs.UpdateAccount = nowUser;
                                            PlainText.Append(ObjAttrs.UpdateAccount + Configer.SplitSymbol);

                                            ObjAttrs.UpdateTime = DateTime.Now;
                                            PlainText.Append(ObjAttrs.UpdateTime.ToString());

                                            //計算HASH值
                                            SF.logandshowInfo("匯入物件屬性子程序-計算第[" + i.ToString() + "]筆物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            ObjAttrs.HashValue = SF.getHashValue(PlainText.ToString());
                                            SF.logandshowInfo("匯入物件屬性子程序-計算第[" + i.ToString() + "]筆物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo("匯入物件屬性子程序-計算第[" + i.ToString() + "]筆物件屬性HASH值結果:明文:[" + PlainText.ToString() + "];HASH[" + ObjAttrs.HashValue + "]", log_Info);

                                            //新增屬性
                                            SF.logandshowInfo("匯入物件屬性子程序-儲存第[" + i.ToString() + "]筆物件屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            context.CI_Object_Data.Add(ObjAttrs);
                                            context.SaveChanges();
                                            SF.logandshowInfo("匯入物件屬性子程序-儲存第[" + i.ToString() + "]筆物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SL.SuccessCount += 1;
                                        }
                                    }
                                }
                            }
                        }
                        i += 1;
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo("匯入物件屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (SL.FailCount == 0)
                    {
                        SL.Result = true;
                        SL.Msg = "匯入物件屬性完成，本次作業全部成功";
                        SF.logandshowInfo("匯入物件屬性完成，本次作業全部成功", log_Info);
                        Result = "true" + Configer.SplitSymbol + "匯入物件屬性完成，本次作業全部成功";
                    }
                    else
                    {
                        SL.Result = false;
                        SL.Msg = "匯入物件屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                        SF.logandshowInfo("匯入物件屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗", log_Info);
                        Result = "false" + Configer.SplitSymbol + "匯入物件屬性完成，本次作業有[" + SL.SuccessCount.ToString() + "]筆成功；[" + SL.FailCount.ToString() + "]筆失敗";
                    }
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return Result;
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo("匯入物件屬性資料結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                //SL.TotalCount = 1;
                //SL.SuccessCount = 0;
                //SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = "匯入物件屬性失敗，錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Result = "false" + Configer.SplitSymbol + "匯入物件屬性失敗，錯誤訊息[" + ex.ToString() + "]";

                return Result;
            }
        }
    }
}