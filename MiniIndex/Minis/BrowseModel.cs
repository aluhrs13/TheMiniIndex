using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System;
using System.Linq;

namespace MiniIndex.Minis
{
    public class BrowseModel
    {
        public BrowseModel(MiniSearchModel searchModel, PaginatedList<Mini> minis)
        {
            SearchModel = searchModel;
            Minis = minis ?? PaginatedList.Empty<Mini>();

            Random rand = new Random();
            ShowRateResults = (rand.Next(0, 24) == 0);
        }

        public MiniSearchModel SearchModel { get; set; }
        public PaginatedList<Mini> Minis { get; set; }
        public bool ShowRateResults { get; set; }
    }
}