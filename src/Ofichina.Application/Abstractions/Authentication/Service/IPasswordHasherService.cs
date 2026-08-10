namespace Ofichina.Application.Abstractions.Authentication.Service
{
    /// <summary>
    /// Contrato para hash e validação de senha.
    /// </summary>
    public interface IPasswordHasherService
    {
        string GerarHash(string senha);
        bool Verificar(string senha, string hash);
    }

}
