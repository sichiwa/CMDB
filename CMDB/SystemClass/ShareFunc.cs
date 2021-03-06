﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using TWCAlib;
using NLog;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Net;
using CMDB.DAL;
using CMDB.Models;
using System.Globalization;
using CMDB.ViewModels;
using System.Data.Entity.SqlServer;

namespace CMDB.SystemClass
{
    public class ShareFunc
    {
        CMDBContext context = new CMDBContext();
        SystemConfig Configer = new SystemConfig();
        public static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _ConnStr;

        private string _op_name;
        private string log_Info = "Info";
        private string log_Err = "Err";
        public enum MailPriority : int
        {
            Low = 0,
            Middle = 1,
            High = 2
        }


        public string getNowDateString()
        {
            //初始化系統參數
            Configer.Init();

            return DateTime.Now.ToString(Configer.SystemDateTimeFormat);
        }

        /// <summary>
        /// 將資訊記錄至Log檔中
        /// </summary>
        /// <param name="_Str">顯示資訊</param>
        /// <param name="_Mode">Err:記錄至Debug log;Info記錄至Info log:</param>
        /// <remarks>2014/03/04 黃富彥</remarks>
        public void logandshowInfo(string _Str, string _Mode)
        {
            if ((_Mode == "Err"))
            {
                logger.Error(_Str);
            }
            else
            {
                logger.Info(_Str);
            }
        }

        /// <summary>
        /// 將執行結果寫入資料庫
        /// </summary>
        /// <param name="_OPLogger">OPLoger類別</param>
        /// <remarks>2014/03/04 黃富彥</remarks>
        public void log2DB(SystemLogs _SL,
                           string _MailServer,
                           int _MailServerPort,
                           string _MailSender,
                           List<string> _MailReceiver)
        {
            SystemLogs SL = _SL;
            string MailServer = _MailServer;
            int MailServerPort = _MailServerPort;
            string MailSender = _MailSender;
            List<string> MailReceiver = _MailReceiver;
            string MailSubject = string.Empty;
            StringBuilder MailBody = new StringBuilder();
            string SendResult = string.Empty;

            try
            {
                using (CMDBContext context = new CMDBContext())
                {
                    //初始化系統參數
                    Configer.Init();

                    StringBuilder PlainText = new StringBuilder();
                    PlainText.Append(SL.Account + Configer.SplitSymbol);
                    PlainText.Append(SL.Controller + Configer.SplitSymbol);
                    PlainText.Append(SL.Action + Configer.SplitSymbol);
                    PlainText.Append(SL.StartTime.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.EndTime.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.TotalCount.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.SuccessCount.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.FailCount.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.Result.ToString() + Configer.SplitSymbol);
                    PlainText.Append(SL.Msg);

                    //計算HASH值
                    SL.HashValue = getHashValue(PlainText.ToString());

                    context.SystmeLogs.Add(SL);
                    context.SaveChanges();

                    //寫入文字檔Log
                    logandshowInfo("[" + SL.Account + "]執行[寫入資料庫紀錄作業]成功", log_Info);
                }
            }
            catch (Exception ex)
            {
                //異常
                //寫入文字檔Log
                logandshowInfo("[" + SL.Account + "]執行[寫入資料庫紀錄作業]發生未預期的異常,請查詢Debug Log得到詳細資訊", log_Info);
                logandshowInfo("[" + SL.Account + "]執行[寫入資料庫紀錄作業]發生未預期的異常,詳細資訊如下", log_Err);
                logandshowInfo("執行人:[" + SL.Account + "]", log_Err);
                logandshowInfo("執行模組名稱:[" + SL.Controller + "]", log_Err);
                logandshowInfo("執行作業名稱:[" + SL.Action + "]", log_Err);
                logandshowInfo("處理結果:[" + SL.Result.ToString() + "]", log_Err);
                logandshowInfo("起始時間:[" + SL.StartTime.ToString() + "]", log_Err);
                logandshowInfo("結束時間:[" + SL.EndTime.ToString() + "]", log_Err);
                logandshowInfo("處理總筆數:[" + SL.TotalCount.ToString() + "]", log_Err);
                logandshowInfo("處理成功筆數:[" + SL.SuccessCount.ToString() + "]", log_Err);
                logandshowInfo("處理失敗筆數:[" + SL.FailCount.ToString() + "]", log_Err);
                logandshowInfo("作業訊息:[" + SL.Msg + "]", log_Err);
                logandshowInfo("錯誤訊息:[" + ex.ToString() + "]", log_Err);

                //通知系統管理人員
                MailSubject = "[異常]組態管理系統-執行[寫入資料庫紀錄作業]失敗";
                MailBody.Append("<table>");
                MailBody.Append("<tr><td>");
                MailBody.Append("執行人:[" + SL.Account + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("執行模組名稱:[" + SL.Controller + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("執行作業名稱:[" + SL.Action + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("處理結果:[" + SL.Result.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("起始時間:[" + SL.StartTime.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("結束時間:[" + SL.EndTime.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("處理總筆數:[" + SL.TotalCount.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("處理成功筆數:[" + SL.SuccessCount.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("處理失敗筆數:[" + SL.FailCount.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("作業訊息:[" + SL.Msg + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("<tr><td>");
                MailBody.Append("錯誤訊息:[" + ex.ToString() + "]");
                MailBody.Append("</td></tr>");
                MailBody.Append("</table>");

                EmailNotify2Sys(MailServer, MailServerPort, MailSender, MailReceiver, false, MailSubject, MailBody.ToString());
            }

        }

        /// <summary>
        /// 寄送郵件
        /// </summary>
        /// <param name="_MailServer">郵件主機位置</param>
        /// <param name="_MailServerPort">郵件主機服務Port</param>
        /// <param name="_MailSender">寄件者</param>
        /// <param name="_MailReceivers">收件者清單</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SendEmail(string _MailServer, int _MailServerPort, string _MailSender, List<string> _MailReceivers, string _Subject, string _Body)
        {
            string SendResult = null;
            MailProcessor MailP = new MailProcessor();
            string MailServer = _MailServer;
            int MailServerPort = _MailServerPort;
            string MailSender = _MailSender;
            List<string> MailReceivers = _MailReceivers;
            string MailSubject = _Subject;
            string SendBody = _Body;
            List<System.Net.Mail.Attachment> MailA = new List<System.Net.Mail.Attachment>();
            List<string> MailCC = new List<string>();

            MailP.setMailProcossor(MailSender, MailReceivers, MailCC, MailSubject, SendBody, MailA, MailServer, MailServerPort, false, true,
            MailPriority.High.ToString(), 65001);

            SendResult = MailP.Send();

            return SendResult;
        }

        /// <summary>
        /// Email通知系統管理人員
        /// </summary>
        /// <param name="_WitreDB">是否要將通知結果寫入資料庫</param>
        /// <param name="_MailSubject">郵件主旨</param>
        /// <param name="_MailBody">郵件內容</param>
        /// <remarks></remarks>
        public void EmailNotify2Sys(string _MailServer, int _MailServerPort, string _MailSender, List<string> _MailReceiver, bool _WitreDB, string _MailSubject, string _MailBody)
        {
            SystemLogs SL = new SystemLogs();
            SL.Account = "System";
            SL.Action = "通知系統管理人員作業";
            SL.StartTime = DateTime.Now;
            SL.TotalCount = 1;

            bool WitreDB = _WitreDB;

            string MailServer = _MailServer;
            int MailServerPort = _MailServerPort;
            string MailSender = _MailSender;
            List<string> MailReceiver = _MailReceiver;
            string MailSubject = _MailSubject;
            string MailBody = _MailBody;
            string SendResult = string.Empty;

            //寄送通知信給系統管理人員
            SendResult = SendEmail(MailServer, MailServerPort, MailSender, MailReceiver, MailSubject, MailBody.ToString());
            SL.EndTime = DateTime.Now;

            if (SendResult == "success")
            {
                //寫入文字檔Log
                logandshowInfo("[" + SL.Account + "]執行[" + SL.Action + "]成功", log_Info);

                SL.SuccessCount = 1;
                SL.Result = true;
            }
            else
            {
                //寫入文字檔Log
                logandshowInfo("[" + SL.Account + "]執行[通知系統管理人員作業]失敗,請查詢Debug Log得到詳細資訊", log_Info);
                logandshowInfo("[" + SL.Account + "]執行[通知系統管理人員作業]失敗,詳細資訊如下", log_Err);
                logandshowInfo("錯誤訊息:[" + SendResult + "]", log_Err);

                SL.FailCount = 1;
                SL.Msg = SendResult;
            }

            if (WitreDB == true)
            {
                //寫入DB Log
                //OPLoger.SetOPLog(this.op_name, op_action, op_stime, op_etime, op_a_count, op_s_count, op_f_count, op_msg, op_result);
                log2DB(SL, MailServer, MailServerPort, MailSender, MailReceiver);
            }
        }

        /// <summary>
        /// 取得使用者角色ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int getUserRole(string UserID)
        {
            //初始化系統參數
            Configer.Init();

            int Role = -1;
            using (CMDBContext context = new CMDBContext())
            {
                Role = context.Accounts.Find(UserID).RoleID;
                if (Role <= 0)
                {
                    Role = Configer.PublicRoleID;
                }
            }
            return Role;
        }

        /// <summary>
        /// 取得使用者授權
        /// </summary>
        /// <param name="ignoreCertLogin">是否忽略憑證登入</param>
        /// <param name="UseCertLogin">使用憑證登入</param>
        /// <param name="RoleID">角色ID</param>
        /// <param name="FuncnctionID">功能ID</param>
        /// <returns></returns>
        public int getAuthority(bool ignoreCertLogin, bool UseCertLogin, int RoleID, int FuncnctionID)
        {
            //預設唯讀
            int Result = 0;

            if (ignoreCertLogin == true)
            {
                RoleFunctions RF = context.RoleFunctions.Where(b => b.RoleID == RoleID).Where(b => b.FunctionID == FuncnctionID).First();

                if (RF != null)
                {
                    Result = RF.Authority;
                }
            }
            else
            {
                if (UseCertLogin == false)
                {

                }
                else
                {
                    RoleFunctions RF = context.RoleFunctions.Where(b => b.RoleID == RoleID).Where(b => b.FunctionID == FuncnctionID).First();

                    if (RF != null)
                    {
                        Result = RF.Authority;
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// 取得帳號下拉式選單
        /// </summary>
        /// <param name="nowtAccount">目前帳號</param>
        /// <returns></returns>
        public SelectList getAccountList(string nowtAccount)
        {
            var query = context.Accounts.ToList();

            //List<SelectListItem> items = query.Select(b => new SelectListItem() {
            //    Text=b.UserName,
            //    Value=b.UId,
            //}).ToList();

            SelectList UserList = new SelectList(query, "Account", "Name", nowtAccount);

            return UserList;
        }

        /// <summary>
        /// 取得屬性類型下拉式選單
        /// </summary>
        /// <param name="nowAttributeType">目前屬性類型ID</param>
        /// <returns></returns>
        public SelectList getAttributeTypeList(int nowAttributeType)
        {
            var query = context.CI_AttributeTypes.ToList();

            //List<SelectListItem> items = query.Select(b => new SelectListItem() {
            //    Text=b.UserName,
            //    Value=b.UId,
            //}).ToList();

            SelectList AttributeTypeList = new SelectList(query, "AttributeTypeID", "AttributeTypeName", nowAttributeType);

            return AttributeTypeList;
        }

        /// <summary>
        /// 取得範本下拉式選單
        /// </summary>
        /// <param name="nowProfileID">目前範本ID</param>
        /// <returns></returns>
        public SelectList getProfileList(int nowProfileID)
        {
            SelectList ProfileList = null;
            var query = context.CI_Proflies.ToList();
            if (nowProfileID < 0)
            {
                ProfileList = new SelectList(query, "ProfileID", "ProfileName");
            }
            else
            {
                ProfileList = new SelectList(query, "ProfileID", "ProfileName", nowProfileID);
            }
            return ProfileList;
        }

        /// <summary>
        /// 取得帳號資訊
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string getAccountAttribute(string Account, string Type)
        {
            Accounts _Account = context.Accounts.Find(Account);
            if (_Account != null)
            {
                switch (Type)
                {
                    case "Name":
                        return _Account.Name;
                    case "Email":
                        return _Account.Email;
                    case "RoleID":
                        return _Account.RoleID.ToString();
                    case "isEnable":
                        return _Account.isEnable.ToString();
                    default:
                        return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 取得系統圖片路徑
        /// </summary>
        /// <param name="ImgID">圖片ID</param>
        /// <returns></returns>
        public string getSysImgPath(int ImgID)
        {
            string SysImgPath = "";
            SystemImgs _SystemImgs = context.SystemImgs.Where(b => b.ImgID == ImgID).First();

            if (_SystemImgs != null)
            {
                SysImgPath = _SystemImgs.ImgPath;
            }
            return SysImgPath;
        }

        /// <summary>
        /// 取得屬性清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Attributes> getAttributesData()
        {
            var query = from Att in context.CI_Attributes
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            AttributeTypeName = AttType.AttributeTypeName,
                            //EditAccount = canEdit("CI_Attributes", Att.AttributeID.ToString(), ""),
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.AttributeID).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得範本清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Proflies> getProfilesData()
        {
            var query = from Pro in context.CI_Proflies
                        join Cre in context.Accounts on Pro.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Pro.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Proflies
                        {
                            ProfileID = Pro.ProfileID,
                            ProfileName = Pro.ProfileName,
                            //EditAccount = canEdit("CI_Proflies", Pro.ProfileID.ToString(), ""),
                            Creator = Cre.Name,
                            CreateTime = Pro.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Cre.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.ProfileID).ToList();
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<vCI_Object_Relationship> getReviewObjectRelationshipData(int ObjectID, string Type)
        {
            Configer.Init();

            if (Type == Configer.CreateAction)
            {
                var query = from ObjR in context.Tmp_CI_Object_Relationship
                        .Where(b => b.ObjectID == ObjectID)
                        .Where(b => b.isClose == false)
                            join Obj in context.Tmp_CI_Objects on ObjR.ObjectID equals Obj.ObjectID
                            join Obj1 in context.CI_Objects on ObjR.RelationshipObjectID equals Obj1.ObjectID
                            join Cre in context.Accounts on ObjR.CreateAccount equals Cre.Account
                            // join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vCI_Object_Relationship
                            {
                                ObjectID = ObjR.ObjectID,
                                ProfileID = ObjR.ProfileID,
                                ObjectName = Obj.ObjectName,
                                RelationshipObjectID = ObjR.RelationshipObjectID,
                                RelationshipProfileID = ObjR.RelationshipProfileID,
                                RelationshipObjectName = Obj1.ObjectName,
                                Creator = x.Name,
                                CreateTime = ObjR.CreateTime,
                                Upadter = x.Name,
                                UpdateTime = x.UpdateTime
                            };


                if (query.Count() > 0)
                {
                    return query.OrderBy(b => b.ProfileID).ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var query = from ObjR in context.Tmp_CI_Object_Relationship
                       .Where(b => b.ObjectID == ObjectID)
                       .Where(b => b.isClose == false)
                            join Obj in context.CI_Objects on ObjR.oObjectID equals Obj.ObjectID
                            join Obj1 in context.CI_Objects on ObjR.RelationshipObjectID equals Obj1.ObjectID
                            join Cre in context.Accounts on ObjR.CreateAccount equals Cre.Account
                            // join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                            into y
                            from x in y.DefaultIfEmpty()
                            select new vCI_Object_Relationship
                            {
                                ObjectID = ObjR.ObjectID,
                                ProfileID = ObjR.ProfileID,
                                ObjectName = Obj.ObjectName,
                                RelationshipObjectID = ObjR.RelationshipObjectID,
                                RelationshipProfileID = ObjR.RelationshipProfileID,
                                RelationshipObjectName = Obj1.ObjectName,
                                Creator = x.Name,
                                CreateTime = ObjR.CreateTime,
                                Upadter = x.Name,
                                UpdateTime = x.UpdateTime
                            };


                if (query.Count() > 0)
                {
                    return query.OrderBy(b => b.ProfileID).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得待覆核範本關係清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Profile_Relationship> getReviewProfileRelationshipData(int ProfileID)
        {
            var query = from ProR in context.Tmp_CI_Profile_Relationship
                        .Where(b => b.ProfileID == ProfileID)
                         .Where(b => b.isClose == false)
                        join Pro in context.CI_Proflies on ProR.ProfileID equals Pro.ProfileID
                        join Pro1 in context.CI_Proflies on ProR.RelationshipProileID equals Pro1.ProfileID
                        join Cre in context.Accounts on ProR.CreateAccount equals Cre.Account
                        // join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Profile_Relationship
                        {
                            ProfileID = ProR.ProfileID,
                            ProfileName = Pro.ProfileName,
                            RelationshipProfileID = ProR.RelationshipProileID,
                            RelationshipProfileName = Pro1.ProfileName,
                            Creator = x.Name,
                            CreateTime = ProR.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = x.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.ProfileID).ToList();
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<vCI_Object_Relationship> getObjectRelationshipData(int ObjectID)
        {
            var query = from ObjR in context.CI_Object_Relationship
                        .Where(b => b.ObjectID == ObjectID)
                        join Obj in context.CI_Objects on ObjR.ObjectID equals Obj.ObjectID
                        join Obj1 in context.CI_Objects on ObjR.RelationshipObjectID equals Obj1.ObjectID
                        join Cre in context.Accounts on ObjR.CreateAccount equals Cre.Account
                        // join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Object_Relationship
                        {
                            ObjectID = ObjR.ObjectID,
                            ProfileID = ObjR.ProfileID,
                            ObjectName = Obj.ObjectName,
                            RelationshipObjectID = ObjR.RelationshipObjectID,
                            RelationshipProfileID = ObjR.RelationshipProfileID,
                            RelationshipObjectName = Obj1.ObjectName,
                            Creator = x.Name,
                            CreateTime = ObjR.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = x.UpdateTime
                        };

            if (query.Count() > 0)
            {
                //var query1 = query.Select(b => b.ObjectID).Distinct().ToList();
                //List<vCI_Object_Relationship> vCIObjectRelationships = new List<vCI_Object_Relationship>();

                //foreach (var item in query1)
                //{
                //    vCI_Object_Relationship T = new vCI_Object_Relationship();
                //    T.ObjectID = item;
                //    StringBuilder bb = new StringBuilder();
                //    List<string> RelationshipObjectNames = new List<string>();
                //    foreach (var item1 in query.Where(b => b.ObjectID == item).ToList())
                //    {
                //        if (string.IsNullOrEmpty(T.ObjectName))
                //        {
                //            T.ProfileName = item1.ObjectName;
                //            T.Creator = item1.Creator;
                //            T.CreateTime = item1.CreateTime;
                //            T.Upadter = item1.Upadter;
                //            T.UpdateTime = item1.UpdateTime;
                //        }
                //        string RelationshipObjectName = item1.RelationshipProfileName;
                //        RelationshipObjectNames.Add(RelationshipObjectName);
                //    }
                //    T.RelationshipObjectNames = RelationshipObjectNames;
                //    vCIObjectRelationships.Add(T);
                //}
                //return vCIObjectRelationships;
                return query.ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得範本關係清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Profile_Relationship> getProfileRelationshipData()
        {
            var query = from ProR in context.CI_Profile_Relationship
                        join Pro in context.CI_Proflies on ProR.ProfileID equals Pro.ProfileID
                        join Pro1 in context.CI_Proflies on ProR.RelationshipProfileID equals Pro1.ProfileID
                        join Cre in context.Accounts on ProR.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Profile_Relationship
                        {
                            ProfileID = ProR.ProfileID,
                            ProfileName = Pro.ProfileName,
                            RelationshipProfileID = ProR.RelationshipProfileID,
                            RelationshipProfileName = Pro1.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = ProR.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = ProR.UpdateTime
                        };

            if (query.Count() > 0)
            {
                var query1 = query.Select(b => b.ProfileID).Distinct().ToList();
                List<vCI_Profile_Relationship> vCIProfileRelationships = new List<vCI_Profile_Relationship>();

                foreach (var item in query1)
                {
                    vCI_Profile_Relationship T = new vCI_Profile_Relationship();
                    T.ProfileID = item;
                    StringBuilder bb = new StringBuilder();
                    List<string> RelationshipProfileNames = new List<string>();
                    foreach (var item1 in query.Where(b => b.ProfileID == item).ToList())
                    {
                        if (string.IsNullOrEmpty(T.ProfileName))
                        {
                            T.ProfileName = item1.ProfileName;
                            T.Creator = item1.Creator;
                            T.CreateTime = item1.CreateTime;
                            T.Upadter = item1.Upadter;
                            T.UpdateTime = item1.UpdateTime;
                        }
                        string RelationshipProfileName = item1.RelationshipProfileName;
                        RelationshipProfileNames.Add(RelationshipProfileName);
                    }
                    T.RelationshipProfileNames = RelationshipProfileNames;
                    vCIProfileRelationships.Add(T);
                }
                return vCIProfileRelationships;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得特定範本關係清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Profile_Relationship> getProfileRelationshipData(int ProfileID)
        {
            var query = from ProR in context.CI_Profile_Relationship
                        .Where(b => b.ProfileID == ProfileID)
                        join Pro in context.CI_Proflies on ProR.ProfileID equals Pro.ProfileID
                        join Pro1 in context.CI_Proflies on ProR.RelationshipProfileID equals Pro1.ProfileID
                        join Cre in context.Accounts on ProR.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on ProR.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Profile_Relationship
                        {
                            ProfileID = ProR.ProfileID,
                            ProfileName = Pro.ProfileName,
                            RelationshipProfileID = ProR.RelationshipProfileID,
                            RelationshipProfileName = Pro1.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = ProR.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Cre.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.ProfileID).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得物件清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Objects> getObjectsData()
        {
            var query = from Obj in context.CI_Objects
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Obj.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            ProfileName = Pro.ProfileName,
                            //EditAccount = canEdit("CI_Objects", Obj.ObjectID.ToString(), ""),
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.ObjectID).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得物件清單資料
        /// </summary>
        /// <returns></returns>
        public vCI_Objects getObjectsDatabyObjectID(int ObjectID)
        {
            var query = from Obj in context.CI_Objects
                        .Where(b => b.ObjectID == ObjectID)
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Obj.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            ProfileName = Pro.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得物件清單資料
        /// </summary>
        /// <returns></returns>
        public vCI_Objects getObjectsDatabyObjectName(string ObjectName)
        {
            var query = from Obj in context.CI_Objects
                        .Where(b => b.ObjectName.Contains(ObjectName))
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Obj.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            ProfileName = Pro.ProfileName,
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得套用特定範本物件清單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Objects> getObjectsData(int ProfileID)
        {
            var query = from Obj in context.CI_Objects
                        .Where(b => b.ProfileID == ProfileID)
                        join Pro in context.CI_Proflies on Obj.ProfileID equals Pro.ProfileID
                        join Cre in context.Accounts on Obj.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Obj.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Objects
                        {
                            ObjectID = Obj.ObjectID,
                            ObjectName = Obj.ObjectName,
                            ProfileName = Pro.ProfileName,
                            //EditAccount = canEdit("CI_Objects", Obj.ObjectID.ToString(), ""),
                            Creator = Cre.Name,
                            CreateTime = Obj.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Obj.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.ObjectID).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得特定範本物件清單資料(關鍵字)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<vCI_Objects> getObjectsData(List<vSearchKeywords> SearchKeywords)
        {
            List<vCI_Objects> Result = new List<vCI_Objects>();
            var query = from Obj in context.CI_Objects
                        join ObjData in context.CI_Object_Data on Obj.ObjectID equals ObjData.ObjectID
                        select new { ObjectID = ObjData.ObjectID, ObjectName = Obj.ObjectName, AttributeID = ObjData.AttributeID, AttributeValue = ObjData.AttributeValue };

            foreach (var item in SearchKeywords)
            {
                if (item.AttributeID >= 0)
                {
                    if (item.Keyword != "" )
                    {
                        if (item.AttributeID == 0)
                        {
                            query = query
                           .Where(b => b.ObjectName.Contains(item.Keyword));
                        }
                        else
                        {
                            query = query
                          .Where(b => b.AttributeID == item.AttributeID)
                          .Where(b => b.AttributeValue.Contains(item.Keyword));

                        }
                        var query2 = query.Select(b => b.ObjectID).Distinct();

                        foreach (var q in query2.ToList())
                        {
                            vCI_Objects o = new vCI_Objects();
                            o = getObjectsDatabyObjectID(q);
                            if (o != null)
                            {
                                Result.Add(o);
                            }
                        }
                    }
                }
                else {
                    //var query = getObjectsDatabyObjectName(item.Keyword);
                }
            }

            return Result;
        }

        /// <summary>
        /// 取得範本中屬性清單
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        public IEnumerable<vCI_Attributes> getProfileAttributesData(int ProfileID)
        {
            var query = from Att in context.CI_Attributes
                        join ProAtts in context.CI_Proflie_Attributes.Where(b => b.ProfileID == ProfileID) on Att.AttributeID equals ProAtts.AttributeID
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            AttributeTypeName = AttType.AttributeTypeName,
                            AttributeOrder = ProAtts.AttributeOrder,
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.AttributeOrder).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得待覆核範本中屬性清單
        /// </summary>
        /// <param name="ProfileID">範本ID</param>
        /// <returns></returns>
        public IEnumerable<vCI_Attributes> getReviewProfileAttributesData(int ProfileID)
        {
            var query = from Att in context.CI_Attributes
                        join ProAtts in context.Tmp_CI_Proflie_Attributes.Where(b => b.ProfileID == ProfileID) on Att.AttributeID equals ProAtts.AttributeID
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            AttributeTypeName = AttType.AttributeTypeName,
                            AttributeOrder = ProAtts.AttributeOrder,
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.AttributeOrder).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得物件中屬性清單
        /// </summary>
        /// <param name="ObjectID">物件ID</param>
        /// <returns></returns>
        public IEnumerable<vCI_Attributes> getObjectAttributesData(int ObjectID)
        {
            var query = from Att in context.CI_Attributes
                        join ObjAtts in context.CI_Object_Data.Where(b => b.ObjectID == ObjectID) on Att.AttributeID equals ObjAtts.AttributeID
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            AttributeValue = ObjAtts.AttributeValue,
                            AttributeTypeID = Att.AttributeTypeID,
                            AttributeTypeName = AttType.AttributeTypeName,
                            AttributeOrder = ObjAtts.AttributeOrder,
                            AllowMutiValue = Att.AllowMutiValue,
                            DropDownValues = Att.DropDownValues,
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.AttributeOrder).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得待覆核物件中屬性清單
        /// </summary>
        /// <param name="ObjectID">物件ID</param>
        /// <returns></returns>
        public IEnumerable<vCI_Attributes> getReviewObjectAttributesData(int ObjectID)
        {
            var query = from Att in context.CI_Attributes
                        join ObjAtts in context.Tmp_CI_Object_Data.Where(b => b.ObjectID == ObjectID).Where(b => b.isClose == false) on Att.AttributeID equals ObjAtts.AttributeID
                        join AttType in context.CI_AttributeTypes on Att.AttributeTypeID equals AttType.AttributeTypeID
                        join Cre in context.Accounts on Att.CreateAccount equals Cre.Account
                        join Upd in context.Accounts on Att.UpdateAccount equals Upd.Account
                        into y
                        from x in y.DefaultIfEmpty()
                        select new vCI_Attributes
                        {
                            AttributeID = Att.AttributeID,
                            AttributeName = Att.AttributeName,
                            AttributeValue = ObjAtts.AttributeValue,
                            AttributeTypeID = Att.AttributeTypeID,
                            AttributeTypeName = AttType.AttributeTypeName,
                            AttributeOrder = ObjAtts.AttributeOrder,
                            //DropDownValues = Att.DropDownValues,
                            Creator = Cre.Name,
                            CreateTime = Att.CreateTime,
                            Upadter = x.Name,
                            UpdateTime = Att.UpdateTime
                        };

            if (query.Count() > 0)
            {
                return query.OrderBy(b => b.AttributeOrder).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Profile/Object是否含有某項屬性
        /// </summary>
        /// <param name="Type">Profile/Object</param>
        /// <param name="AttributeID">屬性ID</param>
        /// <param name="ProfileID">範本ID</param>
        /// <param name="ObjectID">物件ID</param>
        /// <returns></returns>
        public bool hasAttribute(string Type, int AttributeID, int ProfileID, int ObjectID)
        {
            bool Result = false;
            int ResultCount = 0;

            switch (Type)
            {
                case "Profile":
                    ResultCount = context.CI_Proflie_Attributes.Where(b => b.ProfileID == ProfileID)
                        .Where(b => b.AttributeID == AttributeID).Count();

                    break;
                case "Object":
                    ResultCount = context.CI_Object_Data.Where(b => b.ObjectID == ObjectID)
                        .Where(b => b.AttributeID == AttributeID).Count();

                    //case "Tmp_ObjectReverse":
                    //    ResultCount = context.Tmp_CI.Where(b => b.ObjectID == ObjectID)
                    //        .Where(b => b.AttributeID == AttributeID).Count();

                    break;

                default:
                    break;
            }

            if (ResultCount == 1)
            {
                Result = true;
            }
            else
            {

            }

            return Result;
        }

        /// <summary>
        /// 取得Entity是否能被編輯
        /// </summary>
        /// <param name="EntityName">Entity 名稱</param>
        /// <param name="EntityKey">Entity Key</param>
        /// <param name="Account">目前登入帳號</param>
        /// <returns>noBody:目前沒人編輯否則則回應該Entity編輯之帳號</returns>
        public string canEdit(string EntityName, string EntityKey, string Account)
        {
            string Result = "noBody";

            switch (EntityName)
            {
                case "CI_Attributes":
                    int AttributeID = Convert.ToInt32(EntityKey);
                    var CI_Attributes = context.Tmp_CI_Attributes.Where(b => b.isClose == false).Where(b => b.oAttributeID == AttributeID);
                    if (CI_Attributes.Count() > 0)
                    {
                        Result = CI_Attributes.First().CreateAccount;
                    }
                    break;
                case "CI_Profiles":
                    int ProfileID = Convert.ToInt32(EntityKey);
                    var CI_Profiles = context.Tmp_CI_Proflies.Where(b => b.isClose == false).Where(b => b.oProfileID == ProfileID);
                    if (CI_Profiles.Count() > 0)
                    {
                        Result = CI_Profiles.First().CreateAccount;
                    }
                    break;
                case "CI_Objects":
                    int ObjectID = Convert.ToInt32(EntityKey);
                    var CI_Objects = context.Tmp_CI_Objects.Where(b => b.isClose == false).Where(b => b.oObjectID == ObjectID);
                    if (CI_Objects.Count() > 0)
                    {
                        Result = CI_Objects.First().CreateAccount;
                    }
                    break;
                case "CI_Profile_Relationship":
                    int ProfileID1 = Convert.ToInt32(EntityKey);
                    var CI_Profile_Relationship = context.Tmp_CI_Profile_Relationship.Where(b => b.isClose == false).Where(b => b.oProfileID == ProfileID1);
                    if (CI_Profile_Relationship.Count() > 0)
                    {
                        Result = CI_Profile_Relationship.First().CreateAccount;
                    }
                    break;
                default:
                    break;
            }

            return Result;
        }

        /// <summary>
        /// 計算HASH值
        /// </summary>
        /// <param name="PlainText">明文</param>
        /// <returns>BASE64編碼的HASH值或null</returns>
        public string getHashValue(string PlainText)
        {
            //初始化系統參數
            Configer.Init();

            string ResultStr = string.Empty;

            byte[] SaltBytes = Encoding.UTF8.GetBytes(Configer.SystemSlat);

            if (SaltBytes.Length > 0)
            {
                ResultStr = SecurityProcessor.SHAEncodebyUTF8(PlainText, Configer.SystemHashAlg, true, SaltBytes).ToString();
                ResultStr = SecurityProcessor.TurnStrig2Base64byUTF8(ResultStr);

                return ResultStr;
            }
            else
            {
                return null;
            }
        }

        public vMainPage getMainPageInfo()
        {
            Configer.Init();

            vMainPage _vMainPage = new vMainPage();
            _vMainPage.NumberOfProfilePerRow = Configer.NumofgridviewPage_perrows;
            _vMainPage.NumberOfProfile = context.CI_Proflies.Count();
            _vMainPage.ProfileSearchList = ProfileSearchList();
            if (_vMainPage != null)
            {
                return _vMainPage;
            }
            else {
                return null;
            }
        }

        private IEnumerable<vMainPage_ProfileSearch> ProfileSearchList()
        {
            List<vMainPage_ProfileSearch> Result = new List<vMainPage_ProfileSearch>();

            var query = from Pro in context.CI_Proflies
                        join Img in context.SystemImgs on Pro.ImgID equals Img.ImgID
                        select new vMainPage_ProfileSearch
                        {
                            ProfileID = Pro.ProfileID,
                            ProfileName = Pro.ProfileName,
                            ImgID = Pro.ImgID,
                            ImgPath = Img.ImgPath
                        };

            if (query.Count() > 0)
            {
                //foreach (var item in query)
                //{
                //    int UsedObjectCount = getUsedObjectCount(item.ProfileID);
                //    item.UsedObjectCount = UsedObjectCount;
                //    Result.Add(item);
                //}
                return query.ToList();
            }
            else {
                return null;
            }
        }

        public int getUsedObjectCount(int ProfileID)
        {
            int UsedObjectCount = context.CI_Objects
                    .Where(b => b.ProfileID == ProfileID)
                    .Count();

            return UsedObjectCount;
        }

    }
}