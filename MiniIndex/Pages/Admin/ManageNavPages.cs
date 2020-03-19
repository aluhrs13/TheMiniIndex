using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniIndex.Pages.Admin
{
    public static class ManageNavPages
    {
        public static string Minis => "Minis";
        public static string Tags => "Tags";
        public static string Users => "Users";
        public static string Creators => "Creators";
        public static string TagStream => "TagStream";


        public static string MinisNavClass(ViewContext viewContext) => PageNavClass(viewContext, Minis);
        public static string TagsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Tags);
        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);
        public static string CreatorsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Creators);
        public static string TagStreamNavClass(ViewContext viewContext) => PageNavClass(viewContext, TagStream);


        private static string PageNavClass(ViewContext viewContext, string page)
        {
            string activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return String.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}