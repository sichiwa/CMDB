using System.Data.Entity;
using CMDB.Models;

namespace CMDB.DAL
{
    public class CMDBContext : DbContext
    {
        // 您的內容已設定為使用應用程式組態檔 (App.config 或 Web.config)
        // 中的 'CMDBContext' 連接字串。根據預設，這個連接字串的目標是
        // 您的 LocalDb 執行個體上的 'CMDB.DAL.CMDBContext' 資料庫。
        // 
        // 如果您的目標是其他資料庫和 (或) 提供者，請修改
        // 應用程式組態檔中的 'CMDBContext' 連接字串。
        public CMDBContext()
            : base("name=CMDBContext")
        {
        }

        // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
        // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。

        public DbSet<CI_Attributes> CI_Attributes { get; set; }
        public DbSet<Tmp_CI_Attributes> Tmp_CI_Attributes { get; set; }

        public DbSet<CI_Proflies> CI_Proflies { get; set; }
        public DbSet<Tmp_CI_Proflies> Tmp_CI_Proflies { get; set; }

        public DbSet<CI_Proflie_Attributes> CI_Proflie_Attributes { get; set; }
        public DbSet<Tmp_CI_Proflie_Attributes> Tmp_CI_Proflie_Attributes { get; set; }

        public DbSet<CI_AttributeTypes> CI_AttributeTypes { get; set; }
        public DbSet<Tmp_CI_AttributeTypes> Tmp_CI_AttributeTypes { get; set; }

        public DbSet<CI_Objects> CI_Objects { get; set; }
        public DbSet<Tmp_CI_Objects> Tmp_CI_Objects { get; set; }

        public DbSet<CI_Object_Data> CI_Object_Data { get; set; }
        public DbSet<Tmp_CI_Object_Data> Tmp_CI_Object_Data { get; set; }
       
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Tmp_Accounts> Tmp_Accounts { get; set; }

        public DbSet<Roles> Roles { get; set; }
        public DbSet<Tmp_Roles> Tmp_Roles { get; set; }

        public DbSet<Functions> Functions { get; set; }
        public DbSet<Tmp_Functions> Tmp_Functions { get; set; }

        public DbSet<RoleFunctions> RoleFunctions { get; set; }
        public DbSet<Tmp_RoleFunctions> Tmp_RoleFunctions { get; set; }

        public DbSet<SystemImgs> SystemImgs { get; set; }
        public DbSet<Tmp_SystemImgs> Tmp_SystemImgs { get; set; }

        public DbSet<SystemLogs> SystmeLogs { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

}