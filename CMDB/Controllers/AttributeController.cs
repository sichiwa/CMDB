using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Data.Entity;
using CMDB.DAL;
using CMDB.Models;
using CMDB.ViewModels;
using CMDB.SystemClass;

namespace CMDB.Controllers
{
    public class AttributeController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: Attribute
        public ActionResult Index()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 21;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser; 
            SL.Controller = "Attribute";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "屬性清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Attributes_List vAttrList = new vCI_Attributes_List();
                SF.logandshowInfo(Configer.GetAction + "屬性清單-子程序-"+ Configer.GetAction + "待覆核屬性資料筆數開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vAttrList.ReviewCount = context.Tmp_CI_Attributes.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();
                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序" + Configer.GetAction + "待覆核屬性資料筆數結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "待覆核屬性資料結果:共取得[" + vAttrList.ReviewCount .ToString()+ "]筆", log_Info);


                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vAttrList.AttributesData = SF.getAttributesData();
                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

              
                int AttributeCount = 0;

                if (vAttrList.AttributesData != null)
                {
                    AttributeCount = vAttrList.AttributesData.Count();
                    SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "屬性資料結果:共取得[" + AttributeCount.ToString() + "]筆", log_Info);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "屬性資料結果:共取得[0]筆", log_Info);
                }

                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "使用者授權開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vAttrList.Authority = SF.getAuthority(true, false, nowRole, nowFunction);
                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "使用者授權結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "使用者授權結果:使用者[" + nowUser+"];授權[" + vAttrList.Authority + "]", log_Info);

                if (AttributeCount> 0)
                {
                    foreach (var item in vAttrList.AttributesData)
                    {
                        SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "正在編輯屬性資料的帳號開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        item.EditAccount = SF.canEdit("CI_Attributes", item.AttributeID.ToString(), "");
                        SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "正在編輯屬性資料的帳號結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.GetAction + "屬性清單子程序-" + Configer.GetAction + "正在編輯屬性資料的帳號結果:屬性[" + item .AttributeName+ "];編輯帳號[" + item.EditAccount + "]", log_Info);
                    }
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "屬性清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "屬性清單成功" , log_Info);
                    SL.TotalCount = AttributeCount;
                    SL.SuccessCount = AttributeCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction +  "屬性清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
                
                    TempData["SystemInfo"] = "OK";

                    return View(vAttrList);
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.GetAction + "屬性清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.GetAction + "屬性清單成功，系統尚未建立屬性", log_Info);
                    SL.TotalCount =0;
                    SL.SuccessCount =0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "屬性清單作業成功，系統尚未建立屬性";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "尚未建立屬性";

                    return View(vAttrList);
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "屬性清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "屬性清單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
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
            SL.Controller = "Attribute";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "新增屬性表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vCI_Attributes_CU vAttrCU = new vCI_Attributes_CU();

                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單子程序-" + Configer.GetAction + "屬性類型下拉式選單開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                vAttrCU.AttributeType = SF.getAttributeTypeList(1);
                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單子程序-" + Configer.GetAction + "屬性類型下拉式選單結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (vAttrCU.AttributeType != null)
                {
                    SF.logandshowInfo(Configer.CreateAction + "新增屬性表單子程序-" + Configer.GetAction + "屬性類型下拉式選單結果:共取得[" + vAttrCU.AttributeType.Count().ToString() + "]", log_Info);
                    vAttrCU.AttributeTypeID = 1;
                }
                else
                {
                    SF.logandshowInfo(Configer.CreateAction + "新增屬性表單子程序-" + Configer.GetAction + "屬性類型下拉式選單結果:目前系統無屬性類型下拉式選單", log_Info);
                }

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單作業成功", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 1;
                SL.FailCount = 0;
                SL.Result = true;
                SL.Msg = Configer.CreateAction+  "新增屬性表單作業成功";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return View(vAttrCU);

            }
            catch (Exception ex)
            { 
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "新增屬性表單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "新增屬性表單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Attribute");
            }
        }

        // POST: Create
        [HttpPost]
        public string Create(vCI_Attributes_CU vAttrCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "Create";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "屬性開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                if (ModelState.IsValid)
                {
                    StringBuilder PlainText = new StringBuilder();

                    Tmp_CI_Attributes _Tmp_CI_Attributes = new Tmp_CI_Attributes();
                    _Tmp_CI_Attributes.AttributeName = vAttrCU.AttributeName;
                    PlainText.Append(_Tmp_CI_Attributes.AttributeName + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.Description = vAttrCU.Description;
                    PlainText.Append(_Tmp_CI_Attributes.Description + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.AttributeTypeID = vAttrCU.AttributeTypeID;
                    PlainText.Append(_Tmp_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.DropDownValues = vAttrCU.DropDownValues;
                    PlainText.Append(_Tmp_CI_Attributes.DropDownValues + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.CreateAccount = nowUser;
                    PlainText.Append(_Tmp_CI_Attributes.CreateAccount + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.CreateTime = DateTime.Now;
                    PlainText.Append(_Tmp_CI_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.Type = Configer.CreateAction;
                    PlainText.Append(_Tmp_CI_Attributes.Type + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.isClose = false;
                    PlainText.Append(_Tmp_CI_Attributes.isClose.ToString());

                    //計算HASH值
                    SF.logandshowInfo(Configer.CreateAction + "屬性子程序-計算HASH值開始@" +DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    _Tmp_CI_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                    SF.logandshowInfo(Configer.CreateAction + "屬性子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "屬性子程序-計算HASH值結果:明文:["+ PlainText.ToString() + "];HASH值[" + _Tmp_CI_Attributes.HashValue + "]", log_Info);

                    SF.logandshowInfo(Configer.CreateAction + "屬性子程序-新增資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    context.Tmp_CI_Attributes.Add(_Tmp_CI_Attributes);
                    context.SaveChanges();
                    SF.logandshowInfo(Configer.CreateAction + "屬性子程序-新增資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "屬性作業成功", log_Info);
                    SL.TotalCount =1;
                    SL.SuccessCount =1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.CreateAction + "屬性作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    //TempData["CreateMsg"] = "<script>alert('建立屬性成功');</script>";

                    return SL.Msg;
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "屬性作業失敗，異常訊息[資料驗證失敗]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount =0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "屬性作業失敗，異常訊息[資料驗證失敗]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    //TempData["CreateMsg"] = "<script>alert('建立屬性失敗');</script>";

                    return SL.Msg;
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "屬性作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "屬性作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                return SL.Msg;
            }
        }

        // GET: Edit
        public ActionResult Edit(int AttributeID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                CI_Attributes _CI_Attributes = new CI_Attributes();
                vCI_Attributes_CU vAttrCU = new vCI_Attributes_CU();

                SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單-子程序"+ Configer.GetAction+"屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _CI_Attributes = context.CI_Attributes.Where(b=>b.AttributeID== AttributeID).First();
                SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單-子程序" + Configer.GetAction + "屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_CI_Attributes != null)
                {
                    vAttrCU.AttributeID = _CI_Attributes.AttributeID;
                    vAttrCU.AttributeName = _CI_Attributes.AttributeName;
                    vAttrCU.Description = _CI_Attributes.Description;
                    vAttrCU.AttributeType = SF.getAttributeTypeList(_CI_Attributes.AttributeTypeID);
                    vAttrCU.AttributeTypeID = _CI_Attributes.AttributeTypeID;
                    vAttrCU.DropDownValues = _CI_Attributes.DropDownValues;

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "編輯屬性表單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(vAttrCU);
                }
                else
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單作業失敗，異常訊息[查無原始屬性資料]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.CreateAction + "編輯屬性表單作業失敗，異常訊息[查無原始屬性資料]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "Attribute");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.CreateAction + "編輯屬性表單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.CreateAction + "編輯屬性表單作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Attribute");
            }
        }

        // POST: Edit
        [HttpPost]
        public ActionResult Edit(vCI_Attributes_CU vAttrCU)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "Edit";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.EditAction + "屬性開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                if (ModelState.IsValid)
                {
                    StringBuilder PlainText = new StringBuilder();

                    Tmp_CI_Attributes _Tmp_CI_Attributes = new Tmp_CI_Attributes();
                    _Tmp_CI_Attributes.oAttributeID = vAttrCU.AttributeID;
                    PlainText.Append(_Tmp_CI_Attributes.oAttributeID.ToString() + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.AttributeName = vAttrCU.AttributeName;
                    PlainText.Append(_Tmp_CI_Attributes.AttributeName + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.Description = vAttrCU.Description;
                    PlainText.Append(_Tmp_CI_Attributes.Description + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.AttributeTypeID = vAttrCU.AttributeTypeID;
                    PlainText.Append(_Tmp_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.DropDownValues = vAttrCU.DropDownValues;
                    PlainText.Append(_Tmp_CI_Attributes.DropDownValues + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.CreateAccount = nowUser;
                    PlainText.Append(_Tmp_CI_Attributes.CreateAccount + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.CreateTime = DateTime.Now;
                    PlainText.Append(_Tmp_CI_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.Type = Configer.EditAction;
                    PlainText.Append(_Tmp_CI_Attributes.Type + Configer.SplitSymbol);

                    _Tmp_CI_Attributes.isClose = false;
                    PlainText.Append(_Tmp_CI_Attributes.isClose.ToString());

                    //計算HASH值
                    SF.logandshowInfo(Configer.EditAction + "屬性子程序-計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    _Tmp_CI_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                    SF.logandshowInfo(Configer.EditAction + "屬性子程序-計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SF.logandshowInfo(Configer.EditAction + "屬性子程序-"+ Configer.EditAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    context.Tmp_CI_Attributes.Add(_Tmp_CI_Attributes);
                    context.SaveChanges();
                    SF.logandshowInfo(Configer.EditAction + "屬性子程序-"+ Configer.EditAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.EditAction + "屬性結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.EditAction + "屬性作業成功", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.EditAction+ "屬性作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    //TempData["CreateMsg"] = "<script>alert('建立屬性成功');</script>";

                    return RedirectToAction("Index", "Attribute");
                }
                else
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.EditAction + "屬性結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.EditAction + "屬性作業失敗，異常訊息[資料驗證失敗]", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.EditAction+ "屬性作業失敗，異常訊息[資料驗證失敗]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    //TempData["CreateMsg"] = "<script>alert('建立屬性失敗');</script>";

                    return RedirectToAction("Index", "Attribute");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.EditAction + "屬性結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.EditAction + "屬性作業失敗，異常訊息["+ ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.EditAction+  "屬性作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                // TempData["CreateMsg"] = "<script>alert('建立屬性發生異常');</script>";

                return RedirectToAction("Index", "Attribute");
            }
        }

        // GET: ReviewAttribute
        public ActionResult ReviewIndex()
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "ReviewIndex";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                var query = from Att in context.Tmp_CI_Attributes
                            .Where(b => b.isClose == false).Where(b => b.CreateAccount != nowUser)
                            join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                            join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                            join Upd in context.Accounts on Att.CreateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vTmp_CI_Attributes
                            {
                                AttributeID = Att.AttributeID,
                                AttributeName = Att.AttributeName,
                                Description=Att.Description,
                                AttributeTypeName = AttType.AttributeTypeName,
                                Creator = Cre.Name,
                                CreateTime = Att.CreateTime,
                                Type = Att.Type
                            };

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                int ReviewCount = query.Count();
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單結果:共取得[" + ReviewCount.ToString() + "]筆", log_Info);

                if (ReviewCount > 0)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單作業成功", log_Info);
                    SL.TotalCount = ReviewCount;
                    SL.SuccessCount = ReviewCount;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction+ "待覆核屬性清單作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "OK";

                    return View(query);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單作業成功，系統尚未產生待覆核屬性", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核屬性清單作業成功，系統尚未產生待覆核屬性";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    TempData["SystemInfo"] = "無待覆核屬性";

                    return View();
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性清單作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核屬性清單作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("Index", "Attribute");
            }
        }

        // GET: Review
        public ActionResult Review(int AttributeID)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                vTmp_CI_Attributes_R _vTmp_CI_Attributes_R = new vTmp_CI_Attributes_R();
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料子程序-"+ Configer.GetAction + "資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                _vTmp_CI_Attributes_R = getReviewData(AttributeID);
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料子程序-" + Configer.GetAction + "資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);

                if (_vTmp_CI_Attributes_R != null)
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料作業成功" , log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 1;
                    SL.FailCount = 0;
                    SL.Result = true;
                    SL.Msg = Configer.GetAction + "待覆核屬性資料作業成功";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return View(_vTmp_CI_Attributes_R);
                }
                else
                {
                    SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料作業失敗，查無待覆核屬性[" + AttributeID + "]資料", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.GetAction + "待覆核屬性資料作業失敗，查無待覆核屬性[" + AttributeID + "]資料";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("ReviewIndex", "Attribute");
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.GetAction + "待覆核屬性資料作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 0;
                SL.SuccessCount = 0;
                SL.FailCount = 0;
                SL.Result = false;
                SL.Msg = Configer.GetAction + "待覆核屬性資料作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                return RedirectToAction("ReviewIndex", "Attribute");
            }
        }

        // POST: Review
        [HttpPost]
        public ActionResult Review(vTmp_CI_Attributes_R _vTmp_CI_Attributes_R)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = "Attribute";
            SL.Action = "Review";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.ReviewAction + "屬性開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            try
            {
                if (ModelState.IsValid)
                {
                    CI_Attributes _CI_Attributes = new CI_Attributes();
                    Tmp_CI_Attributes _Tmp_CI_Attributes = new Tmp_CI_Attributes();

                    SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-"+ Configer .GetAction+ "待覆核屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                    _Tmp_CI_Attributes = context.Tmp_CI_Attributes.Find(_vTmp_CI_Attributes_R.AttributeID);
                    SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.GetAction + "待覆核屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                    if (_Tmp_CI_Attributes != null)
                    {
                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-取得待覆核屬性結果:共取得[1]筆", log_Info);
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            switch (_Tmp_CI_Attributes.Type)
                            {
                                case "建立":
                                    try
                                    {
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" +Configer.CreateAction +"屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        StringBuilder verifyPlainText = new StringBuilder();
                                        string verifyHashValue = string.Empty;
                                        StringBuilder PlainText = new StringBuilder();

                                        _CI_Attributes.AttributeID = _Tmp_CI_Attributes.AttributeID;
                                        _CI_Attributes.AttributeName = _Tmp_CI_Attributes.AttributeName;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.AttributeName + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.AttributeName + Configer.SplitSymbol);

                                        _CI_Attributes.Description = _Tmp_CI_Attributes.Description;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.Description + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.Description + Configer.SplitSymbol);

                                        _CI_Attributes.AttributeTypeID = _Tmp_CI_Attributes.AttributeTypeID;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);

                                        _CI_Attributes.DropDownValues = _Tmp_CI_Attributes.DropDownValues;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.DropDownValues + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.DropDownValues + Configer.SplitSymbol);

                                        _CI_Attributes.CreateAccount = _Tmp_CI_Attributes.CreateAccount;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.CreateAccount + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.CreateAccount + Configer.SplitSymbol);

                                        _CI_Attributes.CreateTime = _Tmp_CI_Attributes.CreateTime;
                                        verifyPlainText.Append(_Tmp_CI_Attributes.CreateTime.ToString() + Configer.SplitSymbol);
                                        PlainText.Append(_CI_Attributes.CreateTime.ToString());

                                        verifyPlainText.Append(_Tmp_CI_Attributes.Type + Configer.SplitSymbol );
                                        verifyPlainText.Append(_Tmp_CI_Attributes.isClose.ToString());
                                        //PlainText.Append(_CI_Attributes.CreateTime.ToString());

                                        //重新計算HASH
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        //SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        verifyPlainText.Replace("False", "");
                                        verifyPlainText.Remove(verifyPlainText.Length-1,1);

                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業子程序-重新計算HASH值結果:原HASH["+ _Tmp_CI_Attributes.HashValue + "];重新計算HASH["+ verifyPlainText .ToString()+ "]", log_Info);

                                        //與原HASH相比
                                        if (verifyHashValue == _Tmp_CI_Attributes.HashValue)
                                        {
                                            _CI_Attributes.UpdateAccount = nowUser;
                                            PlainText.Append(Configer.SplitSymbol+_CI_Attributes.UpdateAccount + Configer.SplitSymbol);

                                            _CI_Attributes.UpdateTime = DateTime.Now;
                                            PlainText.Append(_CI_Attributes.UpdateTime.ToString());

                                            //計算HASH值
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            _CI_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值結果:明文:["+ PlainText.ToString() + "];HASH[" + _CI_Attributes.HashValue + "]", log_Info);

                                            //新增屬性
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            context.CI_Attributes.Add(_CI_Attributes);
                                            context.SaveChanges();
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                            //修改待覆核屬性
                                            _Tmp_CI_Attributes.ReviewAccount = nowUser;
                                            verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Attributes.ReviewAccount+ Configer.SplitSymbol);

                                            _Tmp_CI_Attributes.ReviewTime = DateTime.Now;
                                            verifyPlainText.Append( _Tmp_CI_Attributes.ReviewTime.ToString() + Configer.SplitSymbol);

                                            _Tmp_CI_Attributes.isClose = true;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.isClose.ToString());

                                            //計算HASH值
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            _Tmp_CI_Attributes.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值結果:明文:["+ verifyPlainText.ToString() + "];HASH[" + _Tmp_CI_Attributes.HashValue + "]", log_Info);

                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            context.Entry(_Tmp_CI_Attributes).State = EntityState.Modified;
                                            context.SaveChanges();
                                            dbContextTransaction.Commit();
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性作業成功，作業類型[" + _vTmp_CI_Attributes_R.Type + "]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 1;
                                            SL.FailCount = 0;
                                            SL.Result = true;
                                            SL.Msg = Configer.ReviewAction + "屬性作業成功，作業類型[" + _vTmp_CI_Attributes_R.Type + "]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            int ReviewCount = context.Tmp_CI_Attributes.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                            if (ReviewCount > 0)
                                            {
                                                return RedirectToAction("ReviewIndex", "Attribute");
                                            }
                                            else
                                            {
                                                return RedirectToAction("Index", "Attribute");
                                            }
                                        }
                                        else
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                            //HASH驗證失敗
                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，異常訊息[屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，異常訊息[屬性HASH驗證失敗，原HASH:" + _Tmp_CI_Attributes.HashValue + "；重新計算HASH:"+ verifyHashValue + "]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        dbContextTransaction.Rollback();
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.CreateAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        //記錄錯誤
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "屬性作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                                    }
                                    
                                case "編輯":
                                    SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-取得待覆核屬性開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    _CI_Attributes = context.CI_Attributes.Where(b => b.AttributeID == _Tmp_CI_Attributes.oAttributeID).First();
                                    SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-取得待覆核屬性結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                    if (_CI_Attributes != null)
                                    {
                                        try
                                        {
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            StringBuilder verifyPlainText = new StringBuilder();
                                            string verifyHashValue = string.Empty;
                                            StringBuilder PlainText = new StringBuilder();

                                            _CI_Attributes.AttributeID = _Tmp_CI_Attributes.oAttributeID;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.oAttributeID.ToString() + Configer.SplitSymbol);
                                            PlainText.Append( _CI_Attributes.AttributeID.ToString() + Configer.SplitSymbol);

                                            _CI_Attributes.AttributeName = _Tmp_CI_Attributes.AttributeName;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.AttributeName+ Configer.SplitSymbol);
                                            PlainText.Append(_CI_Attributes.AttributeName + Configer.SplitSymbol);

                                            _CI_Attributes.Description = _Tmp_CI_Attributes.Description;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.Description + Configer.SplitSymbol);
                                            PlainText.Append(_CI_Attributes.Description + Configer.SplitSymbol);

                                            _CI_Attributes.AttributeTypeID = _Tmp_CI_Attributes.AttributeTypeID;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);
                                            PlainText.Append(_CI_Attributes.AttributeTypeID.ToString() + Configer.SplitSymbol);

                                            _CI_Attributes.DropDownValues = _Tmp_CI_Attributes.DropDownValues;
                                            verifyPlainText.Append(_Tmp_CI_Attributes.DropDownValues + Configer.SplitSymbol);
                                            PlainText.Append(_CI_Attributes.DropDownValues + Configer.SplitSymbol);

                                            verifyPlainText.Append(_Tmp_CI_Attributes.CreateAccount + Configer.SplitSymbol);
                                            verifyPlainText.Append(_Tmp_CI_Attributes.CreateTime.ToString() + Configer.SplitSymbol);
                                            verifyPlainText.Append(_Tmp_CI_Attributes.Type + Configer.SplitSymbol);
                                            verifyPlainText.Append(_Tmp_CI_Attributes.isClose.ToString());

                                            //重新計算HASH值
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業子程序-重新計算HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            verifyHashValue = SF.getHashValue(verifyPlainText.ToString());
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業子程序-重新計算HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            verifyPlainText.Replace("False", "");
                                            verifyPlainText.Remove(verifyPlainText.Length - 1, 1);

                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業子程序-重新計算HASH值結果:原HASH[" + _Tmp_CI_Attributes.HashValue + "];重新計算HASH[" + verifyPlainText.ToString() + "]", log_Info);

                                            //與原HASH值相比
                                            if (verifyHashValue == _Tmp_CI_Attributes.HashValue)
                                            {

                                                PlainText.Append(_CI_Attributes.CreateAccount + Configer.SplitSymbol);
                                                PlainText.Append(_CI_Attributes.CreateTime.ToString() + Configer.SplitSymbol);

                                                _CI_Attributes.UpdateAccount = nowUser;
                                                PlainText.Append(_CI_Attributes.UpdateAccount + Configer.SplitSymbol);

                                                _CI_Attributes.UpdateTime = DateTime.Now;
                                                PlainText.Append(_CI_Attributes.UpdateTime.ToString());

                                                //計算HASH值
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                _CI_Attributes.HashValue = SF.getHashValue(PlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算屬性HASH值結果:明文:["+ PlainText.ToString() + "];HASH值[" + _Tmp_CI_Attributes.HashValue + "]", log_Info);

                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存核屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                context.Entry(_CI_Attributes).State = EntityState.Modified;
                                                context.SaveChanges();
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                //修改待覆核屬性
                                                _Tmp_CI_Attributes.ReviewAccount = nowUser;
                                                verifyPlainText.Append(Configer.SplitSymbol + _Tmp_CI_Attributes.ReviewAccount + Configer.SplitSymbol);

                                                _Tmp_CI_Attributes.ReviewTime = DateTime.Now;
                                                verifyPlainText.Append(_Tmp_CI_Attributes.ReviewTime.ToString() + Configer.SplitSymbol);

                                                _Tmp_CI_Attributes.isClose = true;
                                                verifyPlainText.Append(_Tmp_CI_Attributes.isClose.ToString());

                                                //計算HASH值
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                _Tmp_CI_Attributes.HashValue = SF.getHashValue(verifyPlainText.ToString());
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-計算待覆核屬性HASH值結果:HASH[" + _Tmp_CI_Attributes.HashValue + "]", log_Info);

                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料開始@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                context.Entry(_Tmp_CI_Attributes).State = EntityState.Modified;
                                                context.SaveChanges();
                                                dbContextTransaction.Commit();
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);

                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性作業成功，作業類型[" + _vTmp_CI_Attributes_R.Type + "]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 1;
                                                SL.FailCount = 0;
                                                SL.Result = true;
                                                SL.Msg = Configer.ReviewAction + "屬性作業成功，作業類型[" + _vTmp_CI_Attributes_R.Type + "]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                int ReviewCount = context.Tmp_CI_Attributes.Where(b => b.CreateAccount != nowUser).Where(b => b.isClose == false).Count();

                                                if (ReviewCount > 0)
                                                {
                                                    return RedirectToAction("ReviewIndex", "Attribute");
                                                }
                                                else
                                                {
                                                    return RedirectToAction("Index", "Attribute");
                                                }
                                            }
                                            else
                                            {
                                                dbContextTransaction.Rollback();
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                //HASH驗證失敗
                                                SL.EndTime = DateTime.Now;
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                                SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue + "]", log_Info);
                                                SL.TotalCount = 1;
                                                SL.SuccessCount = 0;
                                                SL.FailCount = 1;
                                                SL.Result = false;
                                                SL.Msg = Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，異常訊息[HASH驗證失敗，原HASH:" + _Tmp_CI_Attributes.HashValue + "；重新計算HASH:" + verifyHashValue + "]";
                                                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                                return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            dbContextTransaction.Rollback();
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            //記錄錯誤
                                            SL.EndTime = DateTime.Now;
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                            SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]", log_Info);
                                            SL.TotalCount = 1;
                                            SL.SuccessCount = 0;
                                            SL.FailCount = 1;
                                            SL.Result = false;
                                            SL.Msg = Configer.ReviewAction + "屬性作業失敗，異常訊息[資料庫作業失敗:" + ex.ToString() + "]";
                                            SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                            return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                                        }
                                    }
                                    else
                                    {
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-儲存待覆核屬性資料結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性子程序-" + Configer.EditAction + "屬性作業結束@" + DateTime.Now.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        //記錄錯誤
                                        SL.EndTime = DateTime.Now;
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                        SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，查無原始屬性資料", log_Info);
                                        SL.TotalCount = 1;
                                        SL.SuccessCount = 0;
                                        SL.FailCount = 1;
                                        SL.Result = false;
                                        SL.Msg = Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，查無原始屬性資料";
                                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                        return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                                    }
                                //case "刪除":
                                default:
                                    //記錄錯誤
                                    SL.EndTime = DateTime.Now;
                                    SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                                    SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，查無原始屬性資料", log_Info);
                                    SL.TotalCount = 1;
                                    SL.SuccessCount = 0;
                                    SL.FailCount = 1;
                                    SL.Result = false;
                                    SL.Msg = Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，系統不存在的作業類型";
                                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                                    return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                            }
                        }
                    }
                    else
                    {
                        //記錄錯誤
                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，查無待覆核屬性資料", log_Info);
                        SL.TotalCount = 1;
                        SL.SuccessCount = 0;
                        SL.FailCount = 1;
                        SL.Result = false;
                        SL.Msg = Configer.ReviewAction + "屬性作業失敗，作業類型[" + _vTmp_CI_Attributes_R.Type + "]，查無待覆核屬性資料";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                    }

                }
                else
                {
                    //記錄錯誤
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，資料驗證失敗", log_Info);
                    SL.TotalCount = 1;
                    SL.SuccessCount = 0;
                    SL.FailCount = 1;
                    SL.Result = false;
                    SL.Msg = Configer.ReviewAction +  "屬性作業失敗，資料驗證失敗";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                }
            }
            catch (Exception ex)
            {
                SL.EndTime = DateTime.Now;
                SF.logandshowInfo(Configer.ReviewAction + "屬性結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                SF.logandshowInfo(Configer.ReviewAction + "屬性作業失敗，異常訊息[" + ex.ToString() + "]", log_Info);
                SL.TotalCount = 1;
                SL.SuccessCount = 0;
                SL.FailCount = 1;
                SL.Result = false;
                SL.Msg = Configer.ReviewAction+ "屬性作業失敗，異常訊息[" + ex.ToString() + "]";
                SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                //TempData["CreateMsg"] = "<script>alert('覆核屬性發生異常');</script>";
                return RedirectToAction("Review", "Attribute", new { AttributeID = _vTmp_CI_Attributes_R.AttributeID });
                //return RedirectToAction("Review", "Attribute");
            }
        }

        /// <summary>
        /// 取得待覆核屬性資料
        /// </summary>
        /// <param name="AttributeID">屬性ID</param>
        /// <returns></returns>
        private vTmp_CI_Attributes_R getReviewData(int AttributeID)
        {
            vTmp_CI_Attributes_R _vTmp_CI_Attributes_R = new vTmp_CI_Attributes_R();
            vCI_Attributes _vCI_Attributes = new vCI_Attributes();
            var query = from Att in context.Tmp_CI_Attributes
                        .Where(b => b.AttributeID == AttributeID)
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                         into y
                        from x in y.DefaultIfEmpty()
                        select new vTmp_CI_Attributes_R
                        {
                            AttributeID = Att.AttributeID,
                            oAttributeID=Att.oAttributeID,
                            AttributeName = Att.AttributeName,
                            Description = Att.Description,
                            AttributeTypeName = AttType.AttributeTypeName,
                            DropDownValues = Att.DropDownValues,
                            Creator = x.Name,
                            CreateTime = Att.CreateTime,
                            Type = Att.Type
                        };

            if (query.Count() == 1)
            {
                _vTmp_CI_Attributes_R = query.First();

                if (_vTmp_CI_Attributes_R.Type == Configer.CreateAction)
                {
                    return _vTmp_CI_Attributes_R;
                }
                else if (_vTmp_CI_Attributes_R.Type == Configer.EditAction || _vTmp_CI_Attributes_R.Type == Configer.RemoveAction)
                {
                    _vCI_Attributes = getAttributeData(_vTmp_CI_Attributes_R.oAttributeID);

                    if (_vCI_Attributes != null)
                    {
                        _vTmp_CI_Attributes_R.oAttributeName = _vCI_Attributes.AttributeName;
                        _vTmp_CI_Attributes_R.oDescription = _vCI_Attributes.Description;
                        _vTmp_CI_Attributes_R.oAttributeTypeName = _vCI_Attributes.AttributeTypeName;
                        _vTmp_CI_Attributes_R.oDropDownValues = _vCI_Attributes.DropDownValues;
                        _vTmp_CI_Attributes_R.oUpadter = _vCI_Attributes.Upadter;
                        _vTmp_CI_Attributes_R.oUpdateTime = _vCI_Attributes.UpdateTime;

                        return _vTmp_CI_Attributes_R;
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
        /// 取得原屬性資料
        /// </summary>
        /// <param name="AttributeID">屬性ID</param>
        /// <returns></returns>
        private vCI_Attributes getAttributeData(int AttributeID)
        {
            vCI_Attributes _vCI_Attributes = new vCI_Attributes();

            var query = from Att in context.CI_Attributes
                        .Where(b=>b.AttributeID== AttributeID)
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.CreateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            Description = Att.Description,
                            AttributeTypeName = AttType.AttributeTypeName,
                            DropDownValues=Att.DropDownValues,
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() == 1)
            {
                _vCI_Attributes = query.First();

                return _vCI_Attributes;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得完整屬性清單
        /// </summary>
        /// <returns></returns>
        public JsonResult getAttributeList()
        {
            //Expression<Func<CI_Attributes, bool>> TypeWhereCondition;
            //if (t_id != "-1")
            //{
            //    TypeWhereCondition = b => b.t_id == t_id;
            //}
            //else
            //{
            //    TypeWhereCondition = b => 1 == 1;
            //}

            var AttributeList = from Att in context.CI_Attributes
                                join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                                select new { Att.AttributeID, Att.AttributeName, AttType.AttributeTypeName};

            if (AttributeList != null)
            {
                return Json(AttributeList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }
    }
}