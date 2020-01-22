using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System.Linq;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public BrowseModel(SearchParametersModel searchModel, PaginatedList<Mini> minis)
        {
            SearchModel = searchModel;
            Minis = minis ?? PaginatedList.Empty<Mini>();
        }

        public SearchParametersModel SearchModel { get; set; }
        public PaginatedList<Mini> Minis { get; set; }
    }
}