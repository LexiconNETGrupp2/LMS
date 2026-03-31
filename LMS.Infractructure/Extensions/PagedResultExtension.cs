using LMS.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Extensions;

public static class PagedResultExtensions
{
    extension<T>(IQueryable<T> source)
    {
        public async Task<PagedResult<T>> ToPagedResultAsync(
            PagedQuery query,
            CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(query.Page, PagedQuery.MinPage, nameof(query.Page));
            ArgumentOutOfRangeException.ThrowIfLessThan(query.PageSize, PagedQuery.MinPageSize, nameof(query.PageSize));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(query.PageSize, PagedQuery.MaxPageSize, nameof(query.PageSize));
            var totalItems = await source.CountAsync(cancellationToken);
            var items = await source
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }
    }
}
