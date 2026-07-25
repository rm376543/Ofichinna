using Ofichina.Contracts.Common;

namespace Ofichina.Contracts;

public static class PagedResponseExtensions
{
    public static PagedResponse<TSource> ToPagedResponse<TSource>(
        this IEnumerable<TSource> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items);

        return new PagedResponse<TSource>
        {
            Items = items.ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
            HasNextPage = pageNumber * pageSize < totalCount,
            HasPreviousPage = pageNumber > 1
        };
    }

    public static PagedResponse<TResult> ToPagedResponse<TSource, TResult>(
        this PagedResponse<TSource> source,
        Func<TSource, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return new PagedResponse<TResult>
        {
            Items = source.Items.Select(selector).ToList(),
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
            TotalPages = source.TotalPages,
            HasNextPage = source.HasNextPage,
            HasPreviousPage = source.HasPreviousPage
        };
    }
}