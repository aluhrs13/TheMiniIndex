namespace MiniIndex.Core.Pagination
{
    public class PageInfo
    {
        public PageInfo(int pageSize, int pageIndex)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public int PageSize { get; }
        public int PageIndex { get; }
    }
}