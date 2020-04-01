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
        public static string TagManager => "Tag Manager";
        public static string UserManager => "User Manager";
        public static string CreatorManager => "Creator Manager";
        public static string PendingTags => "Pending Tags";
        public static string TagPairManager => "Tag Pair Manager";



        public static string MinisNavClass(ViewContext viewContext) => PageNavClass(viewContext, Minis);
        public static string TagManagerNavClass(ViewContext viewContext) => PageNavClass(viewContext, TagManager);
        public static string UserManagerNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserManager);
        public static string CreatorManagerNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreatorManager);
        public static string PendingTagsNavClass(ViewContext viewContext) => PageNavClass(viewContext, PendingTags);
        public static string TagPairManagerNavClass(ViewContext viewContext) => PageNavClass(viewContext, TagPairManager);



        private static string PageNavClass(ViewContext viewContext, string page)
        {
            string activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return String.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}