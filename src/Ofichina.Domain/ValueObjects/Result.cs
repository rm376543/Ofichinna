namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Resultado padrão para operações da aplicação.
/// Encapsula sucesso, falha e mensagens de erro.
/// </summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public IReadOnlyCollection<string> Errors { get; }

    private Result(bool isSuccess, string? error = null, IReadOnlyCollection<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors: errors.ToList());

    public static Result<T> Success<T>(T value) => new(true, value);
    public static Result<T> Failure<T>(string error) => new(false, default, error);
    public static Result<T> Failure<T>(IEnumerable<string> errors) => new(false, default, errors: errors.ToList());
}

/// <summary>
/// Resultado genérico para operações que retornam um valor.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public IReadOnlyCollection<string> Errors { get; }

    public Result(bool isSuccess, T? value = default, string? error = null, IReadOnlyCollection<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors: errors.ToList());
}
