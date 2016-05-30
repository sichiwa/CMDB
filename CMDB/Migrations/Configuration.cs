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
            //    FunctionName = "�t�γ]�w",
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
            //    FunctionName = "�����˵�",
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
            //    FunctionName = "�b���޲z",
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
            //    FunctionName = "����޲z",
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
            //    FunctionName = "���޲z",
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
            //    FunctionName = "�պA�妸�פJ",
            //    Controller = "�պA�妸�פJ",
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
            //    FunctionName = "�պA�޲z",
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
            //    FunctionName = "�ݩʺ޲z",
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
            //    FunctionName = "�d���޲z",
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
            //    FunctionName = "����޲z",
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
            //    FunctionName = "�n�X",
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
            //    FunctionName = "����",
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
            //    Name = "���I��",
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
            //    Name = "���h޳",
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
            //    Name = "���M��",
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
            //    Name = "���p��",
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
            //    Name = "�L���",
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
            //    Name = "���~�a",
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
            //    Description = "�@���r��J",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 2,
            //    AttributeTypeName = "textarea",
            //    Description = "�@���r��J(�h��)",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 3,
            //    AttributeTypeName = "datetime",
            //    Description = "�����J",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.CI_AttributeTypes.Add(new CI_AttributeTypes()
            //{
            //    AttributeTypeID = 4,
            //    AttributeTypeName = "dropdown",
            //    Description = "�U�Ԧ�����J",
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
            //    AttributeName = "�ʶR���",
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
            //    AttributeName = "�ʶR���",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "�W�u���",
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
            //    AttributeName = "�W�u���",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "�O�����",
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
            //    AttributeName = "�O�����",
            //    AttributeTypeID = 3,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "�D���t�d�H",
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
            //    AttributeName = "�D���t�d�H",
            //    AttributeTypeID = 1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "�t�έt�d�H",
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
            //    AttributeName = "�t�έt�d�H",
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
            //    AttributeName = "�w��t�P",
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
            //    AttributeName = "�w��t�P",
            //    AttributeTypeID = 4,
            //    DropDownValues = "Acer#Apple#Asus#Dell#HP#Lenovo#Oracle",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.Tmp_CI_Attributes.Add(new Tmp_CI_Attributes()
            //{
            //    AttributeName = "�겣�s��",
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
            //    AttributeName = "�겣�s��",
            //    AttributeTypeID =1,
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS191",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID=1,
            //    ImgName="�t�ιw�]",
            //    Description="�w�]",
            //    ImgPath= "~/Content/SystemImgs/Default.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 2,
            //    ImgName = "�D��",
            //    Description = "�D��",
            //    ImgPath = "~/Content/SystemImgs/Server.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 3,
            //    ImgName = "�n��",
            //    Description = "�n��",
            //    ImgPath = "~/Content/SystemImgs/Software.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 4,
            //    ImgName = "��Ʈw",
            //    Description = "��Ʈw",
            //    ImgPath = "~/Content/SystemImgs/Database.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 5,
            //    ImgName = "�����]��",
            //    Description = "�����]��",
            //    ImgPath = "~/Content/SystemImgs/Networkdevice.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});

            //context.SystemImgs.Add(new SystemImgs()
            //{
            //    ImgID = 6,
            //    ImgName = "�H��",
            //    Description = "�H��",
            //    ImgPath = "~/Content/SystemImgs/People.png",
            //    CreateAccount = "TAS170",
            //    UpdateAccount = "TAS170",
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now
            //});
        }
    }
}
