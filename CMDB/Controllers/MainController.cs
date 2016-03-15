using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMDB.DAL;
using CMDB.Models;
using CMDB.ViewModels;
using CMDB.SystemClass;

namespace CMDB.Controllers
{
    public class MainController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        // GET: Main/About
        public ActionResult About()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Main";
            SL.Action = "About";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                SL.EndTime = DateTime.Now;

                SystemInfo SI = new SystemInfo();

                SI.ProjectVersion = Configer.Version;
                SI.ProjectName = Configer.VersionName;
                SI.DevelopmentTools = "Visual Studio 2015 community update 1";
                SI.FrameworkVersion = "framework 4.6";
                SI.DevelopmentHistory = "0.2015/12/03 確認專案開發需求(系統組態由屬性及範本構成，當範本變動時須能動態反應至組態)\r\n";
                SI.DevelopmentHistory += "1.2015/12/02 建立專案\r\n";
                SI.DevelopmentHistory += "2.2015/12/15 完成屬性管理功能開發\r\n";
                SI.DevelopmentHistory += "3.2015/12/23 完成範本管理功能開發\r\n";
                SI.DevelopmentHistory += "4.2015/12/31 完成物件管理功能開發\r\n";
                SI.DevelopmentHistory += "5.2016/01/08 介面優化，支援範本建立/編輯時能夠拖拉屬性並決定顯示順序；增加頁面授權檢查機制，需有權限角色才能建立/編輯組態\r\n";
                SI.DevelopmentHistory += "6.2016/01/11 增加屬性、範本、物件清單顯示時檢查是否正常被編輯，如果正在被編輯則無法被下一位使用者編輯；物件編輯時將無法修改範本\r\n";
                SI.DevelopmentHistory += "7.2016/01/25 增加屬性、範本、物件新增編輯時，使用系統預設的雜湊演算法進行資料HASH計算並儲存，確保資料異動時不會被竄改\r\n";

                SL.TotalCount = 1;
                SL.SuccessCount = 1;
                SL.FailCount = 0;
                SL.Result = true;
                SL.Msg = Configer.GetAction + "系統資訊作業成功";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                TempData["SystemInfo"] = "OK";

                return View(SI);
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "系統資訊作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Login", "Account");
            }         
        }
    }
}