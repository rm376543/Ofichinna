namespace Ofichina.Contracts.Requests;

/// <summary>
/// Classe base para requisições de criação e atualização.
/// </summary>
public abstract class BaseRequest
{
}

/// <summary>
/// Requisição padrão para criar uma entidade.
/// </summary>
public class CreateRequest : BaseRequest
{
}

/// <summary>
/// Requisição padrão para atualizar uma entidade.
/// </summary>
public class UpdateRequest : BaseRequest
{
    public Guid Id { get; set; }
}
