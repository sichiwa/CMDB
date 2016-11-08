using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TWCAlib;

namespace CMDB.SystemClass
{
    public class SystemConfig
    {
        private string _Version;
        private string _VersionName;
        private int _Schedule;
        private string _LDAPName;
        private string _VAVerifyURL;
        private int _NumofgridviewPage_perrows;
        private int _SelectTopN;

        private string _C_DBConnstring;
        private string _SystemHashAlg;
        private string _SystemSlat;
        private string _SystemDateTimeFormat;
        private string _SplitSymbol;
        private string _SplitSymbol2;
        private string _SplitSymbol3;

        private int _PublicRoleID;

        private string _CreateAction;
        private string _EditAction;
        private string _RemoveAction;
        private string _InsertAction;
        private string _UpdateAction;
        private string _DeleteAction;
        private string _GetAction;
        private string _VerifyAction;
        private string _ReviewAction;
        private string _LoginAction;
        private string _LogoutAction;

        private string _UploadPath;

        private string _MailServer;
        private int _MailServerPort;
        private string _MailSender;
        private List<string> _MailReceiver;
        private List<string> _MailCC;
        private bool _MailUseSSL;
        private bool _MailBodyUseHTML;
        private string _MailPriority;
        private int _MailCodePage;
        private string _MailSubject;

        private string _MailBody;
        private List<string> _TextReceivers;
        private string _TextSubject;
        private string _TextBody;

        /// <summary>
        /// 系統版次
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        /// <summary>
        /// 程式名稱
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string VersionName
        {
            get { return _VersionName; }
            set { _VersionName = value; }
        }

        /// <summary>
        /// 系統資料庫連線字串
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string C_DBConnstring
        {
            get { return _C_DBConnstring; }
            set { _C_DBConnstring = value; }
        }

        /// <summary>
        /// 系統使用之雜湊演算法
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SystemHashAlg
        {
            get { return _SystemHashAlg; }
            set { _SystemHashAlg = value; }
        }

        /// <summary>
        /// 系統使用之Slat
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SystemSlat
        {
            get { return _SystemSlat; }
            set { _SystemSlat = value; }
        }

        /// <summary>
        /// 系統使用時間格式
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SystemDateTimeFormat
        {
            get { return _SystemDateTimeFormat; }
            set { _SystemDateTimeFormat = value; }
        }

        /// <summary>
        /// 系統使用分隔符號
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SplitSymbol
        {
            get { return _SplitSymbol; }
            set { _SplitSymbol = value; }
        }

        /// <summary>
        /// 系統使用分隔符號(2)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SplitSymbol2
        {
            get { return _SplitSymbol2; }
            set { _SplitSymbol2 = value; }
        }

        /// <summary>
        /// 系統使用分隔符號(3)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SplitSymbol3
        {
            get { return _SplitSymbol3; }
            set { _SplitSymbol3 = value; }
        }

        /// <summary>
        ///掃描頻率(單位秒)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Schedule
        {
            get { return _Schedule; }
            set { _Schedule = value; }
        }

        /// <summary>
        /// 網域登入網址
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LDAPName
        {
            get { return _LDAPName; }
            set { _LDAPName = value; }
        }

        /// <summary>
        /// VA驗章網址
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string VAVerifyURL
        {
            get { return _VAVerifyURL; }
            set { _VAVerifyURL = value; }
        }

        /// <summary>
        ///GridView分頁，每頁資料筆數
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int NumofgridviewPage_perrows
        {
            get { return _NumofgridviewPage_perrows; }
            set { _NumofgridviewPage_perrows = value; }
        }

        /// <summary>
        ///顯示最近N筆監控紀錄
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int SelectTopN
        {
            get { return _SelectTopN; }
            set { _SelectTopN = value; }
        }

        /// <summary>
        /// Public角色ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PublicRoleID
        {
            get { return _PublicRoleID; }
            set { _PublicRoleID = value; }
        }

        /// <summary>
        ///系統動作(建立)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string CreateAction
        {
            get { return _CreateAction; }
            set { _CreateAction = value; }
        }

        /// <summary>
        ///系統動作(編輯)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string EditAction
        {
            get { return _EditAction; }
            set { _EditAction = value; }
        }

        /// <summary>
        /// 系統動作(移除)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string RemoveAction
        {
            get { return _RemoveAction; }
            set { _RemoveAction = value; }
        }

        /// <summary>
        /// 系統動作(新增)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string InsertAction
        {
            get { return _InsertAction; }
            set { _InsertAction = value; }
        }

        /// <summary>
        /// 系統動作(修改)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string UpdateAction
        {
            get { return _UpdateAction; }
            set { _UpdateAction = value; }
        }

        /// <summary>
        /// 系統動作(刪除)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DeleteAction
        {
            get { return _DeleteAction; }
            set { _DeleteAction = value; }
        }

        /// <summary>
        /// 系統動作(取得)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetAction
        {
            get { return _GetAction; }
            set { _GetAction = value; }
        }

        /// <summary>
        /// 系統動作(覆核)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ReviewAction
        {
            get { return _ReviewAction; }
            set { _ReviewAction = value; }
        }

        /// <summary>
        /// 系統動作(驗證)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string VerifyAction
        {
            get { return _VerifyAction; }
            set { _VerifyAction = value; }
        }

        /// <summary>
        /// 系統動作(登入)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LoginAction
        {
            get { return _LoginAction; }
            set { _LoginAction = value; }
        }

        /// <summary>
        /// 系統動作(登出)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LogoutAction
        {
            get { return _LogoutAction; }
            set { _LogoutAction = value; }
        }

        /// <summary>
        /// 郵件伺服器IP
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MailServer
        {
            get { return _MailServer; }
            set { _MailServer = value; }
        }

        /// <summary>
        /// 檔案上傳路徑
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string UploadPath
        {
            get { return _UploadPath; }
            set { _UploadPath = value; }
        }

        /// <summary>
        /// 郵件伺服器使用Port
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int MailServerPort
        {
            get { return _MailServerPort; }
            set { _MailServerPort = value; }
        }

        /// <summary>
        /// 寄件人
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MailSender
        {
            get { return _MailSender; }
            set { _MailSender = value; }
        }

        /// <summary>
        /// 收件人
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<string> MailReceiver
        {
            get { return _MailReceiver; }
            set { _MailReceiver = value; }
        }

        /// <summary>
        /// 副本收件人
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<string> MailCC
        {
            get { return _MailCC; }
            set { _MailCC = value; }
        }

        /// <summary>
        /// 郵件是否使用SSL
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool MailUseSSL
        {
            get { return _MailUseSSL; }
            set { _MailUseSSL = value; }
        }

        /// <summary>
        /// 郵件內容是否使用HTML
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool MailBodyUseHTML
        {
            get { return _MailBodyUseHTML; }
            set { _MailBodyUseHTML = value; }
        }

        /// <summary>
        /// 郵件重要性
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MailPriority
        {
            get { return _MailPriority; }
            set { _MailPriority = value; }
        }

        /// <summary>
        /// 郵件內容編碼
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int MailCodePage
        {
            get { return _MailCodePage; }
            set { _MailCodePage = value; }
        }

        /// <summary>
        /// 郵件主旨
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MailSubject
        {
            get { return _MailSubject; }
            set { _MailSubject = value; }
        }

        /// <summary>
        /// 郵件內容
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MailBody
        {
            get { return _MailBody; }
            set { _MailBody = value; }
        }

        /// <summary>
        /// 簡訊收件人
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<string> TextReceivers
        {
            get { return _TextReceivers; }
            set { _TextReceivers = value; }
        }

        /// <summary>
        /// 簡訊主旨
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string TextSubject
        {
            get { return _TextSubject; }
            set { _TextSubject = value; }
        }

        /// <summary>
        /// 簡訊內容
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string TextBody
        {
            get { return _TextBody; }
            set { _TextBody = value; }
        }

        public void Init(string Version,string VersionName, int Schedule, int NumofgridviewPage_perrows, int SelectTopN, string C_DBConnstring,
        string SystemHashAlg,string SystemSlat, string SystemDateTimeFormat, string SplitSymbol, string SplitSymbol2, string SplitSymbol3,
        int PublicRoleID,
        string CreateAction,string EditAction,string RemoveAction,string InsertAction,string UpdateAction,string DeleteAction,string GetAction,string ReviewAction, string VerifyAction, string LoginAction,string LogoutAction,
        string UploadPath,
        string MailServer, int MailServerPort, string MailSender, List<string> MailReceiver, List<string> MailCC, bool MailUseSSL, bool MailBodyUseHTML, string MailPriority, int MailCodePage, string MailSubject, string MailBody,
        List<string> TextReceivers,
        string TextSubject, string TextBody)
        {
            Char[] splitsymbol = { ',' };

            this.Version = Version;
            this.VersionName = VersionName;
            this.Schedule = Schedule;

            this.NumofgridviewPage_perrows = NumofgridviewPage_perrows;
            this.SelectTopN = Convert.ToInt16(SelectTopN);

            this.C_DBConnstring = SecurityProcessor.TurnBase642String(C_DBConnstring);
            this.SystemHashAlg = SystemHashAlg;
            this.SystemSlat = SystemSlat;
            this.SystemDateTimeFormat = SystemDateTimeFormat;
            this.SplitSymbol = SplitSymbol;
            this.SplitSymbol2 = SplitSymbol2;
            this.SplitSymbol3 = SplitSymbol3;

            this.PublicRoleID = PublicRoleID;
            this.CreateAction = CreateAction;
            this.EditAction = EditAction;
            this.RemoveAction = RemoveAction;
            this.InsertAction = InsertAction;
            this.UpdateAction = UpdateAction;
            this.DeleteAction = DeleteAction;
            this.GetAction = GetAction;
            this.ReviewAction = ReviewAction;
            this.VerifyAction = VerifyAction;
            this.LoginAction = LoginAction;
            this.LogoutAction = LogoutAction;

            this.UploadPath = UploadPath;

            this.MailServer = MailServer;
            this.MailServerPort = Convert.ToInt16(MailServerPort);
            this.MailSender = MailSender;
            this.MailReceiver = MailReceiver;
            this.MailCC = MailCC;
            this.MailUseSSL = MailUseSSL;
            this.MailBodyUseHTML = MailBodyUseHTML;
            this.MailPriority = MailPriority;
            this.MailCodePage = MailCodePage;
            this.MailSubject = MailSubject;
            this.MailBody = MailBody;

            this.TextReceivers = TextReceivers;
            this.TextSubject = TextSubject;
            this.TextBody = TextBody;
        }

        public void Init()
        {
            Char[] splitsymbol = { ',' };

            this.Version = WebConfigurationManager.AppSettings["Version"];
            this.VersionName = WebConfigurationManager.AppSettings["VersionName"];
            this.Schedule = Convert.ToInt16(WebConfigurationManager.AppSettings["Schedule"]);

            this.LDAPName= WebConfigurationManager.AppSettings["LDAPName"];
            this.NumofgridviewPage_perrows = Convert.ToInt16(WebConfigurationManager.AppSettings["NumofgridviewPage_perrows"]);
            this.SelectTopN = Convert.ToInt16(WebConfigurationManager.AppSettings["SelectTopN"]);

            //this.C_DBConnstring = WebConfigurationManager.ConnectionStrings["C_DBConnstring"].ToString();//SecurityProcessor.TurnBase642String(WebConfigurationManager.ConnectionStrings["C_DBConnstring"].ToString());
            this.SystemHashAlg = WebConfigurationManager.AppSettings["SystemHashAlg"];
            this.SystemSlat= WebConfigurationManager.AppSettings["SystemSlat"];
            this.SystemDateTimeFormat = WebConfigurationManager.AppSettings["SystemDateTimeFormat"];
            this.SplitSymbol = WebConfigurationManager.AppSettings["SplitSymbol"];
            this.SplitSymbol2 = WebConfigurationManager.AppSettings["SplitSymbol2"];
            this.SplitSymbol3 = WebConfigurationManager.AppSettings["SplitSymbol3"];

            this.PublicRoleID = Convert.ToInt16(WebConfigurationManager.AppSettings["PublicRoleID"]);

            this.CreateAction = WebConfigurationManager.AppSettings["CreateAction"];
            this.EditAction = WebConfigurationManager.AppSettings["EditAction"];
            this.RemoveAction = WebConfigurationManager.AppSettings["RemoveAction"];
            this.InsertAction = WebConfigurationManager.AppSettings["InsertAction"];
            this.UpdateAction = WebConfigurationManager.AppSettings["UpdateAction"];
            this.DeleteAction = WebConfigurationManager.AppSettings["DeleteAction"];
            this.GetAction = WebConfigurationManager.AppSettings["GetAction"];
            this.ReviewAction = WebConfigurationManager.AppSettings["ReviewAction"];
            this.VerifyAction = WebConfigurationManager.AppSettings["VerifyAction"];
            this.LoginAction = WebConfigurationManager.AppSettings["LoginAction"];
            this.LogoutAction = WebConfigurationManager.AppSettings["LogoutAction"];

            this.UploadPath= WebConfigurationManager.AppSettings["UploadPath"];

            this.MailServer = WebConfigurationManager.AppSettings["MailServer"];
            this.MailServerPort = Convert.ToInt16(WebConfigurationManager.AppSettings["MailServerPort"]);
            this.MailSender = WebConfigurationManager.AppSettings["MailSender"];
            this.MailReceiver = StringProcessor.SplitString2Array(WebConfigurationManager.AppSettings["MailReceiver"], splitsymbol);
            this.MailCC = StringProcessor.SplitString2Array(WebConfigurationManager.AppSettings["MailCC"], splitsymbol);
            this.MailUseSSL = Convert.ToBoolean(WebConfigurationManager.AppSettings["MailUseSSL"]);
            this.MailBodyUseHTML = Convert.ToBoolean(WebConfigurationManager.AppSettings["MailBodyUseHTML"]);
            this.MailPriority = WebConfigurationManager.AppSettings["MailPriority"];
            this.MailCodePage = Convert.ToInt32(WebConfigurationManager.AppSettings["MailCodePage"]);
            this.MailSubject = WebConfigurationManager.AppSettings["MailSubject"];
            this.MailBody = WebConfigurationManager.AppSettings["MailBody"];

            this.TextReceivers = StringProcessor.SplitString2Array(WebConfigurationManager.AppSettings["TextReceivers"], splitsymbol);
            this.TextSubject = WebConfigurationManager.AppSettings["TextSubject"];
            this.TextBody = WebConfigurationManager.AppSettings["TextBody"];
        }
    }
}