using Core.Framework.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Framework.Common.Extension
{
    public static class PagedListExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int page, int pageSize)
        {
            return new PagedList<T>(source, page, pageSize);
        }
    }
}