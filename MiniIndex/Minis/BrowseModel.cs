using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public BrowseModel(SearchParametersModel searchModel, IEnumerable<Mini> minis)
        {
            SearchModel = searchModel;
            Minis = minis ?? Enumerable.Empty<Mini>();
        }

        public SearchParametersModel SearchModel { get; set; }
        public IEnumerable<Mini> Minis { get; set; }
    }
}