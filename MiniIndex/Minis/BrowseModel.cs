using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public BrowseModel(MiniSearchModel searchModel, IEnumerable<Mini> minis)
        {
            SearchModel = searchModel;
            Minis = minis ?? Enumerable.Empty<Mini>();
        }

        public MiniSearchModel SearchModel { get; set; }
        public IEnumerable<Mini> Minis { get; set; }
    }
}