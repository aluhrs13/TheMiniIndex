using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Pagination
{
    public class PaginatedList
    {
        public static async Task<PaginatedList<T>> CreateAsync<T>(IQueryable<T> source, PageInfo pageInfo)
        {
            var count = await source.CountAsync();

            var items = await source
                .Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize)
                .Take(pageInfo.PageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageInfo.PageIndex, pageInfo.PageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsync<T>(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = await source.CountAsync();

            List<T> items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsync<T>(IQueryable<T> source, int count, int pageIndex, int pageSize)
        {
            List<T> items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }

        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }
    }
}