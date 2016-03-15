using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;
using CMDB.DAL;
using CMDB.Models;

namespace CMDB.Models
{
    public class MyDynamicNodeProvider:DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodes)
        {
            var returnValue = new List<DynamicNode>();

            using (CMDBContext context =new CMDBContext())
            {
                //var LoginUserID = HttpContext.Current.Session["UserID"].ToString();
                //int UserRole = Convert.ToInt32(HttpContext.Current.Session["UserRole"].ToString());
                var LoginUserID = "TAS170";
                int UserRole = 1;
                var query = from rm in context.RoleFunctions
                            where rm.RoleID == UserRole
                            join f in context.Functions on rm.FunctionID equals f.FunctionID into memu
                            from x in memu.DefaultIfEmpty()
                            select new
                            {
                                ParentID = x.ParentID,
                                FunctionID = x.FunctionID,
                                FunctionName = x.FunctionName,
                                Controller=x.Controller,
                                Action=x.Action,
                                Url= x.Url,
                                ShowOrder=x.ShowOrder
                            };
                var SysMenus = query.OrderBy(c=>c.ShowOrder).ToList();

                foreach (var menu in SysMenus)
                {
                    DynamicNode Node = new DynamicNode()
                    {
                        Title = menu.FunctionName,
                        ParentKey= menu.ParentID > 0 ? menu.ParentID.ToString():"",
                        Key=menu.FunctionID.ToString(),
                        Controller=menu.Controller,
                        Action=menu.Action,
                        Url=menu.Url
                    };

                    returnValue.Add(Node);
                }
            }

            return returnValue;
        }
    }
}