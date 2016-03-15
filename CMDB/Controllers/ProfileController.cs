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
    public class ProfileController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: Profile
        public ActionResult Index()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 22;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Profile";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "範本清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Proflies_List vProList = new vCI_Proflies_List();
                SF.logandshowInfo(Configer.GetAction + "範本清單-子程序-" + Configer.GetAction + "待覆核範本資料筆數開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProList.ReviewCount = context.Tmp_CI_Proflies.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();
                SF.logandshowInfo(Configer.GetAction + "範本清單子程序" + Configer.GetAction + "待覆核範本資料筆數結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "待覆核範本資料結果:共取得[" + vProList.ReviewCount.ToString() + "]筆", log_Info);

                SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProList.ProfilesData = SF.getProfilesData();
                SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ProfileCount = 0;

                if (vProList.ProfilesData != null)
                {
                    ProfileCount = vProList.ProfilesData.Count();
                    SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "範本資料結果:共取得[" + ProfileCount.ToString() + "]筆", log_Info);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "範本資料結果:共取得[0]筆", log_Info);
                }

                vProList.Authority = SF.getAuthority(true, false, nowRole, nowFunction);
                //vProList.EditAccount = SF.canEdit("CI_Profiles", "");

                if (ProfileCount > 0)
                {
                    foreach (var item in vProList.ProfilesData)
                    {
                        SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "正在編輯範本資料的帳號開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        item.EditAccount = SF.canEdit("CI_Profiles", item.ProfileID.ToString(), "");
                        SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "正在編輯範本資料的帳號結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.GetAction + "範本清單子程序-" + Configer.GetAction + "正在編輯範本資料的帳號結果:屬性[" + item.ProfileName + "];編輯帳號[" + item.EditAccount + "]", log_Info);
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "範本清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "範本清單成功", log_Info);
                    SL.TotalCount = ProfileCount;
                    SL.SuccessCount = ProfileCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "範本清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(vProList);
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "範本清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "範本清單成功，系統尚未建立範本", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "範本清單作業成功，系統尚未建立範本";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "尚未建立範本";

                    return View(vProList);
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "範本清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "範本清單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "屬性清單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
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
            SL.Controller = "Profile";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "新增範本表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Proflies_CU vProCU = new vCI_Proflies_CU();
                SF.logandshowInfo(Configer.CreateAction + "新增範本表單子程序-" + Configer.GetAction + "屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vProCU.AttributesData = SF.getAttributesData();
                SF.logandshowInfo(Configer.CreateAction + "新增範本表單子程序-" + Configer.GetAction + "屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

              
                int AttributeCount = vProCU.AttributesData.Count();

                if (AttributeCount > 0)
                {
                    SF.logandshowInfo(Configer.GetAction + "新增範本表單子程序-" + Configer.GetAction + "屬性資料結果:共取得[" + AttributeCount.ToString() + "]筆", log_Info);
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "新增範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "新增範本表單成功", log_Info);
                    SL.TotalCount = AttributeCount;
                    SL.SuccessCount = AttributeCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "新增範本表單作業成功，" + "共取得[" + AttributeCount + "]筆屬性資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(vProCU);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "新增範本表單子程序-" + Configer.GetAction + "屬性資料結果:共取得[0]筆", log_Info);
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "新增範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "新增範本表單作業成功，系統尚未建立屬性", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "新增範本表單作業成功，系統尚未建立屬性";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "系統尚未建立屬性";

                    return View();
                }

            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "新增範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "新增範本表單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "新增範本表單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Profile");
            }
        }

        // POST: Create
        [HttpPost]
        public string Create(vCI_Proflies_CU vProCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Profile";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "範本開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

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

                        Tmp_CI_Proflies _Tmp_CI_Proflies = new Tmp_CI_Proflies();
                        _Tmp_CI_Proflies.ProfileName = vProCU.ProfileName;
                        PlainText.Append(_Tmp_CI_Proflies.ProfileName + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.ImgID = vProCU.ImgID;
                        PlainText.Append(_Tmp_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.Description = vProCU.Description;
                        PlainText.Append(_Tmp_CI_Proflies.Description + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.CreateAccount = nowUser;
                        PlainText.Append(_Tmp_CI_Proflies.CreateAccount + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.CreateTime = DateTime.Now;
                        PlainText.Append(_Tmp_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.Type = Configer.CreateAction;
                        PlainText.Append(_Tmp_CI_Proflies.Type + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.isClose = false;
                        PlainText.Append(_Tmp_CI_Proflies.isClose.ToString());

                        //計算HASH值
                        SF.logandshowInfo(Configer.CreateAction + "範本子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        _Tmp_CI_Proflies.HashValue = SF.getHashValue(PlainText.ToString());
                        SF.logandshowInfo(Configer.CreateAction + "範本子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "範本子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Proflies.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.CreateAction + "範本子程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        context.Tmp_CI_Proflies.Add(_Tmp_CI_Proflies);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.CreateAction + "範本子程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        StringProcessor.Claersb(PlainText);

                    
                        foreach (var item in vProCU.AttributesData)
                        {
                            Tmp_CI_Proflie_Attributes _Tmp_CI_Proflie_Attributes = new Tmp_CI_Proflie_Attributes();
                            _Tmp_CI_Proflie_Attributes.ProfileID = _Tmp_CI_Proflies.ProfileID;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.ProfileID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.AttributeID = item.AttributeID;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.AttributeOrder = item.AttributeOrder;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.CreateAccount = nowUser;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.CreateAccount + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.CreateTime = DateTime.Now;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.Type = Configer.CreateAction;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.Type + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.isClose = false;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                            //計算HASH值
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序-" + Configer.CreateAction + "屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            _Tmp_CI_Proflie_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序--" + Configer.CreateAction + "屬性子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序--" + Configer.CreateAction + "屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Proflie_Attributes.HashValue + "]", log_Info);

                            StringProcessor.Claersb(PlainText);
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序--" + Configer.CreateAction + "屬性子程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            context.Tmp_CI_Proflie_Attributes.Add(_Tmp_CI_Proflie_Attributes);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序--" + Configer.CreateAction + "屬性子程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        }

                        //context.SaveChanges();
                        dbContextTransaction.Commit();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.CreateAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "範本作業成功", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 1;
                        SL.FailCount = 0;
                        SL.Result = false;
                        SL.Msg = Configer.CreateAction + "範本作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.CreateAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "範本作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.CreateAction + "範本作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "範本作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "範本作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                    return SL.Msg;
                }
            }
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
            SL.Controller = "Profile";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "編輯範本表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                CI_Proflies _CI_Proflies = new CI_Proflies();
                vCI_Proflies_CU vProCU = new vCI_Proflies_CU();

                SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _CI_Proflies = context.CI_Proflies.Where(b => b.ProfileID == ProfileID).First();
                SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_CI_Proflies != null)
                {
                    vProCU.ProfileID = _CI_Proflies.ProfileID;
                    vProCU.ProfileName = _CI_Proflies.ProfileName;
                    vProCU.Description = _CI_Proflies.Description;
                    vProCU.ImgID = _CI_Proflies.ImgID;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "圖片路徑開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    vProCU.ImgPath = SF.getSysImgPath(_CI_Proflies.ImgID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "圖片路徑開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性清單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    vProCU.AttributesData = SF.getProfileAttributesData(ProfileID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性清單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性清單結果:共取得[" + vProCU.AttributesData.Count() + " ]筆資料", log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "編輯範本表單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(vProCU);
                }
                else
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單作業失敗，異常訊息[查無原始範本資料]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "編輯範本表單作業失敗，查無原始範本資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "Profile");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "編輯範本表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "編輯範本表單作業失敗，異常訊息["+ ex.ToString() +"]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "編輯範本表單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Profile");
            }
        }

        //POST:Edit
        [HttpPost]
        public string Edit(vCI_Proflies_CU vProCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Profile";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.EditAction + "範本開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

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

                        Tmp_CI_Proflies _Tmp_CI_Proflies = new Tmp_CI_Proflies();
                        _Tmp_CI_Proflies.oProfileID = vProCU.ProfileID;
                        PlainText.Append(_Tmp_CI_Proflies.oProfileID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.ProfileName = vProCU.ProfileName;
                        PlainText.Append(_Tmp_CI_Proflies.ProfileName + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.ImgID = vProCU.ImgID;
                        PlainText.Append(_Tmp_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.Description = vProCU.Description;
                        PlainText.Append(_Tmp_CI_Proflies.Description + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.CreateAccount = nowUser;
                        PlainText.Append(_Tmp_CI_Proflies.CreateAccount + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.CreateTime = DateTime.Now;
                        PlainText.Append(_Tmp_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.Type = Configer.EditAction;
                        PlainText.Append(_Tmp_CI_Proflies.Type + Configer.SplitSymbol);

                        _Tmp_CI_Proflies.isClose = false;
                        PlainText.Append(_Tmp_CI_Proflies.isClose.ToString());

                        //計算HASH值
                        SF.logandshowInfo(Configer.EditAction + "範本子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        _Tmp_CI_Proflies.HashValue = SF.getHashValue(PlainText.ToString());
                        SF.logandshowInfo(Configer.EditAction + "範本子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "範本子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Proflies.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.EditAction + "範本子程序-"+ Configer.EditAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        context.Tmp_CI_Proflies.Add(_Tmp_CI_Proflies);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.EditAction + "範本子程序-"+ Configer.EditAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        StringProcessor.Claersb(PlainText);

                        foreach (var item in vProCU.AttributesData)
                        {
                            Tmp_CI_Proflie_Attributes _Tmp_CI_Proflie_Attributes = new Tmp_CI_Proflie_Attributes();
                            _Tmp_CI_Proflie_Attributes.ProfileID = _Tmp_CI_Proflies.ProfileID;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.ProfileID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.AttributeID = item.AttributeID;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.AttributeOrder = item.AttributeOrder;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.CreateAccount = nowUser;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.CreateAccount + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.CreateTime = DateTime.Now;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.Type = Configer.EditAction;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.Type + Configer.SplitSymbol);

                            _Tmp_CI_Proflie_Attributes.isClose = false;
                            PlainText.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                            //計算HASH值
                            SF.logandshowInfo(Configer.EditAction + "範本子程序-"+ Configer.EditAction + "屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            _Tmp_CI_Proflie_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.EditAction + "範本子程序-" + Configer.EditAction + "屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.EditAction + "範本子程序-" + Configer.EditAction + "屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Proflie_Attributes.HashValue + "]", log_Info);

                            StringProcessor.Claersb(PlainText);

                            SF.logandshowInfo(Configer.CreateAction + "範本子程序-" + Configer.EditAction + "屬性子程序-"+ Configer.EditAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            context.Tmp_CI_Proflie_Attributes.Add(_Tmp_CI_Proflie_Attributes);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "範本子程序-" + Configer.EditAction + "屬性子程序-"+ Configer.EditAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        }

                        //context.SaveChanges();
                        dbContextTransaction.Commit();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "範本作業成功", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 1;
                        SL.FailCount = 0;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "範本作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "範本作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "範本作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.EditAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.EditAction + "範本作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.EditAction + "範本作業失敗，異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                    return SL.Msg;
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
            SL.Controller = "Profile";
            SL.Action = "ReviewIndex";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核範本清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                var query = from Pro in context.Tmp_CI_Proflies
                            .Where(b => b.isClose == false).Where(b => b.CreateAccount != nowUser)
                            join Img in context.SystemImgs on Pro.ImgID equals Img.ImgID
                            join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                            join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vTmp_CI_Profiles
                            {
                                ProfileID = Pro.ProfileID,
                                ProfileName = Pro.ProfileName,
                                Description = Pro.Description,
                                ImgID = Pro.ImgID,
                                ImgPath = Img.ImgPath,
                                Creator = Cre.Name,
                                CreateTime = Pro.CreateTime,
                                Type = Pro.Type
                            };

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ReviewCount = query.Count();
                SF.logandshowInfo(Configer.GetAction + "待覆核範本清單結果:共取得[" + ReviewCount.ToString() + "]筆", log_Info);

                if (ReviewCount > 0)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本清單作業成功", log_Info);
                    SL.TotalCount = ReviewCount;
                    SL.SuccessCount = ReviewCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(query);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本清單作業成功，系統尚未產生待覆核範本", log_Info);
                    SL.TotalCount = 0;
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本清單作業成功，系統尚未產生待覆核範本";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "無待覆核範本";

                    return View();
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本清單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核範本清單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Profile");
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
            SL.Controller = "Profile";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核範本資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vTmp_CI_Profiles_R _vTmp_CI_Profiles_R = new vTmp_CI_Profiles_R();
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料子程序-" + Configer.GetAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _vTmp_CI_Profiles_R = getReviewData(ProfileID);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料子程序-" + Configer.GetAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_vTmp_CI_Profiles_R != null)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核範本資料作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核範本資料作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(_vTmp_CI_Profiles_R);
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

                    return RedirectToAction("ReviewIndex", "Profile");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核範本資料作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核範本資料作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("ReviewIndex", "Profile");
            }
        }

        // POST: Review
        [HttpPost]
        public ActionResult Review(vTmp_CI_Profiles_R _vTmp_CI_Profiles_R)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Profile";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.ReviewAction + "範本開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                if (ModelState.IsValid)
                {
                    CI_Proflies _CI_Proflies = new CI_Proflies();
                    Tmp_CI_Proflies _Tmp_CI_Proflies = new Tmp_CI_Proflies();

                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.GetAction + "待覆核範本開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    _Tmp_CI_Proflies = context.Tmp_CI_Proflies.Find(_vTmp_CI_Profiles_R.ProfileID);
                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.GetAction + "待覆核範本結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (_Tmp_CI_Proflies != null)
                    {
                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-取得待覆核範本結果:共取得[1]筆", log_Info);
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            switch (_Tmp_CI_Proflies.Type)
                            {
                                case "建立":
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    StringBuilder verifyPlainText = new StringBuilder();
                                    string verifyHashValue = string.Empty;
                                    StringBuilder PlainText = new StringBuilder();

                                    _CI_Proflies.ProfileID = _Tmp_CI_Proflies.ProfileID;
                                  
                                    _CI_Proflies.ProfileName = _Tmp_CI_Proflies.ProfileName;
                                    verifyPlainText.Append(_Tmp_CI_Proflies.ProfileName + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Proflies.ProfileName + Configer.SplitSymbol);

                                    _CI_Proflies.ImgID = _Tmp_CI_Proflies.ImgID;
                                    verifyPlainText.Append(_Tmp_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);

                                    _CI_Proflies.Description = _Tmp_CI_Proflies.Description;
                                    verifyPlainText.Append(_Tmp_CI_Proflies.Description + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Proflies.Description + Configer.SplitSymbol);

                                    _CI_Proflies.CreateAccount = _Tmp_CI_Proflies.CreateAccount;
                                    verifyPlainText.Append(_Tmp_CI_Proflies.CreateAccount + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Proflies.CreateAccount + Configer.SplitSymbol);

                                    _CI_Proflies.CreateTime = _Tmp_CI_Proflies.CreateTime;
                                    verifyPlainText.Append(_Tmp_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);

                                    verifyPlainText.Append(_Tmp_CI_Proflies.Type + Configer.SplitSymbol);
                                    verifyPlainText.Append(_Tmp_CI_Proflies.isClose.ToString());

                                    //重新計算HASH
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    verifyPlainText.Replace("False", "");
                                    verifyPlainText.Remove(verifyPlainText.Length-1,1);
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Proflies.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                    //與原HASH相比
                                    if (verifyHashValue == _Tmp_CI_Proflies.HashValue)
                                    {
                                        _CI_Proflies.UpdateAccount = nowUser;
                                        PlainText.Append(Configer.SplitSymbol + _CI_Proflies.UpdateAccount + Configer.SplitSymbol);

                                        _CI_Proflies.UpdateTime = DateTime.Now;
                                        PlainText.Append(_CI_Proflies.UpdateTime.ToString());

                                        //計算HASH值
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        _CI_Proflies.HashValue = SF.getHashValue(PlainText.ToString());
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算屬性HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _CI_Proflies.HashValue + "]", log_Info);

                                        //新增範本
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Proflies.Add(_CI_Proflies);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        var query = context.Tmp_CI_Proflie_Attributes.Where(b => b.ProfileID == _vTmp_CI_Profiles_R.ProfileID);

                                        int TmpProflieAttributesCount = query.Count();

                                        if (TmpProflieAttributesCount > 0)
                                        {
                                            try
                                            {
                                                foreach (var item in query.ToList())
                                                {
                                                    CI_Proflie_Attributes _CI_Proflie_Attributes = new CI_Proflie_Attributes();
                                                    Tmp_CI_Proflie_Attributes _Tmp_CI_Proflie_Attributes = new Tmp_CI_Proflie_Attributes();

                                                    _Tmp_CI_Proflie_Attributes = context.Tmp_CI_Proflie_Attributes.Where(b => b.AttributeID == item.AttributeID)
                                                         .Where(b => b.ProfileID == _Tmp_CI_Proflies.ProfileID).First();

                                                    if (_Tmp_CI_Proflie_Attributes != null)
                                                    {
                                                        StringBuilder verifyPlainText1 = new StringBuilder();
                                                        string verifyHashValue1 = string.Empty;
                                                        StringBuilder PlainText1= new StringBuilder();

                                                        _CI_Proflie_Attributes.ProfileID = _Tmp_CI_Proflie_Attributes.ProfileID;
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.ProfileID.ToString() + Configer.SplitSymbol);
                                                        PlainText1.Append(_CI_Proflie_Attributes.ProfileID.ToString() + Configer.SplitSymbol);

                                                        _CI_Proflie_Attributes.AttributeID = _Tmp_CI_Proflie_Attributes.AttributeID;
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);
                                                        PlainText1.Append(_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);

                                                        _CI_Proflie_Attributes.AttributeOrder = _Tmp_CI_Proflie_Attributes.AttributeOrder;
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);
                                                        PlainText1.Append(_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                        _CI_Proflie_Attributes.CreateAccount = _Tmp_CI_Proflie_Attributes.CreateAccount;
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.CreateAccount + Configer.SplitSymbol);
                                                        PlainText1.Append(_CI_Proflie_Attributes.CreateAccount+ Configer.SplitSymbol);

                                                        _CI_Proflie_Attributes.CreateTime = _Tmp_CI_Proflie_Attributes.CreateTime;
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.CreateTime.ToString() + Configer.SplitSymbol);
                                                        PlainText1.Append(_CI_Proflie_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.Type + Configer.SplitSymbol);
                                                        verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                                                        //重新計算HASH
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        verifyHashValue = SF.getHashValue(verifyPlainText1.ToString());
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        verifyPlainText1.Replace("False", "");
                                                        verifyPlainText1.Remove(verifyPlainText1.Length-1, 1);
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本屬性作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Proflie_Attributes.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                                        if (verifyHashValue == _Tmp_CI_Proflie_Attributes.HashValue)
                                                        {
                                                            _CI_Proflie_Attributes.UpdateAccount = nowUser;
                                                            PlainText1.Append(Configer.SplitSymbol+ _CI_Proflie_Attributes.UpdateAccount + Configer.SplitSymbol);

                                                            _CI_Proflie_Attributes.UpdateTime = DateTime.Now;
                                                            PlainText1.Append(_CI_Proflie_Attributes.UpdateTime.ToString());

                                                            //計算HASH值
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            _CI_Proflie_Attributes.HashValue = SF.getHashValue(PlainText1.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值結果:明文:[" + PlainText1.ToString() + "];HASH值[" + _CI_Proflie_Attributes.HashValue + "]", log_Info);

                                                            //新增範本屬性
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            context.CI_Proflie_Attributes.Add(_CI_Proflie_Attributes);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                            //修改待覆核屬性
                                                            _Tmp_CI_Proflie_Attributes.ReviewAccount = nowUser;
                                                            verifyPlainText1.Append(Configer.SplitSymbol+_Tmp_CI_Proflie_Attributes.ReviewAccount + Configer.SplitSymbol);

                                                            _Tmp_CI_Proflie_Attributes.ReviewTime = DateTime.Now;
                                                            verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.ReviewTime.ToString() + Configer.SplitSymbol);

                                                            _Tmp_CI_Proflie_Attributes.isClose = true;
                                                            verifyPlainText1.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                                                            //計算HASH值
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            _Tmp_CI_Proflie_Attributes.HashValue = SF.getHashValue(verifyPlainText1.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值結果:明文:[" + verifyPlainText1.ToString() + "];HASH值[" + _Tmp_CI_Proflie_Attributes.HashValue + "]", log_Info);

                                                            context.Entry(_Tmp_CI_Proflie_Attributes).State = EntityState.Modified;
                                                        }
                                                        else
                                                        {
                                                            dbContextTransaction.Rollback();
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            //HASH驗證失敗
                                                            SL.EndTime = DateTime.Now;
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Proflie_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                                            SL.TotalCount = 1;
                                                            SL.SuccessCount = 0;
                                                            SL.FailCount = 1;
                                                            SL.Result = false;
                                                            SL.Msg = Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Proflie_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                            return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dbContextTransaction.Rollback();
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        //記錄錯誤
                                                        SL.EndTime = DateTime.Now;
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[系統無此屬性]", log_Info);
                                                        SL.TotalCount = 1;
                                                        SL.SuccessCount = 0;
                                                        SL.FailCount = 1;
                                                        SL.Result = false;
                                                        SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[系統無此屬性]";
                                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                        return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                                    }
                                                }

                                                //修改待覆核屬性
                                                _Tmp_CI_Proflies.ReviewAccount = nowUser;
                                                verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Proflies.ReviewAccount + Configer.SplitSymbol);

                                                _Tmp_CI_Proflies.ReviewTime = DateTime.Now;
                                                verifyPlainText.Append( _Tmp_CI_Proflies.ReviewTime.ToString() + Configer.SplitSymbol);

                                                _Tmp_CI_Proflies.isClose = true;
                                                verifyPlainText.Append( _Tmp_CI_Proflies.isClose.ToString());

                                                //計算HASH值
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                _Tmp_CI_Proflies.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核屬性HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH[" + _Tmp_CI_Proflies.HashValue + "]", log_Info);

                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                context.Entry(_Tmp_CI_Proflies).State = EntityState.Modified;
                                                context.SaveChanges();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                //範本及範本屬性一起異動
                                                dbContextTransaction.Commit();

                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本作業成功，作業類型[" + _vTmp_CI_Profiles_R.Type + "]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 1;
                                                SL.FailCount = 0;
                                                SL.Result = true;
                                                SL.Msg = Configer.ReviewAction + "範本作業成功，作業類型[" + _vTmp_CI_Profiles_R.Type + "]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                int ReviewCount = context.Tmp_CI_Proflies.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                                if (ReviewCount > 0)
                                                {
                                                    return RedirectToAction("ReviewIndex", "Profile");
                                                }
                                                else
                                                {
                                                    return RedirectToAction("Index", "Profile");
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

                                                return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                            }
                                        }
                                        else
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                            //記錄錯誤
                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[無範本屬性對應資料]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[無範本屬性對應資料]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                        }
                                    }
                                    else
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        //HASH驗證失敗
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本HASH驗證失敗，原HASH:" + _Tmp_CI_Proflies.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本HASH驗證失敗，原HASH:" + _Tmp_CI_Proflies.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                    }

                                case "編輯":
                                    StringBuilder verifyPlainText2 = new StringBuilder();
                                    string verifyHashValue2 = string.Empty;
                                    StringBuilder PlainText2 = new StringBuilder();

                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-取得待覆核範本開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    _CI_Proflies = context.CI_Proflies.Where(b => b.ProfileID == _Tmp_CI_Proflies.oProfileID).First();
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-取得待覆核範本結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                    if (_CI_Proflies != null)
                                    {
                                        try
                                        {
                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            _CI_Proflies.ProfileID = _Tmp_CI_Proflies.oProfileID;
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.oProfileID.ToString() + Configer.SplitSymbol);
                                            PlainText2.Append(_CI_Proflies.ProfileID.ToString() + Configer.SplitSymbol);

                                            _CI_Proflies.ProfileName = _Tmp_CI_Proflies.ProfileName;
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.ProfileName + Configer.SplitSymbol);
                                            PlainText2.Append(_CI_Proflies.ProfileName + Configer.SplitSymbol);

                                            _CI_Proflies.ImgID = _Tmp_CI_Proflies.ImgID;
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);
                                            PlainText2.Append(_CI_Proflies.ImgID.ToString() + Configer.SplitSymbol);

                                            _CI_Proflies.Description = _Tmp_CI_Proflies.Description;
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.Description + Configer.SplitSymbol);
                                            PlainText2.Append(_CI_Proflies.Description + Configer.SplitSymbol);

                                            verifyPlainText2.Append(_Tmp_CI_Proflies.CreateAccount + Configer.SplitSymbol);
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.Type + Configer.SplitSymbol);
                                            verifyPlainText2.Append(_Tmp_CI_Proflies.isClose.ToString());

                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            verifyHashValue2 = SF.getHashValue(verifyPlainText2.ToString());
                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            verifyPlainText2.Replace("False","");
                                            verifyPlainText2.Remove(verifyPlainText2.Length - 1, 1);

                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Proflies.HashValue + "];重新計算HASH[" + verifyPlainText2 + "]", log_Info);

                                            if (verifyHashValue2 == _Tmp_CI_Proflies.HashValue)
                                            {
                                                PlainText2.Append(_CI_Proflies.CreateAccount + Configer.SplitSymbol);
                                                PlainText2.Append(_CI_Proflies.CreateTime.ToString() + Configer.SplitSymbol);

                                                _CI_Proflies.UpdateAccount = nowUser;
                                                PlainText2.Append(_CI_Proflies.UpdateAccount+ Configer.SplitSymbol);

                                                _CI_Proflies.UpdateTime = DateTime.Now;
                                                PlainText2.Append(_CI_Proflies.UpdateTime.ToString());

                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                _CI_Proflies.HashValue = SF.getHashValue(PlainText2.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本HASH值結果:明文:[" + PlainText2.ToString() + "];HASH值[" + _CI_Proflies.HashValue + "]", log_Info);

                                                context.Entry(_CI_Proflies).State = EntityState.Modified;

                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-"+Configer.GetAction + "待覆核範本屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                var query1 = context.Tmp_CI_Proflie_Attributes.Where(b => b.ProfileID == _vTmp_CI_Profiles_R.ProfileID);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.GetAction + "待覆核範本屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                int TmpProflieAttributesCount1 = query1.Count();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.GetAction + "待覆核範本屬性結果，共取得[" + TmpProflieAttributesCount1 .ToString()+"]筆", log_Info);

                                                if (TmpProflieAttributesCount1 > 0)
                                                {
                                                    //移除CI_Proflie_Attributes
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-移除原範本屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    context.CI_Proflie_Attributes.RemoveRange(context.CI_Proflie_Attributes.Where(b => b.ProfileID == _Tmp_CI_Proflies.oProfileID));
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-移除原範本屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                               
                                                    foreach (var item in query1.ToList())
                                                    {
                                                        CI_Proflie_Attributes _CI_Proflie_Attributes = new CI_Proflie_Attributes();
                                                        Tmp_CI_Proflie_Attributes _Tmp_CI_Proflie_Attributes = new Tmp_CI_Proflie_Attributes();

                                                        _Tmp_CI_Proflie_Attributes = context.Tmp_CI_Proflie_Attributes.Where(b => b.AttributeID == item.AttributeID)
                                                       .Where(b => b.ProfileID == _Tmp_CI_Proflies.ProfileID).First();

                                                        if (_Tmp_CI_Proflie_Attributes != null)
                                                        {
                                                            StringBuilder verifyPlainText3 = new StringBuilder();
                                                            string verifyHashValue3 = string.Empty;
                                                            StringBuilder PlainText3 = new StringBuilder();

                                                            _CI_Proflie_Attributes.ProfileID = _Tmp_CI_Proflies.oProfileID;
                                                            verifyPlainText3.Append(_Tmp_CI_Proflies.ProfileID.ToString()+Configer.SplitSymbol);
                                                            PlainText3.Append(_Tmp_CI_Proflies.oProfileID.ToString() + Configer.SplitSymbol);

                                                            _CI_Proflie_Attributes.AttributeID = _Tmp_CI_Proflie_Attributes.AttributeID;
                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);
                                                            PlainText3.Append(_CI_Proflie_Attributes.AttributeID.ToString() + Configer.SplitSymbol);

                                                            _CI_Proflie_Attributes.AttributeOrder = _Tmp_CI_Proflie_Attributes.AttributeOrder;
                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);
                                                            PlainText3.Append(_CI_Proflie_Attributes.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                            _CI_Proflie_Attributes.CreateAccount = _Tmp_CI_Proflie_Attributes.CreateAccount;
                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.CreateAccount + Configer.SplitSymbol);
                                                            PlainText3.Append(_CI_Proflie_Attributes.CreateAccount+ Configer.SplitSymbol);

                                                            _CI_Proflie_Attributes.CreateTime = _Tmp_CI_Proflie_Attributes.CreateTime;
                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.CreateTime.ToString()+ Configer.SplitSymbol);
                                                            PlainText3.Append(_CI_Proflie_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.Type + Configer.SplitSymbol);
                                                            verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                                                            //重新計算HASH
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            verifyHashValue3 = SF.getHashValue(verifyPlainText3.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            verifyPlainText3.Replace("False", "");
                                                            verifyPlainText3.Remove(verifyPlainText3.Length - 1, 1);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本屬性作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Proflie_Attributes.HashValue + "];重新計算HASH[" + verifyPlainText3.ToString() + "]", log_Info);

                                                            if (verifyHashValue3 == _Tmp_CI_Proflie_Attributes.HashValue)
                                                            {
                                                                _CI_Proflie_Attributes.UpdateAccount = nowUser;
                                                                PlainText3.Append(_CI_Proflie_Attributes.UpdateAccount + Configer.SplitSymbol);

                                                                _CI_Proflie_Attributes.UpdateTime = DateTime.Now;
                                                                PlainText3.Append(_CI_Proflie_Attributes.UpdateTime.ToString());

                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                _CI_Proflie_Attributes.HashValue = SF.getHashValue(PlainText3.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算範本屬性HASH值結果:明文:[" + PlainText3.ToString() + "];HASH值[" + _CI_Proflie_Attributes.HashValue + "]", log_Info);
                                                                StringProcessor.Claersb(PlainText3);

                                                                //新增範本屬性
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                context.CI_Proflie_Attributes.Add(_CI_Proflie_Attributes);
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                                //修改待覆核屬性
                                                                _Tmp_CI_Proflie_Attributes.ReviewAccount = nowUser;
                                                                verifyPlainText3.Append(Configer.SplitSymbol+ _Tmp_CI_Proflie_Attributes.ReviewAccount + Configer.SplitSymbol);

                                                                _Tmp_CI_Proflie_Attributes.ReviewTime = DateTime.Now;
                                                                verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.ReviewTime.ToString() + Configer.SplitSymbol);

                                                                _Tmp_CI_Proflie_Attributes.isClose = true;
                                                                verifyPlainText3.Append(_Tmp_CI_Proflie_Attributes.isClose.ToString());

                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                _Tmp_CI_Proflie_Attributes.HashValue = SF.getHashValue(verifyPlainText3.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值結果:明文:[" + verifyPlainText3.ToString() + "];HASH值[" + _Tmp_CI_Proflie_Attributes.HashValue + "]", log_Info);

                                                                StringProcessor.Claersb(verifyPlainText3);

                                                                context.Entry(_Tmp_CI_Proflie_Attributes).State = EntityState.Modified;
                                                                context.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                //HASH驗證錯誤
                                                                dbContextTransaction.Rollback();
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                                //HASH驗證失敗
                                                                SL.EndTime = DateTime.Now;
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Proflie_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue3 + "]", log_Info);
                                                                SL.TotalCount = 1;
                                                                SL.SuccessCount = 0;
                                                                SL.FailCount = 1;
                                                                SL.Result = false;
                                                                SL.Msg = Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Proflie_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue3 + "]";
                                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dbContextTransaction.Rollback();
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                            //記錄錯誤
                                                            SL.EndTime = DateTime.Now;
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[系統無此屬性]", log_Info);
                                                            SL.TotalCount = 1;
                                                            SL.SuccessCount = 0;
                                                            SL.FailCount = 1;
                                                            SL.Result = false;
                                                            SL.Msg = Configer.ReviewAction + "範本屬性作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[系統無此屬性]";
                                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                            return RedirectToAction("Review", "Profile", new { AttributeID = _vTmp_CI_Profiles_R.ProfileID });
                                                        }
                                                    }

                                                    //尋覽有用到此範本的物件，更新異動後屬性
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-"+Configer.UpdateAction+"物件子程序-"+Configer.GetAction+"使用[" + _Tmp_CI_Proflies.oProfileID + "]範本的物件開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    var _CI_Objects = context.CI_Objects.Where(b => b.ProfileID == _Tmp_CI_Proflies.oProfileID);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "使用[" + _Tmp_CI_Proflies.oProfileID + "]範本的物件結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    int ObjectCount = _CI_Objects.Count();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "使用[" + _Tmp_CI_Proflies.oProfileID + "]範本的物件結果，共取得["+ ObjectCount + "]筆", log_Info);

                                                    if (ObjectCount > 0)
                                                    {
                                                        foreach (var o in _CI_Objects.ToList())
                                                        {
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "使用[" + _Tmp_CI_Proflies.oProfileID + "]範本屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            var ObjectsData = context.CI_Object_Data.Where(b => b.ObjectID == o.ObjectID);
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "使用[" + _Tmp_CI_Proflies.oProfileID + "]範本屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                            int ObjectsDataCount = ObjectsData.Count();
                                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "使用[" + _Tmp_CI_Proflies.oProfileID + "]範本屬性結果，共取得[" + ObjectsDataCount + "]筆", log_Info);

                                                            if (ObjectsDataCount > 0)
                                                            {
                                                                var Set1 = context.CI_Proflie_Attributes.Where(b => b.ProfileID == o.ProfileID).Select(b => b.AttributeID);
                                                                var Set2 = ObjectsData.Select(b => b.AttributeID);
                                                                //用Tmp差集Objects Attribute--新增
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction+ "需"+Configer.InsertAction+ "物件屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                var insertAttributeIDs = Set1.Except(Set2).ToList();
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "需" + Configer.InsertAction + "物件屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                                //var insertAttributeIDs =context.Tmp_CI_Proflie_Attributes.Where(b=>b.ProfileID==o.ProfileID).Select(b=>b.AttributeID).ToArray()
                                                                //                     .Except(ObjectsData.Select(b=> b.AttributeID).ToArray()).ToArray();

                                                                int insertCount = insertAttributeIDs.Count();
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "需" + Configer.InsertAction + "物件屬性作業結果，共取得[" + insertCount .ToString()+ "]筆需" + Configer.InsertAction + "屬性", log_Info);

                                                                if (insertCount > 0)
                                                                {
                                                                    foreach (var insertAttributeID in insertAttributeIDs)
                                                                    {
                                                                        CI_Object_Data _insert_CI_Object_Data = new CI_Object_Data();
                                                                        _insert_CI_Object_Data.ObjectID = o.ObjectID;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.ObjectID.ToString()+ Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.AttributeID = insertAttributeID;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.AttributeValue = "none"; //給預設值
                                                                        //PlainText4.Append(_insert_CI_Object_Data.AttributeValue+ Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.AttributeOrder = context.CI_Proflie_Attributes.Where(b => b.ProfileID == o.ProfileID).Where(b => b.AttributeID == insertAttributeID).First().AttributeOrder;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.CreateAccount = nowUser;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.CreateAccount + Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.CreateTime = DateTime.Now;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.UpdateAccount = nowUser;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.UpdateAccount + Configer.SplitSymbol);

                                                                        _insert_CI_Object_Data.UpdateTime = DateTime.Now;
                                                                        //PlainText4.Append(_insert_CI_Object_Data.UpdateTime.ToString() + Configer.SplitSymbol);

                                                                        // _insert_CI_Object_Data.HashValue = SF.getHashValue(PlainText4.ToString());

                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.InsertAction + "物件屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                        context.CI_Object_Data.Add(_insert_CI_Object_Data);
                                                                        context.SaveChanges();
                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" +  Configer.InsertAction + "物件屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                    }
                                                                }

                                                                //用Objects Attribute差集Tmp--移除
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "需"+Configer.RemoveAction+ "物件屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                var removeAttributeIDs = Set2.Except(Set1).ToList();
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "需" + Configer.RemoveAction + "物件屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat),log_Info);

                                                                int removeCount = removeAttributeIDs.Count();
                                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.GetAction + "需" + Configer.RemoveAction + "物件屬性作業結果，共取得[" + insertCount.ToString() + "]筆需" + Configer.RemoveAction + "屬性", log_Info);

                                                                if (removeCount > 0)
                                                                {
                                                                    foreach (var removeAttributeID in removeAttributeIDs)
                                                                    {
                                                                        CI_Object_Data _remove_CI_Object_Data = context.CI_Object_Data.Where(b => b.ObjectID == o.ObjectID)
                                                                            .Where(b => b.AttributeID == removeAttributeID).First();

                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" +  Configer.RemoveAction + "物件屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat),log_Info);
                                                                        context.CI_Object_Data.Remove(_remove_CI_Object_Data);
                                                                        context.SaveChanges();
                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-" + Configer.RemoveAction + "物件屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                    }
                                                                }

                                                                //更新Attribute排序及HASH值
                                                                var newObjectsData = context.CI_Object_Data.Where(b => b.ObjectID == o.ObjectID).ToList();
                                                                int newObjectsDataCount = newObjectsData.Count();

                                                                if (newObjectsDataCount > 0)
                                                                {
                                                                    foreach (var newObj in newObjectsData)
                                                                    {
                                                                        int newAttributeID = newObj.AttributeID;
                                                                        int oldAttributeOrder = newObj.AttributeOrder;
                                                                        int newAttributeOrder = context.CI_Proflie_Attributes.Where(b => b.ProfileID == o.ObjectID).Where(b => b.AttributeID == newAttributeID).First().AttributeOrder;

                                                                        if (oldAttributeOrder != newAttributeOrder)
                                                                        {
                                                                            newObj.AttributeOrder = newAttributeOrder;
                                                                        }

                                                                        StringBuilder verifyPlainText4 = new StringBuilder();
                                                                        string verifyHashValue4 = string.Empty;
                                                                        StringBuilder PlainText4 = new StringBuilder();

                                                                        PlainText4.Append(newObj.ObjectID.ToString() + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.AttributeID.ToString() + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.AttributeValue + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.AttributeOrder.ToString() + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.CreateAccount + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.CreateTime.ToString() + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.UpdateAccount + Configer.SplitSymbol);
                                                                        PlainText4.Append(newObj.UpdateTime.ToString());

                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-重新計算物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                        newObj.HashValue = SF.getHashValue(PlainText4.ToString());
                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-重新計算物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.UpdateAction + "物件子程序-重新計算物件屬性結果:明文:["+ PlainText4.ToString() + "];HASH值[" + newObj.HashValue + "]", log_Info);

                                                                        context.Entry(newObj).State = EntityState.Modified;
                                                                        // context.SaveChanges();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        context.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        //沒有任何物件有套用此範本
                                                        //範本及範本屬性及物件一起異動
                                                        context.SaveChanges();
                                                    }

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Proflies.ReviewAccount = nowUser;
                                                    verifyPlainText2.Append(Configer.SplitSymbol + _Tmp_CI_Proflies.ReviewAccount+ Configer.SplitSymbol);

                                                    _Tmp_CI_Proflies.ReviewTime = DateTime.Now;
                                                    verifyPlainText2.Append(_Tmp_CI_Proflies.ReviewTime.ToString() + Configer.SplitSymbol);

                                                    _Tmp_CI_Proflies.isClose = true;
                                                    verifyPlainText2.Append(_Tmp_CI_Proflies.isClose.ToString() + Configer.SplitSymbol);

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _Tmp_CI_Proflies.HashValue = SF.getHashValue(verifyPlainText2.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-計算待覆核範本屬性HASH值結果:明文:[" + verifyPlainText2.ToString() + "];HASH值[" + _Tmp_CI_Proflies.HashValue + "]", log_Info);

                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    context.Entry(_Tmp_CI_Proflies).State = EntityState.Modified;
                                                    context.SaveChanges();
                                                    dbContextTransaction.Commit();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存待覆核範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本作業成功，作業類型[" + _vTmp_CI_Profiles_R.Type + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 1;
                                                    SL.FailCount = 0;
                                                    SL.Result = true;
                                                    SL.Msg = Configer.ReviewAction + "範本作業成功，作業類型[" + _vTmp_CI_Profiles_R.Type + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    int ReviewCount = context.Tmp_CI_Proflies.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                                    if (ReviewCount > 0)
                                                    {
                                                        return RedirectToAction("ReviewIndex", "Profile");
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction("Index", "Profile");
                                                    }
                                                }
                                                else
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    //記錄錯誤
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[無範本屬性對應資料]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[無範本屬性對應資料]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                                }
                                            }
                                            else
                                            {
                                                //HASH驗證錯誤
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                //HASH驗證失敗
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本HASH驗證失敗，原HASH:" + _Tmp_CI_Proflies.HashValue + "；重新計算HASH:" + verifyHashValue2 + "]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[範本HASH驗證失敗，原HASH:" + _Tmp_CI_Proflies.HashValue + "；重新計算HASH:" + verifyHashValue2 + "]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-儲存範本資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                            //記錄錯誤
                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "範本作業失敗，異常訊息[資料庫作業失敗:"+ ex .ToString()+ "]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                        }
                                    }
                                    else
                                    {
                                        //記錄錯誤
                                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[查無原始範本資料]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[查無原始範本資料]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
                                    }
                                default:
                                    //記錄錯誤
                                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                    SL.EndTime = DateTime.Now;
                                    SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[系統不存在的作業類型]", log_Info);
                                    SL.TotalCount = 1;
                                    SL.SuccessCount = 0;
                                    SL.FailCount = 1;
                                    SL.Result = false;
                                    SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[系統不存在的作業類型]";
                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                    return RedirectToAction("Review", "Profile", new { AttributeID = _vTmp_CI_Profiles_R.ProfileID });
                            }
                        }
                    }
                    else
                    {
                        //記錄錯誤
                        SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Proflies.Type + "]，異常訊息[查無待覆核範本資料]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _vTmp_CI_Profiles_R.Type + "]，異常訊息[查無待覆核範本資料]";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Review", "Profile", new { AttributeID = _vTmp_CI_Profiles_R.ProfileID });
                    }
                }
                else
                {
                    //記錄錯誤
                    SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，異常訊息[資料驗證失敗]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.ReviewAction + "範本作業失敗，異常訊息[資料驗證失敗]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Review", "Profile", new { AttributeID = _vTmp_CI_Profiles_R.ProfileID });
                }
            }
            catch (Exception ex)
            {
                //SF.logandshowInfo(Configer.ReviewAction + "範本子程序-" + Configer.EditAction + "範本作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.ReviewAction + "範本結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.ReviewAction + "範本作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.ReviewAction + "範本作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                //TempData["CreateMsg"] = "<script>alert('覆核屬性發生異常');</script>";

                return RedirectToAction("Review", "Profile", new { ProfileID = _vTmp_CI_Profiles_R.ProfileID });
            }
        }

        /// <summary>
        /// 取得待覆核範本資料
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        private vTmp_CI_Profiles_R getReviewData(int ProfileID)
        {
            vTmp_CI_Profiles_R _vTmp_CI_Profiles_R = new vTmp_CI_Profiles_R();
            vCI_Proflies _vCI_Proflies = new vCI_Proflies();
            var query = from Pro in context.Tmp_CI_Proflies
                        .Where(b => b.ProfileID == ProfileID)
                        join Img in context.SystemImgs on Pro.ImgID equals Img.ImgID
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                         into y
                        from x in y.DefaultIfEmpty()
                        select new vTmp_CI_Profiles_R
                        {
                            ProfileID = Pro.ProfileID,
                            oProfileID = Pro.oProfileID,
                            ProfileName = Pro.ProfileName,
                            Description = Pro.Description,
                            ImgID = Pro.ImgID,
                            ImgPath = Img.ImgPath,
                            Creator = x.Name,
                            CreateTime = Pro.CreateTime,
                            Type = Pro.Type
                        };

            if (query.Count() == 1)
            {
                _vTmp_CI_Profiles_R = query.First();
                _vTmp_CI_Profiles_R.AttributesData = SF.getReviewProfileAttributesData(ProfileID);

                if (_vTmp_CI_Profiles_R.Type == Configer.CreateAction)
                {
                    return _vTmp_CI_Profiles_R;
                }
                else if (_vTmp_CI_Profiles_R.Type == Configer.EditAction || _vTmp_CI_Profiles_R.Type == Configer.RemoveAction)
                {
                    _vCI_Proflies = getProfileData(_vTmp_CI_Profiles_R.oProfileID);

                    if (_vCI_Proflies != null)
                    {
                        _vTmp_CI_Profiles_R.oProfileID = _vTmp_CI_Profiles_R.oProfileID;
                        _vTmp_CI_Profiles_R.oProfileName = _vCI_Proflies.ProfileName;
                        _vTmp_CI_Profiles_R.oDescription = _vCI_Proflies.Description;
                        _vTmp_CI_Profiles_R.oImgID = _vCI_Proflies.ImgID;
                        _vTmp_CI_Profiles_R.oImgPath = _vCI_Proflies.ImgPath;
                        _vTmp_CI_Profiles_R.oAttributesData = SF.getProfileAttributesData(_vTmp_CI_Profiles_R.oProfileID);
                        _vTmp_CI_Profiles_R.oUpadter = _vCI_Proflies.Upadter;
                        _vTmp_CI_Profiles_R.oUpdateTime = _vCI_Proflies.UpdateTime;

                        return _vTmp_CI_Profiles_R;
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
        /// 取得原範本資料
        /// </summary>
        /// <param name="AttributeID">範本ID</param>
        /// <returns></returns>
        private vCI_Proflies getProfileData(int ProfileID)
        {
            vCI_Proflies _vCI_Proflies = new vCI_Proflies();

            var query = from Pro in context.CI_Proflies
                        .Where(b => b.ProfileID == ProfileID)
                        join Img in context.SystemImgs on Pro.ImgID equals Img.ImgID
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Proflies
                        {
                            ProfileID = Pro.ProfileID,
                            ProfileName = Pro.ProfileName,
                            Description = Pro.Description,
                            ImgID = Pro.ImgID,
                            ImgPath = Img.ImgPath,
                            Creator = Cre.Name,
                            CreateTime = Pro.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Pro.UpdateTime
                        };

            if (query.Count() == 1)
            {
                _vCI_Proflies = query.First();
                _vCI_Proflies.AttributesData = SF.getProfileAttributesData(ProfileID);

                return _vCI_Proflies;
            }
            else
            {
                return null;
            }
        }
    }
}