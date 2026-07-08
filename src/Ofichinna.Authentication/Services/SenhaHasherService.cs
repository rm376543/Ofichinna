using Ofichinna.Authentication.Abstractions;
using Ofichinna.Authentication.Security;

namespace Ofichinna.Authentication.Services;

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