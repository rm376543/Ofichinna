using Ofichina.Authentication.Abstractions;
using Ofichina.Authentication.Security;

namespace Ofichina.Authentication.Services;

public sealed class SenhaHasherService : ISenhaHasher
{
    public string GerarHash(string senha)
    {
        return PasswordHasher.Hash(senha);
    }

    public bool Verificar(string senha, string hash)
    {
        return PasswordHasher.Verify(senha, hash);
    }
}