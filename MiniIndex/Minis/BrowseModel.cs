using MiniIndex.Models;
using System.Collections.Generic;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public MiniSearchModel SearchModel { get; set; }
        public IEnumerable<Mini> Minis { get; set; }
    }
}