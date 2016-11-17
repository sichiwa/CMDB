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
using System.Linq.Expressions;

namespace CMDB.Controllers
{
    public class ObjectController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: Object
        public ActionResult Index()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 23;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "物件清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Objects_List vObjList = new vCI_Objects_List();
                SF.logandshowInfo(Configer.GetAction + "物件清單-子程序-" + Configer.GetAction + "待覆核物件資料筆數開始@" + SF.getNowDateString(), log_Info);
                vObjList.ReviewCount = context.Tmp_CI_Objects.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序" + Configer.GetAction + "待覆核物件資料筆數結束@" + SF.getNowDateString(), log_Info);
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "待覆核物件資料結果:共取得[" + vObjList.ReviewCount.ToString() + "]筆", log_Info);

                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料開始@" + SF.getNowDateString(), log_Info);
                vObjList.ObjectsData = SF.getObjectsData();
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料結束@" + SF.getNowDateString(), log_Info);

                int ObjectCount = 0;

                if (vObjList.ObjectsData != null)
                {
                    ObjectCount = vObjList.ObjectsData.Count();
                    SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料結果:共取得[" + ObjectCount.ToString() + "]筆", log_Info);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料結果:共取得[0]筆", log_Info);
                }

                vObjList.Authority = SF.getAuthority(true, false, nowRole, nowFunction);
                //vObjList.EditAccount = SF.canEdit("CI_Objects", "");

                if (ObjectCount > 0)
                {
                    foreach (var item in vObjList.ObjectsData)
                    {
                        SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號開始@" + SF.getNowDateString(), log_Info);
                        item.EditAccount = SF.canEdit("CI_Objects", item.ObjectID.ToString(), "");
                        SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號結束@" + SF.getNowDateString(), log_Info);
                        SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號結果:屬性[" + item.ObjectName + "];編輯帳號[" + item.EditAccount + "]", log_Info);
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "物件清單成功", log_Info);
                    SL.TotalCount = ObjectCount;
                    SL.SuccessCount = ObjectCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "物件清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    vObjList.ProfileID = 0;
                    vObjList.Profile = SF.getProfileList(0);

                    return View(vObjList);
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "物件清單成功，系統尚未建立物件", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "物件清單作業成功，系統尚未建立物件";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "尚未建立物件";

                    return View(vObjList);
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "物件清單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "物件清單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Create
        public ActionResult Create(int ProfileID = -1)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "新增物件表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Objects_CU vObjCU = new vCI_Objects_CU();

                SF.logandshowInfo(Configer.CreateAction + "新增物件表單子程序-" + Configer.GetAction + "範本類型下拉式選單開始@" + SF.getNowDateString(), log_Info);
                if (ProfileID > 0){
                    vObjCU.Profile = SF.getProfileList(ProfileID);
                }
                else {
                    vObjCU.Profile = SF.getProfileList(0);
                }

                SF.logandshowInfo(Configer.CreateAction + "新增物件表單子程序-" + Configer.GetAction + "範本類型下拉式選單結束@" + SF.getNowDateString(), log_Info);

                int ProfileCount = vObjCU.Profile.Count();

                if (ProfileCount > 0)
                {
                    SF.logandshowInfo(Configer.CreateAction + "新增物件表單子程序-" + Configer.GetAction + "屬性類型下拉式選單結果:共取得[" + ProfileCount.ToString() + "]", log_Info);
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "新增物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "新增物件表單成功", log_Info);
                    SL.TotalCount = ProfileCount;
                    SL.SuccessCount = ProfileCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "新增物件表單作業成功，" + "共取得[" + ProfileCount + "]筆範本資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(vObjCU);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "新增物件表單子程序-" + Configer.GetAction + "範本資料結果:共取得[0]筆", log_Info);
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "新增物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "新增物件表單作業成功，系統尚未建立範本", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "新增物件表單作業成功，系統尚未建立範本";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "系統尚未建立範本";

                    return View();
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "新增物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "新增物件表單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "新增物件表單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Object");
            }
        }

        // POST: Create
        [HttpPost]
        public string Create(vCI_Objects_CU vObjCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "物件開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid){
                        string PlainText = string.Empty;

                        Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();
                        _Tmp_CI_Objects = getNeedSaveTmpObject(vObjCU, Configer.CreateAction, nowUser);
                      
                        //計算HASH值
                        PlainText = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects, Configer.VerifyAction, nowUser);
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                        _Tmp_CI_Objects.HashValue = SF.getHashValue(PlainText.ToString());
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增資料開始@" + SF.getNowDateString(), log_Info);
                        context.Tmp_CI_Objects.Add(_Tmp_CI_Objects);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增資料結束@" + SF.getNowDateString(), log_Info);

                        //新增物件屬性資料
                        foreach (var item in vObjCU.AttributesData)
                        {
                            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();
                            _Tmp_CI_Object_Data = getNeedSaveTmpObjectData(_Tmp_CI_Objects.ObjectID, item,Configer.CreateAction, nowUser);
                           
                            //計算HASH值
                            PlainText = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data,Configer.VerifyAction,nowUser); 
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                            _Tmp_CI_Object_Data.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-新增資料開始@" + SF.getNowDateString(), log_Info);
                            context.Tmp_CI_Object_Data.Add(_Tmp_CI_Object_Data);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-新增資料結束@" + SF.getNowDateString(), log_Info);
                        }

                        //新增物件關係
                        foreach (var item in vObjCU.ObjectRelationshipData.RelationshipObjectID)
                        {
                            Tmp_CI_Object_Relationship _Tmp_CI_Object_Relationship = new Tmp_CI_Object_Relationship();
                            _Tmp_CI_Object_Relationship = getNeedSaveTmpObjectRelationship(vObjCU, _Tmp_CI_Objects.ObjectID, item, Configer.CreateAction, nowUser);

                            //計算HASH值
                            PlainText = getNeedCheckReviewObjectRelationshipPlainText(_Tmp_CI_Object_Relationship,Configer.VerifyAction,nowUser);
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增物件關係子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                            _Tmp_CI_Object_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增物件關係子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增物件關係子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Relationship.HashValue + "]", log_Info);

                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增物件關係子程序-新增資料開始@" + SF.getNowDateString(), log_Info);
                            context.Tmp_CI_Object_Relationship.Add(_Tmp_CI_Object_Relationship);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增物件關係子程序-新增資料結束@" + SF.getNowDateString(), log_Info);
                        }

                        dbContextTransaction.Commit();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.CreateAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "物件作業成功", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 1;
                        SL.FailCount = 0;
                        SL.Result = false;
                        SL.Msg = Configer.CreateAction + "物件作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                    else{
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.CreateAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "物件作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.CreateAction + "物件作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "物件作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "物件作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return SL.Msg;
                }
            }
        }

        // GET: Edit
        public ActionResult Edit(int ObjectID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "編輯物件表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                CI_Objects _CI_Objects = new CI_Objects();
                vCI_Objects_CU vObjCU = new vCI_Objects_CU();

                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單-子程序" + Configer.GetAction + "物件資料開始@" + SF.getNowDateString(), log_Info);
                _CI_Objects = context.CI_Objects
                    .Where(b => b.ObjectID == ObjectID)
                    .First();
                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單-子程序" + Configer.GetAction + "物件資料結束@" + SF.getNowDateString(), log_Info);

                if (_CI_Objects != null)
                {
                    vObjCU.ObjectID = _CI_Objects.ObjectID;
                    vObjCU.ObjectName = _CI_Objects.ObjectName;
                    vObjCU.Description = _CI_Objects.Description;
                    vObjCU.ProfileID = _CI_Objects.ProfileID;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單開始@" + SF.getNowDateString(), log_Info);
                    vObjCU.Profile = SF.getProfileList(ObjectID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單結束@" + SF.getNowDateString(), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單結果:共取得[" + vObjCU.Profile.Count() + " ]筆資料", log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性資料開始@" + SF.getNowDateString(), log_Info);
                    vObjCU.AttributesData = SF.getObjectAttributesData(ObjectID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性資料結束@" + SF.getNowDateString(), log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "物件關係資料開始@" + SF.getNowDateString(), log_Info);
                    vObjCU.ObjectRelationshipDatas = SF.getObjectRelationshipData(ObjectID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "物件關係資料結束@" + SF.getNowDateString(), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯物件表單作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "編輯物件表單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(vObjCU);
                }
                else
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯物件表單作業失敗，異常訊息[查無原始物件資料]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "編輯物件表單作業失敗，查無原始物件資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "Object");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "編輯物件表單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Object");
            }
        }

        //POST:Edit
        [HttpPost]
        public string Edit(vCI_Objects_CU vObjCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.EditAction + "物件開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid){
                        string PlainText = string.Empty;

                        Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();
                        _Tmp_CI_Objects = getNeedSaveTmpObject(vObjCU, Configer.EditAction,nowUser);

                        //計算HASH值
                        PlainText = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects,Configer.VerifyAction,nowUser);
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                        _Tmp_CI_Objects.HashValue = SF.getHashValue(PlainText);
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "資料開始@" + SF.getNowDateString(), log_Info);
                        context.Tmp_CI_Objects.Add(_Tmp_CI_Objects);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "資料結束@" + SF.getNowDateString(), log_Info);

                        foreach (var item in vObjCU.AttributesData)
                        {
                            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();
                            _Tmp_CI_Object_Data = getNeedSaveTmpObjectData(_Tmp_CI_Objects.oObjectID, item, Configer.EditAction, nowUser);
                            
                            //計算HASH值
                            PlainText = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data, Configer.VerifyAction, nowUser);
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                            _Tmp_CI_Object_Data.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-" + Configer.EditAction + "屬性子程序-" + Configer.EditAction + "資料開始@" + SF.getNowDateString(), log_Info);
                            context.Tmp_CI_Object_Data.Add(_Tmp_CI_Object_Data);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-" + Configer.EditAction + "屬性子程序-" + Configer.EditAction + "資料結束@" + SF.getNowDateString(), log_Info);
                        }

                        //編輯物件關係
                        foreach (var item in vObjCU.ObjectRelationshipData.RelationshipObjectID)
                        {
                            Tmp_CI_Object_Relationship _Tmp_CI_Object_Relationship = new Tmp_CI_Object_Relationship();
                            _Tmp_CI_Object_Relationship = getNeedSaveTmpObjectRelationship(vObjCU, _Tmp_CI_Objects.ObjectID, item, Configer.EditAction, nowUser);

                            //計算HASH值
                            PlainText = getNeedCheckReviewObjectRelationshipPlainText(_Tmp_CI_Object_Relationship,Configer.VerifyAction,nowUser);
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "物件關係子程序-計算HASH值開始@" + SF.getNowDateString(), log_Info);
                            _Tmp_CI_Object_Relationship.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "物件關係子程序-計算HASH值結束@" + SF.getNowDateString(), log_Info);
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "物件關係子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Relationship.HashValue + "]", log_Info);

                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "物件關係子程序-" + Configer.EditAction + "資料開始@" + SF.getNowDateString(), log_Info);
                            context.Tmp_CI_Object_Relationship.Add(_Tmp_CI_Object_Relationship);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "物件關係子程序-" + Configer.EditAction + "資料結束@" + SF.getNowDateString(), log_Info);
                        }

                        dbContextTransaction.Commit();

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "物件作業成功", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 1;
                        SL.FailCount = 0;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "物件作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                    else{
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.EditAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "物件作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.EditAction + "物件作業失敗，資料驗證失敗";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return SL.Msg;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.EditAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.EditAction + "物件作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.EditAction + "物件作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                    return SL.Msg;
                }
            }
        }

        // GET: ReviewObject
        public ActionResult ReviewIndex()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "ReviewIndex";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核物件清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                var query = from Obj in context.Tmp_CI_Objects
                           .Where(b => b.isClose == false).Where(b => b.CreateAccount != nowUser)
                            join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                            join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                            join Upd in context.Accounts on Obj.CreateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vTmp_CI_Objects
                            {
                                ObjectID = Obj.ObjectID,
                                ObjectName = Obj.ObjectName,
                                ProfileID = Obj.ProfileID,
                                ProfileName = Pro.ProfileName,
                                Description = Obj.Description,
                                Creator = Cre.Name,
                                CreateTime = Obj.CreateTime,
                                Type = Obj.Type
                            };

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ReviewCount = query.Count();
                SF.logandshowInfo(Configer.GetAction + "待覆核物件清單結果:共取得[" + ReviewCount.ToString() + "]筆", log_Info);

                if (ReviewCount > 0)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核物件清單作業成功", log_Info);
                    SL.TotalCount = ReviewCount;
                    SL.SuccessCount = ReviewCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核物件清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(query);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核物件清單作業成功，系統尚未產生待覆核物件", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核物件清單作業成功，系統尚未產生待覆核物件";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "無待覆核物件";

                    return View();
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核物件清單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核物件清單作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Object");
            }
        }

        // GET: Review
        public ActionResult Review(int ObjectID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核物件資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vTmp_CI_Objects_R _vTmp_CI_Objects_R = new vTmp_CI_Objects_R();
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料子程序-" + Configer.GetAction + "資料開始@" + SF.getNowDateString(), log_Info);
                _vTmp_CI_Objects_R = getReviewData(ObjectID);
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料子程序-" + Configer.GetAction + "資料結束@" + SF.getNowDateString(), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_vTmp_CI_Objects_R != null)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核物件資料作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核物件資料作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(_vTmp_CI_Objects_R);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核物件資料作業失敗，查無待覆核物件[" + ObjectID + "]資料", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.GetAction + "待覆核屬性資料作業失敗，查無待覆核物件[" + ObjectID + "]資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("ReviewIndex", "Object");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核物件資料作業失敗，" + "錯誤訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("ReviewIndex", "Object");
            }
        }

        // POST: Review
        [HttpPost]
        public ActionResult Review(vTmp_CI_Objects_R _vTmp_CI_Objects_R)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Object";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.ReviewAction + "物件開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var context = new  CMDBContext())
            {
                try
                {
                    if (ModelState.IsValid){
                        CI_Objects _CI_Objects = new CI_Objects();
                        Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();

                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.GetAction + "待覆核物件開始@" + SF.getNowDateString(), log_Info);
                        _Tmp_CI_Objects = context.Tmp_CI_Objects.Find(_vTmp_CI_Objects_R.ObjectID);
                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.GetAction + "待覆核物件結束@" + SF.getNowDateString(), log_Info);

                        if (_Tmp_CI_Objects != null){
                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件結果:共取得[1]筆", log_Info);
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                switch (_Tmp_CI_Objects.Type)
                                {
                                    case "建立":
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業開始@" + SF.getNowDateString(), log_Info);
                                        string verifyPlainText = string.Empty;
                                        string verifyHashValue = string.Empty;
                                        string PlainText = string.Empty;

                                        //重新計算HASH
                                        verifyPlainText = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects, Configer.VerifyAction, nowUser);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                        verifyHashValue = SF.getHashValue(verifyPlainText);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "範本作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Objects.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                        //與原HASH相比
                                        if (verifyHashValue == _Tmp_CI_Objects.HashValue){
                                            //取得物件
                                            _CI_Objects = getNeedSaveObject(-1,_Tmp_CI_Objects,Configer.CreateAction, nowUser);
                                            PlainText = getNeedSaveObjectPlainText(_CI_Objects);
                                            //計算HASH值
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值開始@" + SF.getNowDateString(), log_Info);
                                            _CI_Objects.HashValue = SF.getHashValue(PlainText);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結束@" + SF.getNowDateString(), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結果:明文:[" + PlainText + "];HASH值[" + _CI_Objects.HashValue + "]", log_Info);

                                            //新增物件
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件資料開始@" + SF.getNowDateString(), log_Info);
                                            context.CI_Objects.Add(_CI_Objects);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件資料結束@" + SF.getNowDateString(), log_Info);

                                            var query = context.Tmp_CI_Object_Data
                                                    .Where(b => b.ObjectID == _vTmp_CI_Objects_R.ObjectID)
                                                    .Where(b => b.isClose == false);

                                            int TmpObjectAttributesCount = query.Count();

                                            if (TmpObjectAttributesCount > 0){
                                                try
                                                {
                                                    foreach (var item in query.ToList())
                                                    {
                                                        CI_Object_Data _CI_Object_Data = new CI_Object_Data();
                                                        Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();

                                                        _Tmp_CI_Object_Data = context.Tmp_CI_Object_Data
                                                             .Where(b => b.AttributeID == item.AttributeID)
                                                             .Where(b => b.ObjectID == _Tmp_CI_Objects.ObjectID)
                                                             .Where(b => b.isClose == false).First();

                                                        if (_Tmp_CI_Object_Data != null){
                                                            string verifyPlainText2 = string.Empty;
                                                            string verifyHashValue2 = string.Empty;
                                                            string PlainText2 = string.Empty;

                                                            //重新計算HASH
                                                            verifyPlainText2 = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data, Configer.VerifyAction, nowUser);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                                            verifyHashValue2 = SF.getHashValue(verifyPlainText2);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Object_Data.HashValue + "];重新計算HASH值[" + verifyHashValue2 + "]", log_Info);

                                                            if (verifyHashValue2 == _Tmp_CI_Object_Data.HashValue){
                                                                //取得物件
                                                                _CI_Object_Data = getNeedSaveObjectData(_Tmp_CI_Object_Data,Configer.EditAction, nowUser);

                                                                //計算HASH值
                                                                PlainText2 = getNeedSaveObjectDataPlainText(_CI_Object_Data);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                _CI_Object_Data.HashValue = SF.getHashValue(PlainText2);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算範本屬性HASH值結果:明文:[" + PlainText2.ToString() + "];HASH值[" + _CI_Object_Data.HashValue + "]", log_Info);

                                                                //新增物件屬性
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料開始@" + SF.getNowDateString(), log_Info);
                                                                context.CI_Object_Data.Add(_CI_Object_Data);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);

                                                                //修改待覆核屬性
                                                                _Tmp_CI_Object_Data.ReviewAccount = nowUser;

                                                                _Tmp_CI_Object_Data.ReviewTime = DateTime.Now;

                                                                _Tmp_CI_Object_Data.isClose = true;

                                                                //計算HASH值
                                                                verifyPlainText2 = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data, Configer.CreateAction, nowUser);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                _Tmp_CI_Object_Data.HashValue = SF.getHashValue(verifyPlainText2.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結果:明文:[" + verifyPlainText2.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                                                                context.Entry(_Tmp_CI_Object_Data).State = EntityState.Modified;
                                                            }
                                                            else{
                                                                dbContextTransaction.Rollback();
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                                //HASH驗證失敗
                                                                SL.EndTime = DateTime.Now;
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件屬性作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Object_Data.HashValue + "；重新計算HASH:" + verifyHashValue2 + "]", log_Info);
                                                                SL.TotalCount = 1;
                                                                SL.SuccessCount = 0;
                                                                SL.FailCount = 1;
                                                                SL.Result = false;
                                                                SL.Msg = Configer.ReviewAction + "物件屬性作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Object_Data.HashValue + "；重新計算HASH:" + verifyHashValue2 + "]";
                                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                            }
                                                        }
                                                        else{
                                                            dbContextTransaction.Rollback();
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                            //記錄錯誤
                                                            SL.EndTime = DateTime.Now;
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件屬性作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[系統無此屬性]", log_Info);
                                                            SL.TotalCount = 1;
                                                            SL.SuccessCount = 0;
                                                            SL.FailCount = 1;
                                                            SL.Result = false;
                                                            SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[系統無此屬性]";
                                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                            return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                        }
                                                    }

                                                    //新增物件關係
                                                    var query2 = context.Tmp_CI_Object_Relationship
                                                            .Where(b => b.ObjectID == _vTmp_CI_Objects_R.ObjectID)
                                                            .Where(b => b.isClose == false);

                                                    int TmpObjectRelationshipCount = query2.Count();

                                                    if (TmpObjectRelationshipCount > 0){
                                                        foreach (var item in query2.ToList())
                                                        {
                                                            //原始物件關係
                                                            CI_Object_Relationship _CI_Object_Relationship = new CI_Object_Relationship();
                                                            //成對的物件關係
                                                            CI_Object_Relationship _CI_Object_Relationship1 = new CI_Object_Relationship();
                                                            Tmp_CI_Object_Relationship _Tmp_CI_Object_Relationship = new Tmp_CI_Object_Relationship();

                                                            string verifyPlainText4 = string.Empty;
                                                            string verifyHashValue4 = string.Empty;
                                                            string PlainText4 = string.Empty;
                                                            string PlainText5 = string.Empty;

                                                            //重新計算HASH
                                                            verifyPlainText4 = getNeedCheckReviewObjectRelationshipPlainText(item, Configer.VerifyAction, nowUser);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                                            verifyHashValue4 = SF.getHashValue(verifyPlainText4);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值結果:原HASH值[" + item.HashValue + "];重新計算HASH值[" + verifyHashValue4 + "]", log_Info);

                                                            if (item.HashValue == verifyHashValue4){
                                                                //取得物件
                                                                _CI_Object_Relationship = getNeedSaveObjectRelationship(item,Configer.CreateAction, nowUser, false);
                                                                _CI_Object_Relationship1 = getNeedSaveObjectRelationship(item, Configer.CreateAction, nowUser, true);

                                                                //計算HASH值
                                                                PlainText4 = getNeedSaveObjectRelationshiplainText(_CI_Object_Relationship);
                                                                PlainText5 = getNeedSaveObjectRelationshiplainText(_CI_Object_Relationship1);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算物件關係HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                _CI_Object_Relationship.HashValue = SF.getHashValue(PlainText4);
                                                                _CI_Object_Relationship1.HashValue = SF.getHashValue(PlainText5);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算物件關係HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算範本關係HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _CI_Object_Relationship.HashValue + "]", log_Info);

                                                                //新增物件關係
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件關係資料開始@" + SF.getNowDateString(), log_Info);
                                                                //成對新增
                                                                context.CI_Object_Relationship.Add(_CI_Object_Relationship);
                                                                context.CI_Object_Relationship.Add(_CI_Object_Relationship1);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件關係資料結束@" + SF.getNowDateString(), log_Info);

                                                                //修改待覆核屬性
                                                                item.ReviewAccount = nowUser;

                                                                item.ReviewTime = DateTime.Now;

                                                                item.isClose = true;

                                                                //計算HASH值
                                                                verifyPlainText4 = getNeedCheckReviewObjectRelationshipPlainText(item, Configer.CreateAction, nowUser);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算待覆核物件關係HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                item.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算待覆核物件關係HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序計算待覆核物件關係HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + item.HashValue + "]", log_Info);

                                                                context.Entry(item).State = EntityState.Modified;
                                                            }
                                                            else{
                                                                dbContextTransaction.Rollback();
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                                //HASH驗證失敗
                                                                SL.EndTime = DateTime.Now;
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件關係作業失敗，作業類型[" + item.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + item.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]", log_Info);
                                                                SL.TotalCount = 1;
                                                                SL.SuccessCount = 0;
                                                                SL.FailCount = 1;
                                                                SL.Result = false;
                                                                SL.Msg = Configer.ReviewAction + "物件關係作業失敗，作業類型[" + item.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + item.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]";
                                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        //沒有物件關係資料，但不影響覆核
                                                    }

                                                    //物件及物件屬性一起異動
                                                    context.SaveChanges();

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Objects.ReviewAccount = nowUser;

                                                    _Tmp_CI_Objects.ReviewTime = DateTime.Now;

                                                    _Tmp_CI_Objects.isClose = true;

                                                    //計算HASH值
                                                    verifyPlainText = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects, Configer.CreateAction, nowUser);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值開始@" + SF.getNowDateString(), log_Info);
                                                    _Tmp_CI_Objects.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結束@" + SF.getNowDateString(), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料開始@" + SF.getNowDateString(), log_Info);
                                                    context.Entry(_Tmp_CI_Objects).State = EntityState.Modified;
                                                    context.SaveChanges();
                                                    dbContextTransaction.Commit();
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料結束@" + SF.getNowDateString(), log_Info);

                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件作業成功，作業類型[" + _vTmp_CI_Objects_R.Type + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 1;
                                                    SL.FailCount = 0;
                                                    SL.Result = true;
                                                    SL.Msg = Configer.ReviewAction + "物件作業成功，作業類型[" + _vTmp_CI_Objects_R.Type + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    int ReviewCount = context.Tmp_CI_Objects.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                                    if (ReviewCount > 0){
                                                        return RedirectToAction("ReviewIndex", "Object");
                                                    }
                                                    else{
                                                        return RedirectToAction("Index", "Object");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                    //記錄錯誤
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "物件作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                }
                                            }
                                            else{
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                //記錄錯誤
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[無物件屬性對應資料]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[無物件屬性對應資料]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                            }
                                        }
                                        else{
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                            //HASH驗證失敗
                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[物件HASH驗證失敗，原HASH:" + _Tmp_CI_Objects.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _vTmp_CI_Objects_R.Type + "]，異常訊息[物件HASH驗證失敗，原HASH:" + _Tmp_CI_Objects.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                        }
                                    case "編輯":
                                        string verifyPlainText3 = string.Empty;
                                        string verifyHashValue3 = string.Empty;
                                        string PlainText3 = string.Empty;

                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件開始@" + SF.getNowDateString(), log_Info);
                                        _CI_Objects = context.CI_Objects
                                            .Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID)
                                            .First();
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件結束@" + SF.getNowDateString(), log_Info);

                                        if (_CI_Objects != null)
                                        {
                                            //計算HASH值
                                            verifyPlainText3 = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects, Configer.VerifyAction, nowUser);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                            verifyHashValue3 = SF.getHashValue(verifyPlainText3);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Objects.HashValue + "];重新計算HASH[" + verifyHashValue3 + "]", log_Info);

                                            if (verifyHashValue3 == _Tmp_CI_Objects.HashValue){
                                                //取得物件
                                                //_CI_Objects = getNeedSaveObject(_CI_Objects.SN, _Tmp_CI_Objects, Configer.EditAction, nowUser);

                                                _CI_Objects.ObjectName = _Tmp_CI_Objects.ObjectName;
                                                _CI_Objects.Description = _Tmp_CI_Objects.Description;
                                                _CI_Objects.UpdateAccount = nowUser;
                                                _CI_Objects.UpdateTime = DateTime.Now;

                                                //計算HASH值
                                                PlainText3 = getNeedSaveObjectPlainText(_CI_Objects);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值開始@" + SF.getNowDateString(), log_Info);
                                                _CI_Objects.HashValue = SF.getHashValue(PlainText3);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結束@" + SF.getNowDateString(), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結果:明文:[" + PlainText3.ToString() + "];HASH值[" + _CI_Objects.HashValue + "]", log_Info);

                                                context.Entry(_CI_Objects).State = EntityState.Modified;

                                                var query1 = context.Tmp_CI_Object_Data
                                                    .Where(b => b.ObjectID == _CI_Objects.ObjectID)
                                                    .Where(b=>b.isClose==false);

                                                int TmpObjectAttributesCount1 = query1.Count();

                                                if (TmpObjectAttributesCount1 > 0){
                                                    try
                                                    {
                                                        //移除CI_Object_Data
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.RemoveAction + "物件屬性開始@" + SF.getNowDateString(), log_Info);
                                                        context.CI_Object_Data.RemoveRange(context.CI_Object_Data
                                                            .Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID));
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.RemoveAction + "物件屬性結束@" + SF.getNowDateString(), log_Info);

                                                        foreach (var item in query1.ToList())
                                                        {
                                                            CI_Object_Data _CI_Object_Data = new CI_Object_Data();
                                                            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();

                                                            _Tmp_CI_Object_Data = context.Tmp_CI_Object_Data
                                                                .Where(b => b.AttributeID == item.AttributeID)
                                                                .Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID)
                                                                .Where(b=>b.isClose==false)
                                                                .First();

                                                            if (_Tmp_CI_Object_Data != null){
                                                                string verifyPlainText4 = string.Empty;
                                                                string verifyHashValue4 = string.Empty;
                                                                string PlainText4 = string.Empty;

                                                                //計算HASH值
                                                                verifyPlainText4 = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data, Configer.VerifyAction, nowUser);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                verifyHashValue4 = SF.getHashValue(verifyPlainText4);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Object_Data.HashValue + "];重新計算HASH值[" + verifyPlainText4 + "]", log_Info);

                                                                if (verifyHashValue4 == _Tmp_CI_Object_Data.HashValue){
                                                                    //取得物件
                                                                    _CI_Object_Data = getNeedSaveObjectData(_Tmp_CI_Object_Data,Configer.EditAction, nowUser);

                                                                    //計算HASH值
                                                                    PlainText4 = getNeedSaveObjectDataPlainText(_CI_Object_Data);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                    _CI_Object_Data.HashValue = SF.getHashValue(PlainText4);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結果:明文:[" + PlainText4.ToString() + "];HASH值[" + _CI_Object_Data.HashValue + "]", log_Info);

                                                                    //新增範本屬性
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料開始@" + SF.getNowDateString(), log_Info);
                                                                    context.CI_Object_Data.Add(_CI_Object_Data);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);

                                                                    //修改待覆核屬性
                                                                    _Tmp_CI_Object_Data.ReviewAccount = nowUser;

                                                                    _Tmp_CI_Object_Data.ReviewTime = DateTime.Now;

                                                                    _Tmp_CI_Object_Data.isClose = true;

                                                                    //計算HASH值
                                                                    verifyPlainText4 = getNeedCheckReviewObjectDataPlainText(_Tmp_CI_Object_Data, Configer.EditAction, nowUser);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                    _Tmp_CI_Object_Data.HashValue = SF.getHashValue(verifyPlainText4.ToString());
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結果:明文:[" + verifyPlainText4.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                                                                    context.Entry(_Tmp_CI_Object_Data).State = EntityState.Modified;
                                                                }
                                                                else {
                                                                    dbContextTransaction.Rollback();
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                                    //HASH驗證失敗
                                                                    SL.EndTime = DateTime.Now;
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件屬性作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Object_Data.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]", log_Info);
                                                                    SL.TotalCount = 1;
                                                                    SL.SuccessCount = 0;
                                                                    SL.FailCount = 1;
                                                                    SL.Result = false;
                                                                    SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Object_Data.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]";
                                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                    return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                                }
                                                            }
                                                            else {
                                                                dbContextTransaction.Rollback();
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                                //記錄錯誤
                                                                SL.EndTime = DateTime.Now;
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件屬性作業失敗，作業類型[" + _Tmp_CI_Object_Data.Type + "]，異常訊息[系統無此屬性]", log_Info);
                                                                SL.TotalCount = 1;
                                                                SL.SuccessCount = 0;
                                                                SL.FailCount = 1;
                                                                SL.Result = false;
                                                                SL.Msg = Configer.ReviewAction + "範本作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[系統無此屬性]";
                                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                return RedirectToAction("Review", "Profile", new { AttributeID = _vTmp_CI_Objects_R.ProfileID });
                                                            }
                                                        }

                                                        //新增物件關係
                                                        var query2 = context.Tmp_CI_Object_Relationship
                                                            .Where(b => b.ObjectID == _vTmp_CI_Objects_R.ObjectID)
                                                            .Where(b => b.isClose == false);

                                                        //移除CI_Object_Relationship
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件關係子程序-移除原物件關係開始@" + SF.getNowDateString(), log_Info);
                                                        context.CI_Object_Relationship.RemoveRange(context.CI_Object_Relationship
                                                            .Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID));
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件關係子程序-移除原物件關係結束@" + SF.getNowDateString(), log_Info);

                                                        //移除CI_Object_Relationship
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件關係子程序-移除成對物件關係開始@" + SF.getNowDateString(), log_Info);
                                                        context.CI_Object_Relationship.RemoveRange(context.CI_Object_Relationship
                                                            .Where(b => b.RelationshipObjectID == _Tmp_CI_Objects.oObjectID));
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件關係子程序-移除成對物件關係結束@" + SF.getNowDateString(), log_Info);

                                                        int TmpObjectRelationshipCount = query2.Count();

                                                        if (TmpObjectRelationshipCount > 0){
                                                            foreach (var item in query2.ToList())
                                                            {
                                                                //原始物件關係
                                                                CI_Object_Relationship _CI_Object_Relationship = new CI_Object_Relationship();
                                                                //成對的物件關係
                                                                CI_Object_Relationship _CI_Object_Relationship1 = new CI_Object_Relationship();
                                                                Tmp_CI_Object_Relationship _Tmp_CI_Object_Relationship = new Tmp_CI_Object_Relationship();

                                                                string verifyPlainText4 = string.Empty;
                                                                string verifyHashValue4 = string.Empty;
                                                                string PlainText4 = string.Empty;
                                                                string PlainText5 = string.Empty;

                                                                //重新計算HASH
                                                                verifyPlainText4 = getNeedCheckReviewObjectRelationshipPlainText(item, Configer.VerifyAction, nowUser);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                verifyHashValue4 = SF.getHashValue(verifyPlainText4.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-重新計算HASH值結果:原HASH值[" + item.HashValue + "];重新計算HASH值[" + verifyHashValue4 + "]", log_Info);

                                                                if (item.HashValue == verifyHashValue4){
                                                                    //取得物件
                                                                    _CI_Object_Relationship = getNeedSaveObjectRelationship(item, Configer.EditAction, nowUser, false);
                                                                    _CI_Object_Relationship1 = getNeedSaveObjectRelationship(item, Configer.EditAction, nowUser, true);

                                                                    //計算HASH值
                                                                    PlainText4 = getNeedSaveObjectRelationshiplainText(_CI_Object_Relationship);
                                                                    PlainText5 = getNeedSaveObjectRelationshiplainText(_CI_Object_Relationship1);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算物件關係HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                    _CI_Object_Relationship.HashValue = SF.getHashValue(PlainText4);
                                                                    _CI_Object_Relationship1.HashValue = SF.getHashValue(PlainText5);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算物件關係HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算範本關係HASH值結果:明文:[" + PlainText4.ToString() + "];HASH值[" + _CI_Object_Relationship.HashValue + "]", log_Info);

                                                                    //新增物件關係
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件關係資料開始@" + SF.getNowDateString(), log_Info);
                                                                    //成對新增
                                                                    context.CI_Object_Relationship.Add(_CI_Object_Relationship);
                                                                    context.CI_Object_Relationship.Add(_CI_Object_Relationship1);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件關係資料結束@" + SF.getNowDateString(), log_Info);

                                                                    //修改待覆核屬性
                                                                    item.ReviewAccount = nowUser;

                                                                    item.ReviewTime = DateTime.Now;

                                                                    item.isClose = true;

                                                                    verifyPlainText4 = getNeedCheckReviewObjectRelationshipPlainText(item, Configer.EditAction, nowUser);

                                                                    //計算HASH值
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算待覆核物件關係HASH值開始@" + SF.getNowDateString(), log_Info);
                                                                    item.HashValue = SF.getHashValue(verifyPlainText4);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序-計算待覆核物件關係HASH值結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件關係作業子程序計算待覆核物件關係HASH值結果:明文:[" + verifyPlainText4.ToString() + "];HASH值[" + item.HashValue + "]", log_Info);

                                                                    context.Entry(item).State = EntityState.Modified;
                                                                }
                                                                else {
                                                                    dbContextTransaction.Rollback();
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                                    //HASH驗證失敗
                                                                    SL.EndTime = DateTime.Now;
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                    SF.logandshowInfo(Configer.ReviewAction + "物件關係作業失敗，作業類型[" + item.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + item.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]", log_Info);
                                                                    SL.TotalCount = 1;
                                                                    SL.SuccessCount = 0;
                                                                    SL.FailCount = 1;
                                                                    SL.Result = false;
                                                                    SL.Msg = Configer.ReviewAction + "物件關係作業失敗，作業類型[" + item.Type + "]，異常訊息[物件屬性HASH驗證失敗，原HASH:" + item.HashValue + "；重新計算HASH:" + verifyHashValue4 + "]";
                                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                                    return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            //沒有物件關係資料，執行刪除，但一開始已經刪除所以不用動作
                                                        }

                                                        //修改待覆核屬性
                                                        _Tmp_CI_Objects.ReviewAccount = nowUser;

                                                        _Tmp_CI_Objects.ReviewTime = DateTime.Now;

                                                        _Tmp_CI_Objects.isClose = true;

                                                        //計算HASH值
                                                        verifyPlainText3 = getNeedCheckReviewObjectPlainText(_Tmp_CI_Objects, Configer.EditAction, nowUser);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值開始@" + SF.getNowDateString(), log_Info);
                                                        _Tmp_CI_Objects.HashValue = SF.getHashValue(verifyPlainText3);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結束@" + SF.getNowDateString(), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結果:明文:[" + verifyPlainText3.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料開始@" + SF.getNowDateString(), log_Info);
                                                        context.Entry(_Tmp_CI_Objects).State = EntityState.Modified;
                                                        context.SaveChanges();
                                                        dbContextTransaction.Commit();
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料結束@" + SF.getNowDateString(), log_Info);

                                                        SL.EndTime = DateTime.Now;
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件作業成功，作業類型[" + _vTmp_CI_Objects_R.Type + "]", log_Info);
                                                        SL.TotalCount = 1;
                                                        SL.SuccessCount = 1;
                                                        SL.FailCount = 0;
                                                        SL.Result = true;
                                                        SL.Msg = Configer.ReviewAction + "物件作業成功，作業類型[" + _Tmp_CI_Objects.Type + "]";
                                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                        int ReviewCount = context.Tmp_CI_Objects
                                                            .Where(b => b.CreateAccount != nowUser)
                                                            .Where(b => b.isClose == false)
                                                            .Count();

                                                        if (ReviewCount > 0){
                                                            return RedirectToAction("ReviewIndex", "Object");
                                                        }
                                                        else {
                                                            return RedirectToAction("Index", "Object");
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        dbContextTransaction.Rollback();
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                        //記錄錯誤
                                                        SL.EndTime = DateTime.Now;
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                                        SL.TotalCount = 1;
                                                        SL.SuccessCount = 0;
                                                        SL.FailCount = 1;
                                                        SL.Result = false;
                                                        SL.Msg = Configer.ReviewAction + "物件作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                        return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                                    }

                                                }
                                                else {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + SF.getNowDateString(), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                    //記錄錯誤
                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[無物件屬性對應資料]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 0;
                                                    SL.FailCount = 1;
                                                    SL.Result = false;
                                                    SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[無物件屬性對應資料]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    return RedirectToAction("Review", "Object", new { ProfileID = _vTmp_CI_Objects_R.ProfileID });
                                                }
                                            }
                                            else {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                                //HASH驗證失敗
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[物件HASH驗證失敗，原HASH:" + _Tmp_CI_Objects.HashValue + "；重新計算HASH:" + verifyHashValue3 + "]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Objects.HashValue + "；重新計算HASH:" + verifyHashValue3 + "]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "Object", new { ObjectID = _Tmp_CI_Objects.ObjectID });
                                            }

                                        }
                                        else {
                                            //記錄錯誤
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[查無原始物件資料]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _vTmp_CI_Objects_R.Type + "]，異常訊息[查無原始物件資料]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Object", new { ObjectID = _vTmp_CI_Objects_R.ObjectID });
                                        }
                                    default:
                                        //記錄錯誤
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + SF.getNowDateString(), log_Info);

                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[系統不存在的作業類型]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[系統不存在的作業類型]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "Object", new { ObjectID = _vTmp_CI_Objects_R.ObjectID });
                                }
                            }
                        }
                        else{
                            //記錄錯誤
                            SL.EndTime = DateTime.Now;
                            SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，作業類型[" + _Tmp_CI_Objects.Type + "]，異常訊息[查無待覆核物件資料]", log_Info);
                            SL.TotalCount = 1;
                            SL.SuccessCount = 0;
                            SL.FailCount = 1;
                            SL.Result = false;
                            SL.Msg = Configer.ReviewAction + "物件作業失敗，作業類型[" + _vTmp_CI_Objects_R.Type + "]，異常訊息[查無待覆核物件資料]";
                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                            return RedirectToAction("Review", "Object", new { ObjectID = _vTmp_CI_Objects_R.ObjectID });
                        }
                    }
                    else{
                        //記錄錯誤
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，異常訊息[資料驗證失敗]", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.ReviewAction + "物件作業失敗，異常訊息[資料驗證失敗]";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Review", "Object", new { ObjectID = _vTmp_CI_Objects_R.ObjectID });
                    }
                }
                catch (Exception ex)
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.ReviewAction + "物件作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.ReviewAction + "物件作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    //TempData["CreateMsg"] = "<script>alert('覆核屬性發生異常');</script>";

                    return RedirectToAction("Review", "Object", new { ObjectID = _vTmp_CI_Objects_R.ObjectID });
                }
            }
        }

        /// <summary>
        /// 取得套用範本屬性資料
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        public JsonResult getAttributeInputList(int ProfileID)
        {
            var AttributeList = from Pro in context.CI_Proflie_Attributes.Where(b => b.ProfileID == ProfileID)
                                join Att in context.CI_Attributes on Pro.AttributeID equals Att.AttributeID
                                join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                                orderby (Pro.AttributeOrder)
                                select new { Att.AttributeID, Att.AttributeName, AttType.AttributeTypeID, AttType.AttributeTypeName, Att.DropDownValues, Att.AllowMutiValue };

            if (AttributeList != null)
            {
                return Json(AttributeList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得範本關係菜單
        /// </summary>
        /// <param name="ProfileID">目前範本ID</param>
        /// <returns>關聯範本集合</returns>
        public JsonResult getRelationshipProfileMenu(int ProfileID)
        {
            var RelationshipProfileMenu = from ProR in context.CI_Profile_Relationship.Where(b => b.ProfileID == ProfileID)
                                          join Pro in context.CI_Proflies on ProR.RelationshipProfileID equals Pro.ProfileID
                                          select new { ProR.RelationshipProfileID, Pro.ProfileName };

            if (RelationshipProfileMenu != null)
            {
                return Json(RelationshipProfileMenu, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 使用物件名稱查詢套用特定ProfileID下物件資料
        /// </summary>
        /// <param name="ProfileID">ProfileID</param>
        /// <param name="ObjectName">物件名稱(模糊查詢)</param>
        /// <returns></returns>
        public JsonResult getObjectDatafromName(int ProfileID, string ObjectName)
        {
            var query = from Obj in context.CI_Objects
                         .Where(b => b.ProfileID == ProfileID)
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            Description = Obj.Description,
                            ProfileID = Obj.ProfileID,
                            ProfileName = Pro.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (string.IsNullOrEmpty(ObjectName) == false)
            {
                query = query.Where(b => b.ObjectName.Contains(ObjectName));
            }

            if (query.Count() > 0)
            {
                return Json(query);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得待覆核物件資料
        /// </summary>
        /// <param name="ObjectID">物件ID</param>
        /// <returns></returns>
        private vTmp_CI_Objects_R getReviewData(int ObjectID)
        {
            vTmp_CI_Objects_R _vTmp_CI_Objects_R = new vTmp_CI_Objects_R();
            vCI_Objects _vCI_Objects = new vCI_Objects();
            var query = from Obj in context.Tmp_CI_Objects
                        .Where(b => b.ObjectID == ObjectID)
                        .Where(b => b.isClose == false)
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                         into y
                        from x in y.DefaultIfEmpty()
                        select new vTmp_CI_Objects_R
                        {
                            ObjectID = Obj.ObjectID,
                            oObjectID = Obj.oObjectID,
                            ObjectName = Obj.ObjectName,
                            Description = Obj.Description,
                            ProfileID = Obj.ProfileID,
                            ProfileName = Pro.ProfileName,
                            Creator = x.Name,
                            CreateTime = Pro.CreateTime,
                            Type = Obj.Type
                        };

            if (query.Count() == 1)
            {
                _vTmp_CI_Objects_R = query.First();
                
                _vTmp_CI_Objects_R.ObjectRelationshipData = SF.getReviewObjectRelationshipData(ObjectID, _vTmp_CI_Objects_R.Type);

                if (_vTmp_CI_Objects_R.Type == Configer.CreateAction)
                {
                    _vTmp_CI_Objects_R.AttributesData = SF.getReviewObjectAttributesData(_vTmp_CI_Objects_R.ObjectID);
                    return _vTmp_CI_Objects_R;
                }
                else if (_vTmp_CI_Objects_R.Type == Configer.EditAction || _vTmp_CI_Objects_R.Type == Configer.RemoveAction)
                {
                    _vTmp_CI_Objects_R.AttributesData = SF.getReviewObjectAttributesData(_vTmp_CI_Objects_R.oObjectID);
                    _vCI_Objects = getObjectData(_vTmp_CI_Objects_R.oObjectID);

                    if (_vCI_Objects != null)
                    {
                        _vTmp_CI_Objects_R.oObjectID = _vTmp_CI_Objects_R.oObjectID;
                        _vTmp_CI_Objects_R.oObjectName = _vCI_Objects.ObjectName;
                        _vTmp_CI_Objects_R.oDescription = _vCI_Objects.Description;
                        _vTmp_CI_Objects_R.oProfileID = _vCI_Objects.ProfileID;
                        _vTmp_CI_Objects_R.oProfileName = _vCI_Objects.ProfileName;
                        _vTmp_CI_Objects_R.oAttributesData = SF.getObjectAttributesData(_vCI_Objects.ObjectID);
                        _vTmp_CI_Objects_R.oObjectRelationshipData = SF.getObjectRelationshipData(_vCI_Objects.ObjectID);
                        _vTmp_CI_Objects_R.oUpadter = _vCI_Objects.Upadter;
                        _vTmp_CI_Objects_R.oUpdateTime = _vCI_Objects.UpdateTime;

                        return _vTmp_CI_Objects_R;
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
        /// 取得原物件資料
        /// </summary>
        /// <param name="ObjectID">物件ID</param>
        /// <returns></returns>
        private vCI_Objects getObjectData(int ObjectID)
        {
            vCI_Objects _vCI_Objects = new vCI_Objects();

            var query = from Obj in context.CI_Objects
                        .Where(b => b.ObjectID == ObjectID)
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Pro.CreateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            Description = Obj.Description,
                            ProfileID = Obj.ProfileID,
                            ProfileName = Pro.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (query.Count() == 1)
            {
                _vCI_Objects = query.First();
                _vCI_Objects.AttributesData = SF.getObjectAttributesData(ObjectID);

                return _vCI_Objects;
            }
            else
            {
                return null;
            }
        }

        public string getNeedCheckReviewObjectPlainText(Tmp_CI_Objects O, string Type, string nowUser)
        {
            StringBuilder verifyPlainText = new StringBuilder();
            if (Type == "建立"){
                verifyPlainText.Append(O.ObjectID + Configer.SplitSymbol);
            }
            else {

                if (O.Type == "建立"){
                    verifyPlainText.Append("0" + Configer.SplitSymbol);
                }
                else {
                    verifyPlainText.Append(O.oObjectID + Configer.SplitSymbol);
                }
              
            }

            verifyPlainText.Append(O.ObjectName + Configer.SplitSymbol);

            verifyPlainText.Append(O.ProfileID.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.Description + Configer.SplitSymbol);

            verifyPlainText.Append(O.CreateAccount + Configer.SplitSymbol);

            verifyPlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.Type);

            if (Type == "驗證"){
                verifyPlainText.Append(Configer.SplitSymbol + O.isClose.ToString());
            }
            else if (Type == "編輯" || Type == "建立"){
                O.ReviewAccount = nowUser;
                verifyPlainText.Append(Configer.SplitSymbol + O.ReviewAccount + Configer.SplitSymbol);

                O.ReviewTime = DateTime.Now;
                verifyPlainText.Append(O.ReviewTime.ToString() + Configer.SplitSymbol);

                O.isClose = true;
                verifyPlainText.Append(O.isClose.ToString());
            }

            return verifyPlainText.ToString();
        }

        public CI_Objects getNeedSaveObject(int SN,Tmp_CI_Objects O, string Type, string nowUser)
        {
            CI_Objects NewO = new CI_Objects();

            if (Type == "建立"){
                NewO.ObjectID = O.ObjectID;
            }
            else {
                NewO.SN = SN;
                NewO.ObjectID = O.oObjectID;
            }

            NewO.ObjectName = O.ObjectName;

            NewO.Description = O.Description;

            NewO.ProfileID = O.ProfileID;

            NewO.CreateAccount = O.CreateAccount;

            NewO.CreateTime = O.CreateTime;

            NewO.UpdateAccount = nowUser;

            NewO.UpdateTime = DateTime.Now;

            return NewO;
        }

        public string getNeedSaveObjectPlainText(CI_Objects O)
        {
            StringBuilder PlainText = new StringBuilder();

            PlainText.Append(O.ObjectName + Configer.SplitSymbol);
            PlainText.Append(O.Description + Configer.SplitSymbol);
            PlainText.Append(O.ProfileID.ToString() + Configer.SplitSymbol);
            PlainText.Append(O.CreateAccount + Configer.SplitSymbol);
            PlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);
            PlainText.Append(O.UpdateAccount + Configer.SplitSymbol);
            PlainText.Append(O.UpdateTime.ToString());

            return PlainText.ToString();
        }

        public string getNeedCheckReviewObjectDataPlainText(Tmp_CI_Object_Data O, string Type, string nowUser)
        {
            StringBuilder verifyPlainText = new StringBuilder();

            verifyPlainText.Append(O.ObjectID.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.AttributeID.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.AttributeValue + Configer.SplitSymbol);
         
            verifyPlainText.Append(O.AttributeOrder.ToString() + Configer.SplitSymbol);
           
            verifyPlainText.Append(O.CreateAccount + Configer.SplitSymbol);
            
            verifyPlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);
      
            verifyPlainText.Append(O.Type);

            if (Type == "驗證"){
                verifyPlainText.Append(Configer.SplitSymbol + O.isClose.ToString());
            }
            else if (Type == "編輯" || Type == "建立"){
                O.ReviewAccount = nowUser;
                verifyPlainText.Append(Configer.SplitSymbol + O.ReviewAccount + Configer.SplitSymbol);

                O.ReviewTime = DateTime.Now;
                verifyPlainText.Append(O.ReviewTime.ToString() + Configer.SplitSymbol);

                O.isClose = true;
                verifyPlainText.Append(O.isClose.ToString());
            }

            return verifyPlainText.ToString();
        }

        public CI_Object_Data getNeedSaveObjectData(Tmp_CI_Object_Data O, string Type, string nowUser)
        {
            CI_Object_Data NewO = new CI_Object_Data();

            if (Type == "建立")
            {
                NewO.ObjectID = O.ObjectID;
            }
            else {
                NewO.ObjectID = O.ObjectID;
            }

            NewO.ObjectID = O.ObjectID;

            NewO.AttributeID = O.AttributeID;

            NewO.AttributeValue = O.AttributeValue;

            NewO.AttributeOrder = O.AttributeOrder;

            NewO.CreateAccount = O.CreateAccount;

            NewO.CreateTime = O.CreateTime;

            NewO.UpdateAccount = nowUser;

            NewO.UpdateTime = DateTime.Now;

            return NewO;
        }

        public string getNeedSaveObjectDataPlainText(CI_Object_Data O)
        {
            StringBuilder PlainText = new StringBuilder();

            PlainText.Append(O.ObjectID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.AttributeID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.AttributeValue + Configer.SplitSymbol);

            PlainText.Append(O.AttributeOrder.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.CreateAccount + Configer.SplitSymbol);

            PlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.UpdateAccount + Configer.SplitSymbol);

            PlainText.Append(O.UpdateTime.ToString());

            return PlainText.ToString();
        }

        public string getNeedCheckReviewObjectRelationshipPlainText(Tmp_CI_Object_Relationship O,string Type,string nowUser)
        {
            StringBuilder verifyPlainText = new StringBuilder();

            verifyPlainText.Append(O.ObjectID.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.RelationshipObjectID.ToString() + Configer.SplitSymbol);
            
            verifyPlainText.Append(O.ProfileID.ToString() + Configer.SplitSymbol);
           
            verifyPlainText.Append(O.RelationshipProfileID.ToString() + Configer.SplitSymbol);
           
            verifyPlainText.Append(O.CreateAccount + Configer.SplitSymbol);
           
            verifyPlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);

            verifyPlainText.Append(O.Type);

            if (Type == "驗證"){
                verifyPlainText.Append(Configer.SplitSymbol + O.isClose.ToString());
            }
            else if (Type == "編輯" || Type == "建立"){
                O.ReviewAccount = nowUser;
                verifyPlainText.Append(Configer.SplitSymbol + O.ReviewAccount + Configer.SplitSymbol);

                O.ReviewTime = DateTime.Now;
                verifyPlainText.Append(O.ReviewTime.ToString() + Configer.SplitSymbol);

                O.isClose = true;
                verifyPlainText.Append(O.isClose.ToString());
            }

            return verifyPlainText.ToString();
        }

        public CI_Object_Relationship getNeedSaveObjectRelationship(Tmp_CI_Object_Relationship O, string Type, string nowUser,bool Reverse)
        {
            CI_Object_Relationship NewO = new CI_Object_Relationship();

            if (Reverse){
                NewO.ObjectID = O.RelationshipObjectID;

                if (Type == "建立"){
                    NewO.RelationshipObjectID = O.ObjectID;
                }
                else {
                    NewO.RelationshipObjectID = O.oObjectID;
                }
            }
            else {
                if (Type == "建立"){
                    NewO.ObjectID = O.ObjectID;
                }
                else {
                    NewO.ObjectID = O.oObjectID;
                }
                NewO.RelationshipObjectID = O.RelationshipObjectID;
            }
            NewO.ProfileID = O.ProfileID;

            NewO.RelationshipProfileID = O.RelationshipProfileID;

            NewO.CreateAccount = O.CreateAccount;

            NewO.CreateTime = O.CreateTime;

            NewO.UpdateAccount = nowUser;

            NewO.UpdateTime = DateTime.Now;

            return NewO;
        }

        public string getNeedSaveObjectRelationshiplainText(CI_Object_Relationship O)
        {
            StringBuilder PlainText = new StringBuilder();

            PlainText.Append(O.ObjectID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.RelationshipObjectID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.ProfileID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.RelationshipProfileID.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.CreateAccount + Configer.SplitSymbol);

            PlainText.Append(O.CreateTime.ToString() + Configer.SplitSymbol);

            PlainText.Append(O.UpdateAccount + Configer.SplitSymbol);

            PlainText.Append(O.UpdateTime.ToString());

            return PlainText.ToString();
        }

        public Tmp_CI_Objects getNeedSaveTmpObject(vCI_Objects_CU vObjCU, string Type, string nowUser)
        {
            Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();

            if (Type == "建立"){

            }
            else {
                _Tmp_CI_Objects.oObjectID = vObjCU.ObjectID;
            }
       
            _Tmp_CI_Objects.ObjectName = vObjCU.ObjectName;

            _Tmp_CI_Objects.ProfileID = vObjCU.ProfileID;

            _Tmp_CI_Objects.Description = vObjCU.Description;

            _Tmp_CI_Objects.CreateAccount = nowUser;

            _Tmp_CI_Objects.CreateTime = DateTime.Now;

            _Tmp_CI_Objects.Type = Type;

            _Tmp_CI_Objects.isClose = false;

            return _Tmp_CI_Objects;
        }

        public Tmp_CI_Object_Data getNeedSaveTmpObjectData(int ObjectID, vCI_Attributes item,string Type,string nowUser )
        {
            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();

            _Tmp_CI_Object_Data.ObjectID = ObjectID;
            
            _Tmp_CI_Object_Data.AttributeID = item.AttributeID;

            //如果沒填值，給預設值
            if (string.IsNullOrEmpty(item.AttributeValue)){
                _Tmp_CI_Object_Data.AttributeValue = "N/A";
            }
            else{
                _Tmp_CI_Object_Data.AttributeValue = item.AttributeValue;
            }

            _Tmp_CI_Object_Data.AttributeOrder = item.AttributeOrder;
            
            _Tmp_CI_Object_Data.CreateAccount = nowUser;
       
            _Tmp_CI_Object_Data.CreateTime = DateTime.Now;
         
            _Tmp_CI_Object_Data.Type = Type;
       
            _Tmp_CI_Object_Data.isClose = false;

            return _Tmp_CI_Object_Data;

        }

        public Tmp_CI_Object_Relationship getNeedSaveTmpObjectRelationship(vCI_Objects_CU vObjCU, int ObjectID, int item, string Type,string nowUser)
        {
            Tmp_CI_Object_Relationship _Tmp_CI_Object_Relationship = new Tmp_CI_Object_Relationship();
            _Tmp_CI_Object_Relationship.ObjectID = ObjectID;

            _Tmp_CI_Object_Relationship.oObjectID = vObjCU.ObjectID;

            _Tmp_CI_Object_Relationship.RelationshipObjectID = item;

            _Tmp_CI_Object_Relationship.ProfileID = vObjCU.ProfileID;

            _Tmp_CI_Object_Relationship.RelationshipProfileID = vObjCU.ObjectRelationshipData.RelationshipProfileID;

            _Tmp_CI_Object_Relationship.CreateAccount = nowUser;

            _Tmp_CI_Object_Relationship.CreateTime = DateTime.Now;

            _Tmp_CI_Object_Relationship.Type = Type;

            _Tmp_CI_Object_Relationship.isClose = false;;

            return _Tmp_CI_Object_Relationship;
        }
    }
}