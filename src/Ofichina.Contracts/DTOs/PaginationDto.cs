namespace Ofichina.Contracts.DTOs;

/// <summary>
/// DTO para paginação.
/// </summary>
public class PaginationDto
{
    /// <summary>
    /// Número da página (começa em 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; set; } = 10;

    public PaginationDto()
    {
    }

    public PaginationDto(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public int GetSkip()
    {
        return (PageNumber - 1) * PageSize;
    }
}

/// <summary>
/// Resultado paginado de uma busca.
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// Total de registros encontrados.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número da página atual.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
