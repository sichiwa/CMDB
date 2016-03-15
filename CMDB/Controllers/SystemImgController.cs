using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CMDB.DAL;
using CMDB.Models;
using CMDB.ViewModels;
using CMDB.SystemClass;

namespace CMDB.Controllers
{
    public class SystemImgController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: SystmeImg
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getImgs()
        {
            //初始化系統參數
            Configer.Init();

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = ""; // Session["UserID"].ToString();
            SL.Controller = "Attribute";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                int SystmeImgCount = context.SystemImgs.Count();
                SL.EndTime = DateTime.Now;

                if (SystmeImgCount > 0)
                {
                    SL.TotalCount = SystmeImgCount;
                    SL.SuccessCount = SystmeImgCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = "取得系統圖片作業清單成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return PartialView("_getImgs", context.SystemImgs.ToList());
                }
                else
                {
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = "取得系統圖片清單作業成功，系統尚未建立圖片";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "尚未建立圖片";

                    return PartialView("_getImgs");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = "取得系統圖片作業清單失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                TempData["SystemInfo"] = "取得圖片異常";

                return PartialView("_getImgs");
            }
        }
    }
}