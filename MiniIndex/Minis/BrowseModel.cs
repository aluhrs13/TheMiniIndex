using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System.Linq;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public BrowseModel(MiniSearchModel searchModel, PaginatedList<Mini> minis)
        {
            SearchModel = searchModel;
            Minis = minis ?? PaginatedList.Empty<Mini>();
        }

        public MiniSearchModel SearchModel { get; set; }
        public PaginatedList<Mini> Minis { get; set; }
    }
}