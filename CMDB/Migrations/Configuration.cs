namespace CMDB.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CMDB.DAL.CMDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CMDB.DAL.CMDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //context.Roles.Add(new Roles()
            //{
            //    //RoleID = 1,
            //    RoleName = "Admin",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Roles.Add(new Roles()
            //{
            //    //RoleID = 2,
            //    RoleName = "Manager",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Roles.Add(new Roles()
            //{
            //    //RoleID = 3,
            //    RoleName = "Public",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 1,
            //    FunctionName = "系統設定",
            //   // ParentID = 0,
            //    Url = "#1",
            //    ShowOrder = 10,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 11,
            //    FunctionName = "紀錄檢視",
            //    Controller = "Log",
            //    Action = "Search",
            //    ParentID = 1,
            //    ShowOrder = 11,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 12,
            //    FunctionName = "帳號管理",
            //    Controller = "Account",
            //    Action = "Index",
            //    ParentID = 1,
            //    ShowOrder = 12,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 13,
            //    FunctionName = "角色管理",
            //    Controller = "Role",
            //    Action = "Index",
            //    ParentID = 1,
            //    ShowOrder = 13,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 14,
            //    FunctionName = "選單管理",
            //    Controller = "Function",
            //    Action = "Index",
            //    ParentID = 1,
            //    ShowOrder = 14,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 14,
            //    FunctionName = "組態批次匯入",
            //    Controller = "組態批次匯入",
            //    Action = "Index",
            //    ParentID = 1,
            //    ShowOrder = 15,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 2,
            //    FunctionName = "組態管理",
            //    //Controller = "Account",
            //    //Action = "Index",
            //    Url = "#2",
            //    //ParentID = 0,
            //    ShowOrder = 20,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 21,
            //    FunctionName = "屬性管理",
            //    Controller = "Attribute",
            //    Action = "Index",
            //    ParentID = 2,
            //    ShowOrder = 21,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 22,
            //    FunctionName = "範本管理",
            //    Controller = "Profile",
            //    Action = "Index",
            //    ParentID = 2,
            //    ShowOrder = 22,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 23,
            //    FunctionName = "物件管理",
            //    Controller = "Object",
            //    Action = "Index",
            //    ParentID = 2,
            //    ShowOrder = 23,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 100,
            //    FunctionName = "登出",
            //    Controller = "Account",
            //    Action = "Logout",
            //    ParentID = 0,
            //    ShowOrder = 100,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Functions.Add(new Functions()
            //{
            //    FunctionID = 99,
            //    FunctionName = "關於",
            //    Controller = "Main",
            //    Action = "About",
            //    ParentID = null,
            //    ShowOrder = 99,
            //    IsEnable = true,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 1,
            //    Authority=1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 2,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 11,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 12,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 13,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 14,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 15,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 21,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 22,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 23,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 99,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 1,
            //    FunctionID = 100,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});


            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 1,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 11,
            //    Authority = 0,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 2,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 21,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 22,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 23,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 100,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 2,
            //    FunctionID = 99,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 1,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 11,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 2,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 21,
            //    Authority = 0,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 22,
            //    Authority = 0,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 23,
            //    Authority = 0,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.RoleFunctions.Add(new RoleFunctions()
            //{
            //    RoleID = 3,
            //    FunctionID = 100,
            //    Authority = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS170",
            //    Name = "黃富彥",
            //    //Pwd = "",
            //    Email = "fuyen@twca.com.tw",
            //    RoleID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable=true
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS191",
            //    Name = "黃士瑋",
            //    //Pwd = "",
            //    //Email = "fuyen@twca.com.tw",
            //    RoleID = 2,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable = true
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS015",
            //    Name = "郭清章",
            //    //Pwd = "",
            //    //Email = "fuyen@twca.com.tw",
            //    RoleID = 2,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable = true
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS046",
            //    Name = "葉峰谷",
            //    //Pwd = "",
            //    //Email = "fuyen@twca.com.tw",
            //    RoleID = 2,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable = true
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS103",
            //    Name = "林依諴",
            //    //Pwd = "",
            //    //Email = "fuyen@twca.com.tw",
            //    RoleID = 2,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable = true
            //});

            //context.Accounts.Add(new Accounts()
            //{
            //    Account = "TAS105",
            //    Name = "陳漢榮",
            //    //Pwd = "",
            //    //Email = "fuyen@twca.com.tw",
            //    RoleID = 2,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    isEnable = true
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 1,
            //    AttributeTypeName = "text",
            //    Description = "一般文字輸入",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 2,
            //    AttributeTypeName = "textarea",
            //    Description = "一般文字輸入(多行)",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 3,
            //    AttributeTypeName = "datetime",
            //    Description = "日期輸入",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 4,
            //    AttributeTypeName = "dropdown",
            //    Description = "下拉式選單輸入",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "CPU",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose=true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID=1,
            //    AttributeName ="CPU",
            //    AttributeTypeID=1,
            //    CreateAccount="TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "Memory",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 2,
            //    AttributeName = "Memory",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "Disk",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 3,
            //    AttributeName = "Disk",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "OS Family",
            //    AttributeTypeID = 4,
            //    DropDownValues= "Windows 2008 r2#Windows 2012 r2#Cent OS 6#OS X#solaris 10#solaris 11",
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 4,
            //    AttributeName = "OS Family",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Windows 2008 r2#Windows 2012 r2#Cent OS 6#OS X#solaris 10#solaris 11",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "購買日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 5,
            //    AttributeName = "購買日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "上線日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 6,
            //    AttributeName = "上線日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "汰換日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 7,
            //    AttributeName = "汰換日期",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "主機負責人",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 8,
            //    AttributeName = "主機負責人",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "系統負責人",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 9,
            //    AttributeName = "系統負責人",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "Middleware",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Acer#Apple#Asus#Dell#HP#Lenovo#Oracle",
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 10,
            //    AttributeName = "Middleware",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Apache2.2#Apache2.4#IIS6.0#IIS7.0#IIS7.5#IIS8.0#IIS8.5#Tomcat6.0#Tomcat7.0#Tomcat8.0#Weblogic 10#Weblogic 11#Weblogic12",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "Database",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Oracle9i#Oracle10g#Oracle11g#SQL Server 2005#SQL Server 2008#SQL Server 2008r2#SQL Server 2012#SQL Server 2012r2",
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 11,
            //    AttributeName = "Database",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Apache2.2#Apache2.4#IIS6.0#IIS7.0#IIS7.5#IIS8.0#IIS8.5#Tomcat6.0#Tomcat7.0#Tomcat8.0#Weblogic 10#Weblogic 11#Weblogic12",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "硬體廠牌",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Acer#Apple#Asus#Dell#HP#Lenovo#Oracle",
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 12,
            //    AttributeName = "硬體廠牌",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Acer#Apple#Asus#Dell#HP#Lenovo#Oracle",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "資產編號",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    ReviewAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    ReviewTime = DateTime.Now,
            //    isClose = true
            //});

            //context.CI_Attributes.Add(new CI_Attributes()
            //{
            //    AttributeID = 13,
            //    AttributeName = "資產編號",
            //    AttributeTypeID =1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID=1,
            //    ImgName="系統預設",
            //    Description="預設",
            //    ImgPath= "~/Content/SystemImgs/Default.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 2,
            //    ImgName = "主機",
            //    Description = "主機",
            //    ImgPath = "~/Content/SystemImgs/Server.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 3,
            //    ImgName = "軟體",
            //    Description = "軟體",
            //    ImgPath = "~/Content/SystemImgs/Software.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 4,
            //    ImgName = "資料庫",
            //    Description = "資料庫",
            //    ImgPath = "~/Content/SystemImgs/Database.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 5,
            //    ImgName = "網路設備",
            //    Description = "網路設備",
            //    ImgPath = "~/Content/SystemImgs/Networkdevice.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 6,
            //    ImgName = "人員",
            //    Description = "人員",
            //    ImgPath = "~/Content/SystemImgs/People.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});
        }
    }
}
