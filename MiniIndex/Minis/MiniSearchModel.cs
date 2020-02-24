using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Core.Minis.Search;

namespace MiniIndex.Minis
{
    public class MiniSearchModel
    {
        public string SearchString { get; set; }

        public string Tags { get; set; }

        public bool FreeOnly { get; set; }

        public SearchSupportingInfo SupportingInfo { get; set; }
        public SelectList TagOptions { get; internal set; }
    }
}