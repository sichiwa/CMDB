using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text;
using CMDB.DAL;
using CMDB.Models;
using CMDB.ViewModels;
using CMDB.SystemClass;
using TWCAlib;


namespace CMDB.Controllers
{
    public class ProfileRelationshipController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";


        // GET: ProfileRelationship
        public ActionResult Index()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 24;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "範本關係清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Profile_Relationship_List vProRList = new vCI_Profile_Relationship_List();
                SF.logandshowInfo(Configer.GetAction + "範本關係清單-子程序-" + Configer.GetAction + "待覆核範本關係資料筆數開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProRList.ReviewCount = context.Tmp_CI_Profile_Relationship.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();
                SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序" + Configer.GetAction + "待覆核範本關係資料筆數結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "待覆核範本關係資料結果:共取得[" + vProRList.ReviewCount.ToString() + "]筆", log_Info);

                SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProRList.ProfileRelationshipData = SF.getProfileRelationshipData();
                SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ProfileRCount = 0;

                if (vProRList.ProfileRelationshipData != null)
                {
                    ProfileRCount = vProRList.ProfileRelationshipData.Count();
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "範本資料結果:共取得[" + ProfileRCount.ToString() + "]筆", log_Info);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "範本關係資料結果:共取得[0]筆", log_Info);
                }

                vProRList.Authority = SF.getAuthority(true, false, nowRole, nowFunction);

                if (ProfileRCount > 0)
                {
                    foreach (var item in vProRList.ProfileRelationshipData)
                    {
                        SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "正在編輯範本關係本資料的帳號開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        item.EditAccount = SF.canEdit("CI_Profile_Relationship", item.ProfileID.ToString(), "");
                        SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "正在編輯範本關係資料的帳號結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.GetAction + "範本關係清單子程序-" + Configer.GetAction + "正在編輯範本關係資料的帳號結果:範本關係[" + item.ProfileName + "];編輯帳號[" + item.EditAccount + "]", log_Info);
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單成功", log_Info);
                    SL.TotalCount = ProfileRCount;
                    SL.SuccessCount = ProfileRCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "範本關係清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(vProRList);
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "範本關係清單成功，系統尚未建立範本關係", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "範本關係清單作業成功，系統尚未建立範本關係";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "尚未建立範本關係";

                    return View(vProRList);
                }
            }
            catch (Exception ex)
            {

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "範本關係清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "範本關係清單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "範本關係清單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Login", "Account");
            }

        }

        // GET: Create
        public ActionResult Create()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "新增範本關係表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Profile_Relationship_CU vProRCU = new vCI_Profile_Relationship_CU();
                SF.logandshowInfo(Configer.CreateAction + "新增範本關係表單-子程序-" + Configer.GetAction + "範本清單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProRCU.ProfileList = SF.getProfileList(1);
                vProRCU.RelationshipProfileList = vProRCU.ProfileList;
                SF.logandshowInfo(Configer.CreateAction + "新增範本關係表單-子程序-" + Configer.GetAction + "範本清單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "新增範本關係表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "新增範本關係表單成功", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 1;
                SL.FailCount = 0;
                SL.Result = true;
                SL.Msg = Configer.CreateAction + "新增範本關係表單作業成功";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                TempData["SystemInfo"] = "OK";

                return View(vProRCU);
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "新增範本關係表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "新增範本關係表單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "新增範本關係表單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "ProfileRelationship");
            }
        }

        // POST: Create
        [HttpPost]
        public string Create(vCI_Profile_Relationship_CU vProRCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "範本關係開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        StringBuilder PlainText = new StringBuilder();

                        if (CheckRep(vProRCU) == false)
                        {
                            foreach (int item in vProRCU.RelationshipProileID)
                            {
                                Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();
                                _Tmp_CI_Profile_Relationship.ProfileID = vProRCU.ProfileID;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.RelationshipProileID = item;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.CreateAccount = nowUser;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.CreateTime = DateTime.Now;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.Type = Configer.CreateAction;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.isClose = false;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                //計算HASH值
                                SF.logandshowInfo(Configer.CreateAction + "範本關係程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                                SF.logandshowInfo(Configer.CreateAction + "範本關係程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                SF.logandshowInfo(Configer.CreateAction + "範本關係程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                                StringProcessor.Claersb(PlainText);
                                SF.logandshowInfo(Configer.CreateAction + "範本關係程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                context.Tmp_CI_Profile_Relationship.Add(_Tmp_CI_Profile_Relationship);
                                context.SaveChanges();
                                SF.logandshowInfo(Configer.CreateAction + "範本關係程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            }

                            //context.SaveChanges();
                            dbContextTransaction.Commit();

                            SL.EndTime = DateTime.Now;
                            SF.logandshowInfo(Configer.CreateAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "範本關係作業成功", log_Info);
                            SL.TotalCount = vProRCU.RelationshipProileID.Count();
                            SL.SuccessCount = vProRCU.RelationshipProileID.Count();
                            SL.FailCount = 0;
                            SL.Result = false;
                            SL.Msg = Configer.CreateAction + "範本關係作業成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                            return SL.Msg;
                        }
                        else {
                            //資料重複
                            dbContextTransaction.Rollback();

                            SL.EndTime = DateTime.Now;
                            SF.logandshowInfo(Configer.CreateAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "範本關係作業失敗，異常訊息[資料重複]", log_Info);
                            SL.TotalCount = vProRCU.RelationshipProileID.Count();
                            SL.SuccessCount = 0;
                            SL.FailCount = vProRCU.RelationshipProileID.Count();
                            SL.Result = false;
                            SL.Msg = Configer.CreateAction + "範本關係作業失敗，資料重複";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                            return SL.Msg;
                        }
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.CreateAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "範本關係作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = vProRCU.RelationshipProileID.Count();
                        SL.SuccessCount = 0;
                        SL.FailCount = vProRCU.RelationshipProileID.Count();
                        SL.Result = false;
                        SL.Msg = Configer.CreateAction + "範本關係作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = vProRCU.RelationshipProileID.Count();
                    SL.SuccessCount = 0;
                    SL.FailCount = vProRCU.RelationshipProileID.Count();
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "範本關係作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                    return SL.Msg;
                }
            }
        }

        /// <summary>
        /// 檢查資料是否有重複
        /// </summary>
        /// <param name="vProRCU"></param>
        /// <returns>false:沒有重複資料;true:有重複資料</returns>
        private bool CheckRep(vCI_Profile_Relationship_CU vProRCU)
        {
            bool CheckResult = false;
            SF.logandshowInfo(Configer.CreateAction + "範本關係程序-檢查重複資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
            if (vProRCU.RelationshipProileID.Count() > 0)
            {
                foreach (int item in vProRCU.RelationshipProileID)
                {
                    int CheckCount = context.CI_Profile_Relationship.Where(b => b.ProfileID == vProRCU.ProfileID)
                         .Where(b => b.RelationshipProfileID == item).Count();

                    if (CheckCount > 0)
                    {
                        SF.logandshowInfo(Configer.CreateAction + "範本關係程序-檢查重複資料-發現重複資料", log_Info);
                        CheckResult = true;
                        SF.logandshowInfo(Configer.CreateAction + "範本關係程序-檢查重複資料-重複資料[ProfileID:" + vProRCU.ProfileID.ToString() + "];[RelationshipProfileID:" + item.ToString() + "]", log_Info);
                    }
                }
            }
            SF.logandshowInfo(Configer.CreateAction + "範本關係程序-檢查重複資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
            SF.logandshowInfo(Configer.CreateAction + "範本關係程序-檢查重複資結果:[" + CheckResult.ToString() + "]", log_Info);
            return CheckResult;
        }

        // GET: Edit
        public ActionResult Edit(int ProfileID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                CI_Profile_Relationship _CI_Profile_Relationship = new CI_Profile_Relationship();
                vCI_Profile_Relationship_CU vProRCU = new vCI_Profile_Relationship_CU();

                SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單-子程序" + Configer.GetAction + "範本關係資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                var query = context.CI_Profile_Relationship.Where(b => b.ProfileID == ProfileID);
                SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單-子程序" + Configer.GetAction + "範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (query.Count() > 0l)
                {
                    _CI_Profile_Relationship = query.First();
                    vProRCU.ProfileID = _CI_Profile_Relationship.ProfileID;

                    List<int> RelationshipProfileIDs = new List<int>();
                    foreach (var item in query.ToList())
                    {
                        RelationshipProfileIDs.Add(item.RelationshipProfileID);
                    }

                    vProRCU.RelationshipProileID = RelationshipProfileIDs.ToArray();
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單關係-子程序" + Configer.GetAction + "範本關係結果:共取得[" + vProRCU.RelationshipProileID.Count() + " ]筆資料", log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單-子程序-" + Configer.GetAction + "範本清單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    vProRCU.ProfileList = SF.getProfileList(-1);
                    vProRCU.RelationshipProfileList = vProRCU.ProfileList;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單-子程序-" + Configer.GetAction + "範本清單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "編輯範本關係表單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(vProRCU);
                }
                else
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單作業失敗，異常訊息[查無原始範本關係資料]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "編輯範本關係表單作業失敗，查無原始範本資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "ProfileRelationship");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "編輯範本關係表單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "編輯範本關係表單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "ProfileRelationship");
            }
        }

        //POST: Edit
        [HttpPost]
        public string Edit(vCI_Profile_Relationship_CU vProRCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.EditAction + "範本關係開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        StringBuilder PlainText = new StringBuilder();
                        foreach (int item in vProRCU.RelationshipProileID)
                        {
                            Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();
                            _Tmp_CI_Profile_Relationship.ProfileID = vProRCU.ProfileID;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.oProfileID = vProRCU.ProfileID;
                            //PlainText.Append(_Tmp_CI_Profile_Relationship.oProfileID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.RelationshipProileID = item;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.CreateAccount = nowUser;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.CreateTime = DateTime.Now;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.Type = Configer.EditAction;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);

                            _Tmp_CI_Profile_Relationship.isClose = false;
                            PlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                            //計算HASH值
                            SF.logandshowInfo(Configer.EditAction + "範本關係程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.EditAction + "範本關係程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.EditAction + "範本關係程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                            StringProcessor.Claersb(PlainText);
                            SF.logandshowInfo(Configer.CreateAction + "範本關係程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            context.Tmp_CI_Profile_Relationship.Add(_Tmp_CI_Profile_Relationship);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "範本關係程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        }

                        //context.SaveChanges();
                        dbContextTransaction.Commit();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "範本關係作業成功", log_Info);
                        SL.TotalCount = vProRCU.RelationshipProileID.Count();
                        SL.SuccessCount = vProRCU.RelationshipProileID.Count();
                        SL.FailCount = 0;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "範本關係作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "範本關係作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "範本關係作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.EditAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.EditAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.EditAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                    return SL.Msg;
                }
            }
        }

        // GET: Delete
        public ActionResult Delete(int ProfileID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Delete";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.DeleteAction + "範本關係開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var query = context.CI_Profile_Relationship
                    .Where(b => b.ProfileID == ProfileID);

                        if (query.Count() > 0)
                        {
                            StringBuilder PlainText = new StringBuilder();
                            foreach (var item in query.ToList())
                            {
                                Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();
                                _Tmp_CI_Profile_Relationship.ProfileID = item.ProfileID;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.oProfileID = item.ProfileID;
                                //PlainText.Append(_Tmp_CI_Profile_Relationship.oProfileID.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.RelationshipProileID = item.RelationshipProfileID;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.CreateAccount = nowUser;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.CreateTime = DateTime.Now;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.Type = Configer.DeleteAction;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);

                                _Tmp_CI_Profile_Relationship.isClose = false;
                                PlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                //計算HASH值
                                SF.logandshowInfo(Configer.DeleteAction + "範本關係程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                                SF.logandshowInfo(Configer.DeleteAction + "範本關係程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                SF.logandshowInfo(Configer.DeleteAction + "範本關係程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                                StringProcessor.Claersb(PlainText);
                                SF.logandshowInfo(Configer.DeleteAction + "範本關係程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                context.Tmp_CI_Profile_Relationship.Add(_Tmp_CI_Profile_Relationship);
                                context.SaveChanges();
                                SF.logandshowInfo(Configer.DeleteAction + "範本關係程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            }

                            dbContextTransaction.Commit();

                            SL.EndTime = DateTime.Now;
                            SF.logandshowInfo(Configer.DeleteAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.DeleteAction + "範本關係作業成功", log_Info);
                            SL.TotalCount = query.Count();
                            SL.SuccessCount = query.Count();
                            SL.FailCount = 0;
                            SL.Result = false;
                            SL.Msg = Configer.DeleteAction + "範本關係作業成功";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                            return RedirectToAction("Index", "ProfileRelationship");
                        }
                        else {
                            //記錄錯誤
                            SL.EndTime = DateTime.Now;
                            SF.logandshowInfo(Configer.DeleteAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.DeleteAction + "範本關係作業失敗，異常訊息[查無原始範本關係資料]", log_Info);
                            SL.TotalCount = 0;
                            SL.SuccessCount = 0;
                            SL.FailCount = 0;
                            SL.Result = false;
                            SL.Msg = Configer.DeleteAction + "範本關係作業失敗，查無原始範本資料";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                            return RedirectToAction("Index", "ProfileRelationship");
                        }
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.DeleteAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.DeleteAction + "範本關係作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.DeleteAction + "範本關係作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Index", "ProfileRelationship");
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.DeleteAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.DeleteAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.DeleteAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "ProfileRelationship");
                }
            }
        }

        // GET: ReviewProfile
        public ActionResult ReviewIndex()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "ReviewIndex";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                var query = from ProR in context.Tmp_CI_Profile_Relationship
                            .Where(b => b.isClose == false).Where(b => b.CreateAccount != nowUser)
                            join Pro in context.CI_Proflies on ProR.ProfileID equals Pro.ProfileID
                            join Pro1 in context.CI_Proflies on ProR.RelationshipProileID equals Pro1.ProfileID
                            join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                            join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vTmp_CI_Profile_Relationship
                            {
                                ProfileID = ProR.ProfileID,
                                ProfileName = Pro.ProfileName,
                                RelationProfileName = Pro1.ProfileName,
                                Creator = Cre.Name,
                                CreateTime = Pro.CreateTime,
                                Type = ProR.Type
                            };

                var query1 = query.Select(b => b.ProfileID).Distinct().ToList();

                List<vTmp_CI_Profile_Relationship> vTmpCIProfileRelationships = new List<vTmp_CI_Profile_Relationship>();

                foreach (var item in query1)
                {
                    vTmp_CI_Profile_Relationship T = new vTmp_CI_Profile_Relationship();
                    T.ProfileID = item;
                    StringBuilder bb = new StringBuilder();
                    foreach (var item1 in query.Where(b => b.ProfileID == item).ToList())
                    {
                        if (string.IsNullOrEmpty(T.ProfileName))
                        {
                            T.ProfileName = item1.ProfileName;
                            T.Creator = item1.Creator;
                            T.CreateTime = item1.CreateTime;
                            T.Type = item1.Type;
                        }
                        bb.Append(item1.RelationProfileName + ",");
                    }
                    T.RelationProfileName = StringProcessor.CutlastChar(bb.ToString());
                    vTmpCIProfileRelationships.Add(T);
                }

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ReviewCount = query1.Count();
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單結果:共取得[" + ReviewCount.ToString() + "]筆", log_Info);

                if (ReviewCount > 0)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單作業成功", log_Info);
                    SL.TotalCount = ReviewCount;
                    SL.SuccessCount = ReviewCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本關係清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(vTmpCIProfileRelationships);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單作業成功，系統尚未產生待覆核範本關係", log_Info);
                    SL.TotalCount = 0;
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本關係清單作業成功，系統尚未產生待覆核範本關係";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "無待覆核範本關係";

                    return View();
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係清單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核範本關係清單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "ProfileRelationship");
            }
        }

        // GET: Review
        public ActionResult Review(int ProfileID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vTmp_CI_Profile_Relationship_R _vTmp_CI_Profile_Relationship_R = new vTmp_CI_Profile_Relationship_R();
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料子程序-" + Configer.GetAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _vTmp_CI_Profile_Relationship_R = getReviewData(ProfileID);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料子程序-" + Configer.GetAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_vTmp_CI_Profile_Relationship_R != null)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本關係資料作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(_vTmp_CI_Profile_Relationship_R);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本資料作業失敗，查無待覆核範本[" + ProfileID + "]資料", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.GetAction + "待覆核範本資料作業失敗，查無待覆核範本[" + ProfileID + "]資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("ReviewIndex", "ProfileRelationship");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本關係資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料關係作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核範本資料作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("ReviewIndex", "ProfileRelationship");
            }
        }

        // POST: Review
        [HttpPost]
        public ActionResult Review(vTmp_CI_Profile_Relationship_R _vTmp_CI_Profile_Relationship_R)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "ProfileRelationship";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.ReviewAction + "範本關係開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                if (ModelState.IsValid)
                {
                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.GetAction + "待覆核範本關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    var query = context.Tmp_CI_Profile_Relationship
                        .Where(b => b.ProfileID == _vTmp_CI_Profile_Relationship_R.ProfileID)
                        .Where(b => b.isClose == false);
                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.GetAction + "待覆核範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    int TmpProflieRelationshipCount = query.Count();

                    if (TmpProflieRelationshipCount > 0)
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            switch (query.First().Type)
                            {
                                case "建立":
                                    try
                                    {
                                        foreach (var item in query.ToList())
                                        {
                                            //原始範本關係
                                            CI_Profile_Relationship _CI_Profile_Relationship = new CI_Profile_Relationship();
                                            //成對的範本關係
                                            CI_Profile_Relationship _CI_Profile_Relationship1 = new CI_Profile_Relationship();
                                            Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();

                                            _Tmp_CI_Profile_Relationship = context.Tmp_CI_Profile_Relationship
                                                .Where(b => b.RelationshipProileID == item.RelationshipProileID)
                                                     .Where(b => b.ProfileID == item.ProfileID)
                                                      .Where(b => b.isClose == false).First();

                                            if (_Tmp_CI_Profile_Relationship != null)
                                            {
                                                StringBuilder verifyPlainText = new StringBuilder();
                                                string verifyHashValue = string.Empty;
                                                StringBuilder PlainText = new StringBuilder();
                                                StringBuilder PlainText1 = new StringBuilder();

                                                //先新增成對範本資料
                                                _CI_Profile_Relationship1.ProfileID = _Tmp_CI_Profile_Relationship.RelationshipProileID;
                                                PlainText1.Append(_CI_Profile_Relationship1.ProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship1.RelationshipProfileID = _Tmp_CI_Profile_Relationship.ProfileID;
                                                PlainText1.Append(_CI_Profile_Relationship1.RelationshipProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship1.CreateAccount = _Tmp_CI_Profile_Relationship.CreateAccount;
                                                PlainText1.Append(_CI_Profile_Relationship1.CreateAccount + Configer.SplitSymbol);

                                                _CI_Profile_Relationship1.CreateTime = _Tmp_CI_Profile_Relationship.CreateTime;
                                                PlainText1.Append(_CI_Profile_Relationship1.CreateTime.ToString() + Configer.SplitSymbol);

                                                //新增原範本關係資料
                                                _CI_Profile_Relationship.ProfileID = _Tmp_CI_Profile_Relationship.ProfileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.RelationshipProfileID = _Tmp_CI_Profile_Relationship.RelationshipProileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.RelationshipProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateAccount = _Tmp_CI_Profile_Relationship.CreateAccount;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateTime = _Tmp_CI_Profile_Relationship.CreateTime;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                //重新計算HASH
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyPlainText.Replace("False", "");
                                                verifyPlainText.Remove(verifyPlainText.Length - 1, 1);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                                if (verifyHashValue == _Tmp_CI_Profile_Relationship.HashValue)
                                                {
                                                    _CI_Profile_Relationship1.UpdateAccount = nowUser;
                                                    PlainText1.Append(Configer.SplitSymbol + _CI_Profile_Relationship1.UpdateAccount + Configer.SplitSymbol);

                                                    _CI_Profile_Relationship1.UpdateTime = DateTime.Now;
                                                    PlainText1.Append(_CI_Profile_Relationship1.UpdateTime.ToString());

                                                    _CI_Profile_Relationship.UpdateAccount = nowUser;
                                                    PlainText.Append(Configer.SplitSymbol + _CI_Profile_Relationship.UpdateAccount + Configer.SplitSymbol);

                                                    _CI_Profile_Relationship.UpdateTime = DateTime.Now;
                                                    PlainText.Append(_CI_Profile_Relationship.UpdateTime.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _CI_Profile_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                                                    _CI_Profile_Relationship1.HashValue = SF.getHashValue(PlainText1.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _CI_Profile_Relationship.HashValue + "]", log_Info);

                                                    //新增範本屬性
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存範本關係資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    //成對新增
                                                    context.CI_Profile_Relationship.Add(_CI_Profile_Relationship);
                                                    context.CI_Profile_Relationship.Add(_CI_Profile_Relationship1);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Profile_Relationship.ReviewAccount = nowUser;
                                                    verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Profile_Relationship.ReviewAccount + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.ReviewTime = DateTime.Now;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ReviewTime.ToString() + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.isClose = true;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                                                    context.Entry(_Tmp_CI_Profile_Relationship).State = EntityState.Modified;
                                                }
                                                else
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本程序-" + Configer.CreateAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    //HASH驗證失敗
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[範本關係HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                                }
                                            }
                                            else
                                            {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                //記錄錯誤
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                            }
                                        }

                                        //一起異動
                                        context.SaveChanges();
                                        dbContextTransaction.Commit();

                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 1;
                                        SL.FailCount = 0;
                                        SL.Result = true;
                                        SL.Msg = Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        int ReviewCount = context.Tmp_CI_Profile_Relationship.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                        if (ReviewCount > 0)
                                        {
                                            return RedirectToAction("ReviewIndex", "ProfileRelationship");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "ProfileRelationship");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        //記錄錯誤
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "範本作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                    }

                                case "編輯":
                                    try
                                    {
                                        //移除CI_Profile_Relationship
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-移除原範本關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Profile_Relationship.RemoveRange(context.CI_Profile_Relationship.Where(b => b.ProfileID == _vTmp_CI_Profile_Relationship_R.ProfileID));
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-移除原範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        foreach (var item in query.ToList())
                                        {
                                            //原始範本關係
                                            CI_Profile_Relationship _CI_Profile_Relationship = new CI_Profile_Relationship();
                                            Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();

                                            _Tmp_CI_Profile_Relationship = context.Tmp_CI_Profile_Relationship
                                                .Where(b => b.RelationshipProileID == item.RelationshipProileID)
                                                     .Where(b => b.ProfileID == item.ProfileID)
                                                     .Where(b => b.oProfileID == item.ProfileID)
                                                     .Where(b => b.isClose == false).First();

                                            if (_Tmp_CI_Profile_Relationship != null)
                                            {
                                                StringBuilder verifyPlainText = new StringBuilder();
                                                string verifyHashValue = string.Empty;
                                                StringBuilder PlainText = new StringBuilder();

                                                //新增範本關係資料
                                                _CI_Profile_Relationship.ProfileID = _Tmp_CI_Profile_Relationship.ProfileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.RelationshipProfileID = _Tmp_CI_Profile_Relationship.RelationshipProileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.RelationshipProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateAccount = _Tmp_CI_Profile_Relationship.CreateAccount;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateTime = _Tmp_CI_Profile_Relationship.CreateTime;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                //重新計算HASH
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyPlainText.Replace("False", "");
                                                verifyPlainText.Remove(verifyPlainText.Length - 1, 1);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.CreateAction + "範本關係作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                                if (verifyHashValue == _Tmp_CI_Profile_Relationship.HashValue)
                                                {
                                                    _CI_Profile_Relationship.UpdateAccount = nowUser;
                                                    PlainText.Append(Configer.SplitSymbol + _CI_Profile_Relationship.UpdateAccount + Configer.SplitSymbol);

                                                    _CI_Profile_Relationship.UpdateTime = DateTime.Now;
                                                    PlainText.Append(_CI_Profile_Relationship.UpdateTime.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _CI_Profile_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算範本關係HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _CI_Profile_Relationship.HashValue + "]", log_Info);

                                                    //新增範本屬性
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存範本關係資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    //新增
                                                    context.CI_Profile_Relationship.Add(_CI_Profile_Relationship);

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Profile_Relationship.ReviewAccount = nowUser;
                                                    verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Profile_Relationship.ReviewAccount + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.ReviewTime = DateTime.Now;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ReviewTime.ToString() + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.isClose = true;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                                                    context.Entry(_Tmp_CI_Profile_Relationship).State = EntityState.Modified;
                                                }
                                                else
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本程序-" + Configer.CreateAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    //HASH驗證失敗
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[範本關係HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                                }
                                            }
                                            else
                                            {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.EditAction + "範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                //記錄錯誤
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                            }
                                        }

                                        //一起異動
                                        context.SaveChanges();
                                        dbContextTransaction.Commit();

                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 1;
                                        SL.FailCount = 0;
                                        SL.Result = true;
                                        SL.Msg = Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        int ReviewCount = context.Tmp_CI_Profile_Relationship.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                        if (ReviewCount > 0)
                                        {
                                            return RedirectToAction("ReviewIndex", "ProfileRelationship");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "ProfileRelationship");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.EditAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        //記錄錯誤
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                    }

                                case "刪除":
                                    try
                                    {
                                        foreach (var item in query.ToList())
                                        {
                                            //原始範本關係
                                            CI_Profile_Relationship _CI_Profile_Relationship = new CI_Profile_Relationship();
                                            Tmp_CI_Profile_Relationship _Tmp_CI_Profile_Relationship = new Tmp_CI_Profile_Relationship();

                                            _Tmp_CI_Profile_Relationship = context.Tmp_CI_Profile_Relationship
                                                .Where(b => b.RelationshipProileID == item.RelationshipProileID)
                                                     .Where(b => b.ProfileID == item.ProfileID)
                                                     .Where(b => b.oProfileID == item.ProfileID)
                                                     .Where(b => b.isClose == false).First();

                                            if (_Tmp_CI_Profile_Relationship != null)
                                            {
                                                StringBuilder verifyPlainText = new StringBuilder();
                                                string verifyHashValue = string.Empty;
                                                StringBuilder PlainText = new StringBuilder();

                                                _CI_Profile_Relationship.ProfileID = _Tmp_CI_Profile_Relationship.ProfileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.ProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.RelationshipProfileID = _Tmp_CI_Profile_Relationship.RelationshipProileID;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.RelationshipProileID.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.RelationshipProfileID.ToString() + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateAccount = _Tmp_CI_Profile_Relationship.CreateAccount;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateAccount + Configer.SplitSymbol);

                                                _CI_Profile_Relationship.CreateTime = _Tmp_CI_Profile_Relationship.CreateTime;
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Profile_Relationship.CreateTime.ToString() + Configer.SplitSymbol);

                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.Type + Configer.SplitSymbol);
                                                verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                //重新計算HASH
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.DeleteAction + "範本關係作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.DeleteAction + "範本關係作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                verifyPlainText.Replace("False", "");
                                                verifyPlainText.Remove(verifyPlainText.Length - 1, 1);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.DeleteAction + "範本關係作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                                if (verifyHashValue == _Tmp_CI_Profile_Relationship.HashValue)
                                                {
                                                    //物件關係刪除
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "物件關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    context.CI_Object_Relationship
                                                        .RemoveRange(context.CI_Object_Relationship
                                                        .Where(b => b.ProfileID == item.ProfileID)
                                                        .Where(b => b.RelationshipProfileID == item.RelationshipProileID));

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "物件關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //成對物件關係刪除
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "成對物件關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    context.CI_Object_Relationship
                                                        .RemoveRange(context.CI_Object_Relationship
                                                        .Where(b => b.ProfileID == item.RelationshipProileID)
                                                        .Where(b => b.RelationshipProfileID == item.ProfileID));

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "成對物件關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //範本關係刪除
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "範本關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    context.CI_Profile_Relationship
                                                      .RemoveRange(context.CI_Profile_Relationship
                                                      .Where(b => b.ProfileID == item.ProfileID)
                                                      .Where(b => b.RelationshipProfileID == item.RelationshipProileID));

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //成對範本關係刪除
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "成對範本關係開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    context.CI_Profile_Relationship
                                                    .RemoveRange(context.CI_Profile_Relationship
                                                    .Where(b => b.ProfileID == item.RelationshipProileID)
                                                    .Where(b => b.RelationshipProfileID == item.ProfileID));

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + Configer.DeleteAction + "成對範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Profile_Relationship.ReviewAccount = nowUser;
                                                    verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Profile_Relationship.ReviewAccount + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.ReviewTime = DateTime.Now;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.ReviewTime.ToString() + Configer.SplitSymbol);

                                                    _Tmp_CI_Profile_Relationship.isClose = true;
                                                    verifyPlainText.Append(_Tmp_CI_Profile_Relationship.isClose.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _Tmp_CI_Profile_Relationship.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-計算待覆核範本關係HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + _Tmp_CI_Profile_Relationship.HashValue + "]", log_Info);

                                                    context.Entry(_Tmp_CI_Profile_Relationship).State = EntityState.Modified;
                                                }
                                                else
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本程序-" + Configer.DeleteAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    //HASH驗證失敗
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[範本關係HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Profile_Relationship.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                                }
                                            }
                                            else {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存待覆核範本關係資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.DeleteAction + "範本關係結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                //記錄錯誤
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統無此範本關係]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                            }
                                        }

                                        //一起異動
                                        context.SaveChanges();
                                        dbContextTransaction.Commit();

                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 1;
                                        SL.FailCount = 0;
                                        SL.Result = true;
                                        SL.Msg = Configer.ReviewAction + "範本關係作業成功，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        int ReviewCount = context.Tmp_CI_Profile_Relationship.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                        if (ReviewCount > 0)
                                        {
                                            return RedirectToAction("ReviewIndex", "ProfileRelationship");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "ProfileRelationship");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係子程序-" + _vTmp_CI_Profile_Relationship_R.Type + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        //記錄錯誤
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                                    }

                                default:
                                    //記錄錯誤
                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.RemoveAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                    SL.EndTime = DateTime.Now;
                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統不存在的作業類型]", log_Info);
                                    SL.TotalCount = 1;
                                    SL.SuccessCount = 0;
                                    SL.FailCount = 1;
                                    SL.Result = false;
                                    SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[系統不存在的作業類型]";
                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                    return RedirectToAction("Review", "ProfileRelationship", new { AttributeID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                            }
                        }
                    }
                    else {
                        //dbContextTransaction.Rollback();
                        SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-儲存範本關係屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + Configer.RemoveAction + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        //記錄錯誤
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[無範本關係對應資料]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.ReviewAction + "範本關係作業失敗，作業類型[" + _vTmp_CI_Profile_Relationship_R.Type + "]，異常訊息[無範本關係對應資料]";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                    }
                }
                else
                {
                    //記錄錯誤
                    SF.logandshowInfo(Configer.ReviewAction + "範本關係程序-" + _vTmp_CI_Profile_Relationship_R.Type + "範本關係作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料驗證失敗]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.ReviewAction + "範本關係作業失敗，異常訊息[資料驗證失敗]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.ReviewAction + "範本關係結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.ReviewAction + "範本關係作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.ReviewAction + "範本關係作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                //TempData["CreateMsg"] = "<script>alert('覆核屬性發生異常');</script>";

                return RedirectToAction("Review", "ProfileRelationship", new { ProfileID = _vTmp_CI_Profile_Relationship_R.ProfileID });
            }
        }

        /// <summary>
        /// 取得待覆核範本資料
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        private vTmp_CI_Profile_Relationship_R getReviewData(int ProfileID)
        {
            vTmp_CI_Profile_Relationship_R _vTmp_CI_Profile_Relationship_R = new vTmp_CI_Profile_Relationship_R();
            vCI_Profile_Relationship _vCI_Profile_Relationship = new vCI_Profile_Relationship();
            var query = from ProR in context.Tmp_CI_Profile_Relationship
                        .Where(b => b.ProfileID == ProfileID)
                        join Pro in context.CI_Proflies on ProR.RelationshipProileID equals Pro.ProfileID
                        //join Pro1 in context.CI_Proflies on ProR.RelationshipProileID equals Pro1.ProfileID
                        join Cre in context.Accounts on ProR.CreateAccount equals Cre.Account
                         into y
                        from x in y.DefaultIfEmpty()
                        select new vTmp_CI_Profile_Relationship_R
                        {
                            ProfileID = ProR.ProfileID,
                            oProfileID = ProR.oProfileID,
                            ProfileName = Pro.ProfileName,
                            Creator = x.Name,
                            CreateTime = ProR.CreateTime,
                            Type = ProR.Type
                        };

            if (query.Count() > 0)
            {
                _vTmp_CI_Profile_Relationship_R = query.First();
                _vTmp_CI_Profile_Relationship_R.ProfileID = ProfileID;
                _vTmp_CI_Profile_Relationship_R.ProfileName = context.CI_Proflies.Where(b => b.ProfileID == ProfileID).First().ProfileName;
                _vTmp_CI_Profile_Relationship_R.ProfileRelationshipData = SF.getReviewProfileRelationshipData(ProfileID);

                if (_vTmp_CI_Profile_Relationship_R.Type == Configer.CreateAction)
                {
                    return _vTmp_CI_Profile_Relationship_R;
                }
                else if (_vTmp_CI_Profile_Relationship_R.Type == Configer.EditAction || _vTmp_CI_Profile_Relationship_R.Type == Configer.DeleteAction)
                {
                    _vCI_Profile_Relationship = getProfileRelationshipData(_vTmp_CI_Profile_Relationship_R.oProfileID);

                    if (_vCI_Profile_Relationship != null)
                    {
                        _vTmp_CI_Profile_Relationship_R.oProfileID = _vTmp_CI_Profile_Relationship_R.oProfileID;
                        _vTmp_CI_Profile_Relationship_R.oProfileName = _vTmp_CI_Profile_Relationship_R.ProfileName;

                        _vTmp_CI_Profile_Relationship_R.oProfileRelationshipData = SF.getProfileRelationshipData(_vTmp_CI_Profile_Relationship_R.oProfileID);
                        _vTmp_CI_Profile_Relationship_R.oUpadter = _vCI_Profile_Relationship.Upadter;
                        _vTmp_CI_Profile_Relationship_R.oUpdateTime = _vCI_Profile_Relationship.UpdateTime;

                        return _vTmp_CI_Profile_Relationship_R;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得原範本關係資料
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        private vCI_Profile_Relationship getProfileRelationshipData(int ProfileID)
        {
            vCI_Profile_Relationship _vCI_Profile_Relationship = new vCI_Profile_Relationship();

            var query = from ProR in context.CI_Profile_Relationship
                        .Where(b => b.ProfileID == ProfileID)
                        join Pro in context.CI_Proflies on ProR.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Profile_Relationship
                        {
                            ProfileID = ProR.ProfileID,
                            ProfileName = Pro.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = ProR.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = ProR.UpdateTime
                        };

            if (query.Count() == 1)
            {
                _vCI_Profile_Relationship = query.First();
                //_vCI_Profile_Relationship.RelationshipProileName = SF.getProfileAttributesData(ProfileID);

                return _vCI_Profile_Relationship;
            }
            else
            {
                return null;
            }
        }
    }
}