using Ofichina.Contracts;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.Abstractions.Common;

public static class PagedResponseExtensions
{
    public static PagedResponse<TResult> ToPagedResponse<TSource, TResult>(
        this PagedResult<TSource> source,
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