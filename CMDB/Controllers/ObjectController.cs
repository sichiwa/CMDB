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
                vCI_Objects_list vObjList = new vCI_Objects_list();
                SF.logandshowInfo(Configer.GetAction + "物件清單-子程序-" + Configer.GetAction + "待覆核物件資料筆數開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vObjList.ReviewCount = context.Tmp_CI_Objects.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序" + Configer.GetAction + "待覆核物件資料筆數結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "待覆核物件資料結果:共取得[" + vObjList.ReviewCount.ToString() + "]筆", log_Info);

                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vObjList.ObjectsData = SF.getObjectsData();
                SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                        SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        item.EditAccount = SF.canEdit("CI_Objects", item.ObjectID.ToString(), "");
                        SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
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
        public ActionResult Create()
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

                SF.logandshowInfo(Configer.CreateAction + "新增物件表單子程序-" + Configer.GetAction + "範本類型下拉式選單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vObjCU.Profile = SF.getProfileList(0);
                SF.logandshowInfo(Configer.CreateAction + "新增物件表單子程序-" + Configer.GetAction + "範本類型下拉式選單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                    if (ModelState.IsValid)
                    {
                        StringBuilder PlainText = new StringBuilder();

                        Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();
                        _Tmp_CI_Objects.ObjectName = vObjCU.ObjectName;
                        PlainText.Append(_Tmp_CI_Objects.ObjectName + Configer.SplitSymbol);

                        _Tmp_CI_Objects.Description = vObjCU.Description;
                        PlainText.Append(_Tmp_CI_Objects.Description + Configer.SplitSymbol);

                        _Tmp_CI_Objects.ProfileID = vObjCU.ProfileID;
                        PlainText.Append(_Tmp_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Objects.CreateAccount = nowUser;
                        PlainText.Append(_Tmp_CI_Objects.CreateAccount + Configer.SplitSymbol);

                        _Tmp_CI_Objects.CreateTime = DateTime.Now;
                        PlainText.Append(_Tmp_CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Objects.Type = Configer.CreateAction;
                        PlainText.Append(_Tmp_CI_Objects.Type + Configer.SplitSymbol);

                        _Tmp_CI_Objects.isClose = false;
                        PlainText.Append(_Tmp_CI_Objects.isClose.ToString());

                        //計算HASH值
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        _Tmp_CI_Objects.HashValue = SF.getHashValue(PlainText.ToString());
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        context.Tmp_CI_Objects.Add(_Tmp_CI_Objects);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        StringProcessor.Claersb(PlainText);

                        foreach (var item in vObjCU.AttributesData)
                        {
                            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();
                            _Tmp_CI_Object_Data.ObjectID = _Tmp_CI_Objects.ObjectID;
                            PlainText.Append(_Tmp_CI_Object_Data.ObjectID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeID = item.AttributeID;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeValue = item.AttributeValue;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeValue + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeOrder = item.AttributeOrder;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.CreateAccount = nowUser;
                            PlainText.Append(_Tmp_CI_Object_Data.CreateAccount + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.CreateTime = DateTime.Now;
                            PlainText.Append(_Tmp_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.Type = Configer.CreateAction;
                            PlainText.Append(_Tmp_CI_Object_Data.Type + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.isClose = false;
                            PlainText.Append(_Tmp_CI_Object_Data.isClose);

                            //計算HASH值
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            _Tmp_CI_Object_Data.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);


                            StringProcessor.Claersb(PlainText);
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            context.Tmp_CI_Object_Data.Add(_Tmp_CI_Object_Data);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-新增屬性子程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
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
                    else
                    {
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

                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單-子程序" + Configer.GetAction + "物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _CI_Objects = context.CI_Objects.Where(b => b.ObjectID == ObjectID).First();
                SF.logandshowInfo(Configer.CreateAction + "編輯物件表單-子程序" + Configer.GetAction + "物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_CI_Objects != null)
                {
                    vObjCU.ObjectID = _CI_Objects.ObjectID;
                    vObjCU.ObjectName = _CI_Objects.ObjectName;
                    vObjCU.Description = _CI_Objects.Description;
                    vObjCU.ProfileID = _CI_Objects.ProfileID;
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    vObjCU.Profile = SF.getProfileList(ObjectID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "範本下拉式選單結果:共取得[" + vObjCU.Profile.Count() + " ]筆資料", log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    vObjCU.AttributesData = SF.getObjectAttributesData(ObjectID);
                    SF.logandshowInfo(Configer.CreateAction + "編輯範本表單-子程序" + Configer.GetAction + "屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                    if (ModelState.IsValid)
                    {
                        StringBuilder PlainText = new StringBuilder();

                        Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();
                        _Tmp_CI_Objects.oObjectID = vObjCU.ObjectID;
                        PlainText.Append(_Tmp_CI_Objects.oObjectID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Objects.ObjectName = vObjCU.ObjectName;
                        PlainText.Append(_Tmp_CI_Objects.ObjectName + Configer.SplitSymbol);

                        _Tmp_CI_Objects.ProfileID = vObjCU.ProfileID;
                        PlainText.Append(_Tmp_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Objects.Description = vObjCU.Description;
                        PlainText.Append(_Tmp_CI_Objects.Description + Configer.SplitSymbol);

                        _Tmp_CI_Objects.CreateAccount = nowUser;
                        PlainText.Append(_Tmp_CI_Objects.CreateAccount + Configer.SplitSymbol);

                        _Tmp_CI_Objects.CreateTime = DateTime.Now;
                        PlainText.Append(_Tmp_CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);

                        _Tmp_CI_Objects.Type = Configer.EditAction;
                        PlainText.Append(_Tmp_CI_Objects.Type + Configer.SplitSymbol);

                        _Tmp_CI_Objects.isClose = false;
                        PlainText.Append(_Tmp_CI_Objects.isClose.ToString());

                        //計算HASH值
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        _Tmp_CI_Objects.HashValue = SF.getHashValue(PlainText.ToString());
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                        SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        context.Tmp_CI_Objects.Add(_Tmp_CI_Objects);
                        context.SaveChanges();
                        SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                        StringProcessor.Claersb(PlainText);

                        foreach (var item in vObjCU.AttributesData)
                        {
                            Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();
                            _Tmp_CI_Object_Data.ObjectID = _Tmp_CI_Objects.ObjectID;
                            PlainText.Append(_Tmp_CI_Object_Data.ObjectID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeID = item.AttributeID;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeValue = item.AttributeValue;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeValue + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.AttributeOrder = item.AttributeOrder;
                            PlainText.Append(_Tmp_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.CreateAccount = nowUser;
                            PlainText.Append(_Tmp_CI_Object_Data.CreateAccount + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.CreateTime = DateTime.Now;
                            PlainText.Append(_Tmp_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.Type = Configer.EditAction;
                            PlainText.Append(_Tmp_CI_Object_Data.Type + Configer.SplitSymbol);

                            _Tmp_CI_Object_Data.isClose = false;
                            PlainText.Append(_Tmp_CI_Object_Data.isClose.ToString());

                            //計算HASH值
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            _Tmp_CI_Object_Data.HashValue = SF.getHashValue(PlainText.ToString());
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            SF.logandshowInfo(Configer.EditAction + "物件子程序-" + Configer.EditAction + "屬性子程序-計算HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                            StringProcessor.Claersb(PlainText);


                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-" + Configer.EditAction + "屬性子程序-"+ Configer.EditAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                            context.Tmp_CI_Object_Data.Add(_Tmp_CI_Object_Data);
                            context.SaveChanges();
                            SF.logandshowInfo(Configer.CreateAction + "物件子程序-" + Configer.EditAction + "屬性子程序-" + Configer.EditAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
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
                    else
                    {
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
                                ObjectID= Obj.ObjectID,
                                ObjectName= Obj.ObjectName,
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
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料子程序-" + Configer.GetAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _vTmp_CI_Objects_R = getReviewData(ObjectID);
                SF.logandshowInfo(Configer.GetAction + "待覆核物件資料子程序-" + Configer.GetAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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

            try
            {
                if (ModelState.IsValid)
                {
                    CI_Objects _CI_Objects = new CI_Objects();
                    Tmp_CI_Objects _Tmp_CI_Objects = new Tmp_CI_Objects();

                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.GetAction + "待覆核物件開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    _Tmp_CI_Objects = context.Tmp_CI_Objects.Find(_vTmp_CI_Objects_R.ObjectID);
                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.GetAction + "待覆核物件結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (_Tmp_CI_Objects != null)
                    {
                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件結果:共取得[1]筆", log_Info);
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            switch (_Tmp_CI_Objects.Type)
                            {
                                case "建立":
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    StringBuilder verifyPlainText = new StringBuilder();
                                    string verifyHashValue = string.Empty;
                                    StringBuilder PlainText = new StringBuilder();

                                    _CI_Objects.ObjectID = _Tmp_CI_Objects.ObjectID;

                                    _CI_Objects.ObjectName = _Tmp_CI_Objects.ObjectName;
                                    verifyPlainText.Append(_Tmp_CI_Objects.ObjectName + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Objects.ObjectName + Configer.SplitSymbol);

                                    _CI_Objects.Description = _Tmp_CI_Objects.Description;
                                    verifyPlainText.Append(_Tmp_CI_Objects.Description + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Objects.Description + Configer.SplitSymbol);

                                    _CI_Objects.ProfileID = _Tmp_CI_Objects.ProfileID;
                                    verifyPlainText.Append(_Tmp_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);

                                    _CI_Objects.CreateAccount = _Tmp_CI_Objects.CreateAccount;
                                    verifyPlainText.Append(_Tmp_CI_Objects.CreateAccount + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Objects.CreateAccount + Configer.SplitSymbol);

                                    _CI_Objects.CreateTime = _Tmp_CI_Objects.CreateTime;
                                    verifyPlainText.Append(_Tmp_CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);
                                    PlainText.Append(_CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);

                                    verifyPlainText.Append(_Tmp_CI_Objects.Type + Configer.SplitSymbol);
                                    verifyPlainText.Append(_Tmp_CI_Objects.isClose.ToString());

                                    //重新計算HASH
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    verifyPlainText.Replace("False", "");
                                    verifyPlainText.Remove(verifyPlainText.Length - 1, 1);
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "範本作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Objects.HashValue + "];重新計算HASH值[" + verifyHashValue + "]", log_Info);

                                    //與原HASH相比
                                    if (verifyHashValue == _Tmp_CI_Objects.HashValue)
                                    {
                                        _CI_Objects.UpdateAccount = nowUser;
                                        PlainText.Append(_CI_Objects.UpdateAccount+Configer.SplitSymbol);

                                        _CI_Objects.UpdateTime = DateTime.Now;
                                        PlainText.Append(_CI_Objects.UpdateTime.ToString() + Configer.SplitSymbol);

                                        //計算HASH值
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        _CI_Objects.HashValue = SF.getHashValue(PlainText.ToString());
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結果:明文:[" + PlainText.ToString() + "];HASH值[" + _CI_Objects.HashValue + "]", log_Info);

                                        //新增物件
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        context.CI_Objects.Add(_CI_Objects);
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        var query = context.Tmp_CI_Object_Data.Where(b => b.ObjectID == _vTmp_CI_Objects_R.ObjectID);

                                        int TmpObjectAttributesCount = query.Count();

                                        if (TmpObjectAttributesCount > 0)
                                        {
                                            try
                                            {
                                                foreach (var item in query.ToList())
                                                {
                                                    CI_Object_Data _CI_Object_Data = new CI_Object_Data();
                                                    Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();

                                                    _Tmp_CI_Object_Data = context.Tmp_CI_Object_Data.Where(b => b.AttributeID == item.AttributeID)
                                                         .Where(b => b.ObjectID == _Tmp_CI_Objects.ObjectID).First();

                                                    if (_Tmp_CI_Object_Data != null)
                                                    {
                                                        StringBuilder verifyPlainText2 = new StringBuilder();
                                                        string verifyHashValue2 = string.Empty;
                                                        StringBuilder PlainText2 = new StringBuilder();

                                                        _CI_Object_Data.ObjectID = _Tmp_CI_Object_Data.ObjectID;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.ObjectID.ToString()+Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.ObjectID.ToString() + Configer.SplitSymbol);

                                                        _CI_Object_Data.AttributeID = _Tmp_CI_Object_Data.AttributeID;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);

                                                        _CI_Object_Data.AttributeValue = _Tmp_CI_Object_Data.AttributeValue;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.AttributeValue+ Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.AttributeValue + Configer.SplitSymbol);

                                                        _CI_Object_Data.AttributeOrder = _Tmp_CI_Object_Data.AttributeOrder;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                        _CI_Object_Data.CreateAccount = _Tmp_CI_Object_Data.CreateAccount;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.CreateAccount + Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.CreateAccount + Configer.SplitSymbol);

                                                        _CI_Object_Data.CreateTime = _Tmp_CI_Object_Data.CreateTime;
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);
                                                        PlainText2.Append(_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);

                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.Type + Configer.SplitSymbol);
                                                        verifyPlainText2.Append(_Tmp_CI_Object_Data.isClose.ToString());

                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        verifyHashValue2 = SF.getHashValue(verifyPlainText2.ToString());
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Object_Data.HashValue + "];重新計算HASH值[" + verifyHashValue2 + "]", log_Info);

                                                        verifyPlainText2.Replace("False", "");
                                                        verifyPlainText2.Remove(verifyPlainText2.Length - 1, 1);

                                                        if (verifyHashValue2 == _Tmp_CI_Object_Data.HashValue)
                                                        {
                                                            _CI_Object_Data.UpdateAccount = nowUser;
                                                            PlainText2.Append(_CI_Object_Data.UpdateAccount + Configer.SplitSymbol);

                                                            _CI_Object_Data.UpdateTime = DateTime.Now;
                                                            PlainText2.Append(_CI_Object_Data.UpdateTime.ToString());

                                                            //計算HASH值
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            _CI_Object_Data.HashValue = SF.getHashValue(PlainText2.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算範本屬性HASH值結果:明文:[" + PlainText2.ToString() + "];HASH值[" + _CI_Object_Data.HashValue + "]", log_Info);

                                                            //新增物件屬性
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            context.CI_Object_Data.Add(_CI_Object_Data);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                            //修改待覆核屬性
                                                            _Tmp_CI_Object_Data.ReviewAccount = nowUser;
                                                            verifyPlainText2.Append(Configer.SplitSymbol + _Tmp_CI_Object_Data.ReviewAccount + Configer.SplitSymbol);

                                                            _Tmp_CI_Object_Data.ReviewTime = DateTime.Now;
                                                            verifyPlainText2.Append(_Tmp_CI_Object_Data.ReviewTime.ToString() + Configer.SplitSymbol);

                                                            _Tmp_CI_Object_Data.isClose = true;
                                                            verifyPlainText2.Append(_Tmp_CI_Object_Data.isClose.ToString());

                                                            //計算HASH值
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            _Tmp_CI_Object_Data.HashValue = SF.getHashValue(verifyPlainText2.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結果:明文:[" + verifyPlainText2.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                                                            context.Entry(_Tmp_CI_Object_Data).State = EntityState.Modified;
                                                        }
                                                        else
                                                        {
                                                            dbContextTransaction.Rollback();
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                                    else
                                                    {
                                                        dbContextTransaction.Rollback();
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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

                                                //物件及物件屬性一起異動
                                                context.SaveChanges();

                                                //修改待覆核屬性
                                                _Tmp_CI_Objects.ReviewAccount = nowUser;
                                                verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Objects.ReviewAccount+Configer.SplitSymbol);

                                                _Tmp_CI_Objects.ReviewTime = DateTime.Now;
                                                verifyPlainText.Append(_Tmp_CI_Objects.ReviewTime.ToString() + Configer.SplitSymbol);

                                                _Tmp_CI_Objects.isClose = true;
                                                verifyPlainText.Append(_Tmp_CI_Objects.isClose.ToString());

                                                //計算HASH值
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                _Tmp_CI_Objects.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結果:明文:[" + verifyPlainText.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                context.Entry(_Tmp_CI_Objects).State = EntityState.Modified;
                                                context.SaveChanges();
                                                dbContextTransaction.Commit();
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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

                                                if (ReviewCount > 0)
                                                {
                                                    return RedirectToAction("ReviewIndex", "Object");
                                                }
                                                else
                                                {
                                                    return RedirectToAction("Index", "Object");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                        else
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                    else
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                    StringBuilder verifyPlainText3 = new StringBuilder();
                                    string verifyHashValue3 = string.Empty;
                                    StringBuilder PlainText3 = new StringBuilder();

                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    _CI_Objects = context.CI_Objects.Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID).First();
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-取得待覆核物件結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                    if (_CI_Objects != null)
                                    {
                                        _CI_Objects.ObjectID = _Tmp_CI_Objects.oObjectID;
                                        verifyPlainText3.Append(_Tmp_CI_Objects.oObjectID.ToString() + Configer.SplitSymbol);
                                        PlainText3.Append(_CI_Objects.ObjectID.ToString()+Configer.SplitSymbol);

                                        _CI_Objects.ObjectName = _Tmp_CI_Objects.ObjectName;
                                        verifyPlainText3.Append(_Tmp_CI_Objects.ObjectName + Configer.SplitSymbol);
                                        PlainText3.Append(_CI_Objects.ObjectName + Configer.SplitSymbol);

                                        _CI_Objects.ProfileID = _Tmp_CI_Objects.ProfileID;
                                        verifyPlainText3.Append(_Tmp_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);
                                        PlainText3.Append(_CI_Objects.ProfileID.ToString() + Configer.SplitSymbol);

                                        _CI_Objects.Description = _Tmp_CI_Objects.Description;
                                        verifyPlainText3.Append(_Tmp_CI_Objects.Description + Configer.SplitSymbol);
                                        PlainText3.Append(_CI_Objects.Description + Configer.SplitSymbol);

                                        verifyPlainText3.Append(_Tmp_CI_Objects.CreateAccount + Configer.SplitSymbol);
                                        verifyPlainText3.Append(_Tmp_CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);
                                        verifyPlainText3.Append(_Tmp_CI_Objects.Type+ Configer.SplitSymbol);
                                        verifyPlainText3.Append(_Tmp_CI_Objects.isClose.ToString());

                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        verifyHashValue3 = SF.getHashValue(verifyPlainText3.ToString());
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                        verifyPlainText3.Replace("False", "");
                                        verifyPlainText3.Remove(verifyPlainText3.Length - 1, 1);

                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Objects.HashValue + "];重新計算HASH[" + verifyHashValue3+ "]", log_Info);

                                        if (verifyHashValue3 == _Tmp_CI_Objects.HashValue)
                                        {
                                            PlainText3.Append(_CI_Objects.CreateAccount + Configer.SplitSymbol);
                                            PlainText3.Append( _CI_Objects.CreateTime.ToString() + Configer.SplitSymbol);

                                            _CI_Objects.UpdateAccount = nowUser;
                                            PlainText3.Append(_CI_Objects.UpdateAccount+Configer.SplitSymbol);

                                            _CI_Objects.UpdateTime = DateTime.Now;
                                            PlainText3.Append(_CI_Objects.UpdateTime.ToString() );

                                            //計算HASH值
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            _CI_Objects.HashValue = SF.getHashValue(PlainText3.ToString());
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件HASH值結果:明文:[" + PlainText3.ToString() + "];HASH值[" + _CI_Objects.HashValue + "]", log_Info);

                                            context.Entry(_CI_Objects).State = EntityState.Modified;

                                            var query1 = context.Tmp_CI_Object_Data.Where(b => b.ObjectID == _vTmp_CI_Objects_R.ObjectID);

                                            int TmpObjectAttributesCount1 = query1.Count();

                                            if (TmpObjectAttributesCount1 > 0)
                                            {
                                                try
                                                {
                                                    //移除CI_Object_Data
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.RemoveAction + "物件屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    context.CI_Object_Data.RemoveRange(context.CI_Object_Data.Where(b => b.ObjectID == _Tmp_CI_Objects.oObjectID));
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.RemoveAction + "物件屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    foreach (var item in query1.ToList())
                                                    {

                                                        CI_Object_Data _CI_Object_Data = new CI_Object_Data();
                                                        Tmp_CI_Object_Data _Tmp_CI_Object_Data = new Tmp_CI_Object_Data();

                                                        _Tmp_CI_Object_Data = context.Tmp_CI_Object_Data.Where(b => b.AttributeID == item.AttributeID)
                                                       .Where(b => b.ObjectID == _Tmp_CI_Objects.ObjectID).First();

                                                        if (_Tmp_CI_Object_Data != null)
                                                        {
                                                            StringBuilder verifyPlainText4 = new StringBuilder();
                                                            string verifyHashValue4 = string.Empty;
                                                            StringBuilder PlainText4 = new StringBuilder();

                                                            _CI_Object_Data.ObjectID = _Tmp_CI_Objects.oObjectID;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.ObjectID.ToString() + Configer.SplitSymbol);
                                                            PlainText4.Append(_Tmp_CI_Objects.oObjectID.ToString()+ Configer.SplitSymbol);

                                                            _CI_Object_Data.AttributeID = _Tmp_CI_Object_Data.AttributeID;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);
                                                            PlainText4.Append(_CI_Object_Data.AttributeID.ToString() + Configer.SplitSymbol);

                                                            _CI_Object_Data.AttributeValue = _Tmp_CI_Object_Data.AttributeValue;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.AttributeValue + Configer.SplitSymbol);
                                                            PlainText4.Append(_CI_Object_Data.AttributeValue + Configer.SplitSymbol);

                                                            _CI_Object_Data.AttributeOrder = _Tmp_CI_Object_Data.AttributeOrder;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);
                                                            PlainText4.Append(_CI_Object_Data.AttributeOrder.ToString() + Configer.SplitSymbol);

                                                            _CI_Object_Data.CreateAccount = _Tmp_CI_Object_Data.CreateAccount;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.CreateAccount + Configer.SplitSymbol);
                                                            PlainText4.Append(_CI_Object_Data.CreateAccount + Configer.SplitSymbol);

                                                            _CI_Object_Data.CreateTime = _Tmp_CI_Object_Data.CreateTime;
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);
                                                            PlainText4.Append(_CI_Object_Data.CreateTime.ToString() + Configer.SplitSymbol);

                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.Type+Configer.SplitSymbol);
                                                            verifyPlainText4.Append(_Tmp_CI_Object_Data.isClose.ToString());

                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            verifyHashValue4 = SF.getHashValue(verifyPlainText4.ToString());
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件屬性作業子程序-重新計算HASH值結果:原HASH值[" + _Tmp_CI_Object_Data.HashValue + "];重新計算HASH值[" + verifyPlainText4 + "]", log_Info);

                                                            verifyPlainText4.Replace("False", "");
                                                            verifyPlainText4.Remove(verifyPlainText4.Length - 1, 1);

                                                            if (verifyHashValue4 == _Tmp_CI_Object_Data.HashValue)
                                                            {
                                                                _CI_Object_Data.UpdateAccount = nowUser;
                                                                PlainText4.Append(_CI_Object_Data.UpdateAccount+ Configer.SplitSymbol);

                                                                _CI_Object_Data.UpdateTime = DateTime.Now;
                                                                PlainText4.Append(_CI_Object_Data.UpdateTime.ToString());

                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                _CI_Object_Data.HashValue = SF.getHashValue(PlainText4.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算物件屬性HASH值結果:明文:[" + PlainText4.ToString() + "];HASH值[" + _CI_Object_Data.HashValue + "]", log_Info);

                                                                //新增範本屬性
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                context.CI_Object_Data.Add(_CI_Object_Data);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                                //修改待覆核屬性
                                                                _Tmp_CI_Object_Data.ReviewAccount = nowUser;
                                                                verifyPlainText4.Append(Configer.SplitSymbol + _Tmp_CI_Object_Data.ReviewAccount + Configer.SplitSymbol);

                                                                _Tmp_CI_Object_Data.ReviewTime = DateTime.Now;
                                                                verifyPlainText4.Append( _Tmp_CI_Object_Data.ReviewTime.ToString() + Configer.SplitSymbol);

                                                                _Tmp_CI_Object_Data.isClose = true;
                                                                verifyPlainText4.Append(_Tmp_CI_Object_Data.isClose.ToString() + Configer.SplitSymbol);

                                                                //計算HASH值
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                _Tmp_CI_Object_Data.HashValue = SF.getHashValue(verifyPlainText4.ToString());
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件屬性HASH值結果:明文:[" + verifyPlainText4.ToString() + "];HASH值[" + _Tmp_CI_Object_Data.HashValue + "]", log_Info);

                                                                context.Entry(_Tmp_CI_Object_Data).State = EntityState.Modified;
                                                            }
                                                            else
                                                            {
                                                                dbContextTransaction.Rollback();
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                                        else
                                                        {
                                                            dbContextTransaction.Rollback();
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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

                                                    //修改待覆核屬性
                                                    _Tmp_CI_Objects.ReviewAccount = nowUser;
                                                    verifyPlainText3.Append(Configer.SplitSymbol + _Tmp_CI_Objects.ReviewAccount + Configer.SplitSymbol);

                                                    _Tmp_CI_Objects.ReviewTime = DateTime.Now;
                                                    verifyPlainText3.Append( _Tmp_CI_Objects.ReviewTime.ToString() + Configer.SplitSymbol);

                                                    _Tmp_CI_Objects.isClose = true;
                                                    verifyPlainText3.Append(_Tmp_CI_Objects.isClose.ToString());

                                                    //計算HASH值
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    _Tmp_CI_Objects.HashValue = SF.getHashValue(verifyPlainText3.ToString());
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-計算待覆核物件HASH值結果:明文:[" + verifyPlainText3.ToString() + "];HASH值[" + _Tmp_CI_Objects.HashValue + "]", log_Info);

                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    context.Entry(_Tmp_CI_Objects).State = EntityState.Modified;
                                                    context.SaveChanges();
                                                    dbContextTransaction.Commit();
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存待覆核物件資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                    SL.EndTime = DateTime.Now;
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件作業成功，作業類型[" + _vTmp_CI_Objects_R.Type + "]", log_Info);
                                                    SL.TotalCount = 1;
                                                    SL.SuccessCount = 1;
                                                    SL.FailCount = 0;
                                                    SL.Result = true;
                                                    SL.Msg = Configer.ReviewAction + "物件作業成功，作業類型[" + _Tmp_CI_Objects.Type + "]";
                                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                    int ReviewCount = context.Tmp_CI_Objects.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                                    if (ReviewCount > 0)
                                                    {
                                                        return RedirectToAction("ReviewIndex", "Object");
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction("Index", "Object");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    dbContextTransaction.Rollback();
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.CreateAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                            else
                                            {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-儲存物件屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                        else
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                    else
                                    {
                                        //記錄錯誤
                                        SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                                    SF.logandshowInfo(Configer.ReviewAction + "物件子程序-" + Configer.EditAction + "物件作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

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
                    else
                    {
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
                else
                {
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

        /// <summary>
        /// 取得套用範本屬性資料
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        public JsonResult getAttributeInputList(int ProfileID)
        {
            var AttributeList = from Pro in context.CI_Proflie_Attributes.Where(b=>b.ProfileID== ProfileID)
                                join Att in context.CI_Attributes on Pro.AttributeID equals Att.AttributeID
                                join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                                orderby(Pro.AttributeOrder)
                                select new { Att.AttributeID, Att.AttributeName, AttType.AttributeTypeID, AttType.AttributeTypeName,Att.DropDownValues };

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
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                         into y
                        from x in y.DefaultIfEmpty()
                        select new vTmp_CI_Objects_R
                        {
                            ObjectID= Obj.ObjectID,
                            oObjectID = Obj.oObjectID,
                            ObjectName =Obj.ObjectName,
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
                _vTmp_CI_Objects_R.AttributesData = SF.getReviewObjectAttributesData(ObjectID);

                if (_vTmp_CI_Objects_R.Type == Configer.CreateAction)
                {
                    return _vTmp_CI_Objects_R;
                }
                else if (_vTmp_CI_Objects_R.Type == Configer.EditAction || _vTmp_CI_Objects_R.Type == Configer.RemoveAction)
                {
                    _vCI_Objects = getObjectData(_vTmp_CI_Objects_R.oObjectID);

                    if (_vCI_Objects != null)
                    {
                        _vTmp_CI_Objects_R.oObjectID = _vTmp_CI_Objects_R.oProfileID;
                        _vTmp_CI_Objects_R.oObjectName = _vCI_Objects.ObjectName;
                        _vTmp_CI_Objects_R.oDescription = _vCI_Objects.Description;
                        _vTmp_CI_Objects_R.oProfileID = _vCI_Objects.ProfileID;
                        _vTmp_CI_Objects_R.oProfileName = _vCI_Objects.ProfileName;
                        _vTmp_CI_Objects_R.oAttributesData = SF.getObjectAttributesData(_vCI_Objects.ObjectID);
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
                            ObjectID= Obj.ProfileID,
                            ObjectName= Obj.ObjectName,
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
    }
}