namespace Ofichina.Application.Abstractions.Authentication.Service
{
    /// <summary>
    /// Contrato para acesso ao usuário autenticado na requisição atual.
    /// </summary>
    public interface IUserService
    {
        Guid? ObterUsuarioId();
    }
}
