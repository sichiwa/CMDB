using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMDB.DAL;
using CMDB.Models;
using CMDB.SystemClass;
using TWCAlib;

namespace CMDB.Controllers
{
    public class AccountController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginInfo model)
        {
            //初始化系統參數
            Configer.Init();

            AD AD = new AD();
            VA VA = new VA();
            LoginProcessor LP = new LoginProcessor();
            bool UseCertLogin = false;
            string LDAPName = Configer.LDAPName;
            //string VAVerifyURL = WebConfigurationManager.AppSettings["VAVerifyURL"];
            //string ConnStr = Configer.C_DBConnstring;
            bool ContinueLogin = true;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = model.UserID;
            SL.Controller = "Account";
            SL.Action = "Login";
            SL.StartTime = DateTime.Now;
            SL.TotalCount = 1;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;
            //string SendResult = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    if (LDAPName == "")
                    {
                        //缺少系統參數，需記錄錯誤
                        SL.EndTime = DateTime.Now;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Msg = Configer.LoginAction + "作業失敗，錯誤訊息:[缺少系統參數LDAPName]";
                        SL.Result = false;
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                        ContinueLogin = false;
                    }
                    if (ContinueLogin)
                    {
                        AD.UserName = model.UserID;
                        AD.Pwd = model.Pwd;
                        AD.validType = AD.ValidType.Domain;
                        AD.LDAPName = LDAPName;

                        //VA.SignData = model.SignData;
                        //VA.Plaintext = model.Plaintext;
                        //VA.txnCode = "TxnCode";
                        //VA.VAVerifyURL = VAVerifyURL;
                        //VA.Tolerate = 120;

                        DateTime LoginStartTime = DateTime.Now;
                        SF.logandshowInfo("登入開始@" + LoginStartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        bool LoginResult = LP.DoLogin(UseCertLogin, AD, VA);
                        DateTime LoginEndTime = DateTime.Now;
                        SF.logandshowInfo("登入結束@" + LoginEndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        string LoginSpanTime = OtherProcesser.TimeDiff(LoginStartTime, LoginEndTime, "Milliseconds");
                        SF.logandshowInfo("本次登入共花費@" + LoginSpanTime + "毫秒", log_Info);

                        if (LoginResult == true)
                        {
                            //登入成功，需紀錄
                            SL.Result = true;
                            SL.EndTime = DateTime.Now;
                            SL.SuccessCount = 1;
                            SL.FailCount = 0;
                            SL.Msg = "登入成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                            Session["UseCertLogin"] = UseCertLogin;
                            //Session["UseCertLogin"] = true;
                            Session["UserID"] = model.UserID;
                            //Session["UserID"] = "TAS191";
                            int UserRole = SF.getUserRole(model.UserID);
                            Session["UserRole"] = UserRole;

                            return RedirectToAction("Index", "Attribute");
                        }
                        else
                        {
                            //string a=VA.ResultStr;

                            //登入失敗，需記錄錯誤
                            SL.EndTime = DateTime.Now;
                            SL.FailCount = 1;
                            if (UseCertLogin)
                            {
                                SL.Msg = Configer.LoginAction + "作業失敗，錯誤訊息:[AD或VA驗證失敗]";
                            }
                            else
                            {
                                SL.Msg = Configer.LoginAction + "作業失敗，錯誤訊息:[AD驗證失敗]";
                            }
                            SL.Result = false;
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                            TempData["LoginMsg"] = SL.Msg;

                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        TempData["LoginMsg"] = Configer.LoginAction + "作業失敗，錯誤訊息:[系統登入參數遺失]";
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.LoginAction + "作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Logout()
        {
            //初始化系統參數
            Configer.Init();

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = Session["UserID"].ToString();
            SL.Controller = "Account";
            SL.Action = "Logout";
            SL.StartTime = DateTime.Now;
            SL.TotalCount = 1;

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;
            //string SendResult = string.Empty;

            try
            {
                SL.SuccessCount = 1;
                SL.FailCount = 0;
                SL.Result = true;
                SL.Msg = Configer.LogoutAction + "作業成功" ;
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                Session.Clear();

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.LogoutAction + "作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Login", "Account");
            }
        }
    }
}