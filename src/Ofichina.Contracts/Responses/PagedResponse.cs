namespace Ofichina.Contracts;

/// <summary>
/// Representa uma resposta paginada contendo uma coleção de itens do tipo T.
/// </summary>
/// <typeparam name="T">O tipo dos itens na resposta paginada.</typeparam>
public sealed class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int ItemCount => Items.Count;
    public int FirstItemIndex => ItemCount == 0 ? 0 : ((PageNumber - 1) * PageSize) + 1;
    public int LastItemIndex => ItemCount == 0 ? 0 : FirstItemIndex + ItemCount - 1;
    public bool IsEmpty => ItemCount == 0;
}