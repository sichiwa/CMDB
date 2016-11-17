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
    public class ObjectSearchController : Controller
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        ShareFunc SF = new ShareFunc();
        String log_Info = "Info";
        String log_Err = "Err";

        // GET: ObjectSearch
        public ActionResult Index(int ProfileID=-1,string Keyword="", int AttributeID=-1)
        {
            //初始化系統參數
            Configer.Init();

            string nowUser = Session["UserID"].ToString();
            int nowRole = Convert.ToInt32(Session["UserRole"]);
            int nowFunction = 23;

            //Log記錄用
            SystemLogs SL = new SystemLogs();
            SL.Account = nowUser;
            SL.Controller = " ObjectSearch";
            SL.Action = "Index";
            SL.StartTime = DateTime.Now;
            SF.logandshowInfo(Configer.SearchAction + "物件清單開始@" + SL.StartTime.ToString(Configer.SystemDateTimeFormat), log_Info);

            string MailServer = Configer.MailServer;
            int MailServerPort = Configer.MailServerPort;
            string MailSender = Configer.MailSender;
            List<string> MailReceiver = Configer.MailReceiver;

            using (var context = new CMDBContext())
            {
                try
                {
                    vCI_Objects_List vObjList = new vCI_Objects_List();

                    SF.logandshowInfo(Configer.SearchAction + "物件清單子程序-" + Configer.GetAction + "物件資料開始@" + SF.getNowDateString(), log_Info);
                    if (ProfileID > 0){
                        //有ProfileID，使用ProfileID查詢
                        vObjList.ObjectsData = SF.getObjectsData(ProfileID);

                        vObjList.ProfileID = ProfileID;
                        vObjList.Profile = SF.getProfileList(ProfileID);
                    }
                    else {
                        //沒有ProfileID，改用關鍵字查詢
                        List<vSearchKeywords> SearchKeywords = new List<vSearchKeywords>();
                        vSearchKeywords SearchKeyword = new vSearchKeywords();
                        SearchKeyword.AttributeID = AttributeID;
                        SearchKeyword.Keyword = Keyword;
                        SearchKeywords.Add(SearchKeyword);

                        vObjList.ObjectsData = SF.getObjectsData(SearchKeywords);

                        vObjList.ProfileID = 0;
                        vObjList.Profile = SF.getProfileList(0);
                    }

                    SF.logandshowInfo(Configer.SearchAction + "物件清單子程序-" + Configer.GetAction + "物件資料結束@" + SF.getNowDateString(), log_Info);

                    int ObjectCount = 0;

                    ObjectCount = vObjList.ObjectsData.Count();

                    if (ObjectCount > 0){
                        SF.logandshowInfo(Configer.SearchAction + "物件清單子程序-" + Configer.GetAction + "物件資料結果:共取得[" + ObjectCount.ToString() + "]筆", log_Info);

                        vObjList.Authority = SF.getAuthority(true, false, nowRole, nowFunction);

                        foreach (var item in vObjList.ObjectsData)
                        {
                            SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號開始@" + SF.getNowDateString(), log_Info);
                            item.EditAccount = SF.canEdit("CI_Objects", item.ObjectID.ToString(), "");
                            SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號結束@" + SF.getNowDateString(), log_Info);
                            SF.logandshowInfo(Configer.GetAction + "物件清單子程序-" + Configer.GetAction + "正在編輯物件資料的帳號結果:屬性[" + item.ObjectName + "];編輯帳號[" + item.EditAccount + "]", log_Info);
                        }

                        TempData["SystemInfo"] = "OK";

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.SearchAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.SearchAction + "物件清單成功", log_Info);
                        SL.TotalCount = ObjectCount;
                        SL.SuccessCount = ObjectCount;
                        SL.FailCount = 0;
                        SL.Result = true;
                        SL.Msg = Configer.SearchAction + "物件清單作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return View(vObjList);
                    }
                    else {
                        SF.logandshowInfo(Configer.SearchAction + "物件清單子程序-" + Configer.GetAction + "物件資料結果:共取得[0]筆", log_Info);
                        TempData["SystemInfo"] = "本次查詢無資料";

                        SL.EndTime = DateTime.Now;
                        SF.logandshowInfo(Configer.SearchAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                        SF.logandshowInfo(Configer.SearchAction + "物件清單成功", log_Info);
                        SL.TotalCount = 0;
                        SL.SuccessCount = 0;
                        SL.FailCount = 0;
                        SL.Result = true;
                        SL.Msg = Configer.SearchAction + "物件清單作業成功";
                        SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                        return RedirectToAction("Index", "Main");
                    }
                }
                catch (Exception ex)
                {
                    SL.EndTime = DateTime.Now;
                    SF.logandshowInfo(Configer.SearchAction + "物件清單結束@" + SL.EndTime.ToString(Configer.SystemDateTimeFormat), log_Info);
                    SF.logandshowInfo(Configer.SearchAction + "物件清單作業失敗，" + "異常訊息[" + ex.ToString() + "]", log_Info);
                    SL.TotalCount = 0;
                    SL.SuccessCount = 0;
                    SL.FailCount = 0;
                    SL.Result = false;
                    SL.Msg = Configer.SearchAction + "物件清單作業失敗，" + "異常訊息[" + ex.ToString() + "]";
                    SF.log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);

                    return RedirectToAction("Index", "Main");
                }
            }
        }
    }
}