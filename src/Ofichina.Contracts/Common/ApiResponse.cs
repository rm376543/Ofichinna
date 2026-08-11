namespace Ofichina.Contracts.Common;

/// <summary>
/// Resposta base para todas as APIs.
/// Padroniza o formato de retorno das requisições.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem descritiva da operação.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Lista de erros, se houver.
    /// </summary>
    public IEnumerable<string> Errors { get; set; } = [];

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string? message = null)
    {
        Success = success;
        Message = message;
    }

    public ApiResponse(bool success, IEnumerable<string> errors)
    {
        Success = success;
        Errors = errors;
    }

    public static ApiResponse SuccessResponse(string? message = null)
        => new(true, message);

    public static ApiResponse FailureResponse(string error)
        => new(false, error);

    public static ApiResponse FailureResponse(IEnumerable<string> errors)
        => new(false, errors);
}

/// <summary>
/// Resposta genérica para APIs que retornam dados.
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Dados da resposta.
    /// </summary>
    public T? Data { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, T? data = default, string? message = null)
        : base(success, message)
    {
        Data = data;
    }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new(true, data, message);

    public static new ApiResponse<T> FailureResponse(string error)
        => new(false, default, error);

    public static new ApiResponse<T> FailureResponse(IEnumerable<string> errors)
        => new(false, default) { Errors = errors };
}
