using Ofichinna.Authentication.Security;

namespace Ofichina.Infrastructure.Persistence.Seeds;

public static class AuthSeed
{
    public static readonly Guid AdminPerfilId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid AdminUsuarioId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public const string AdminPassword = "Admin@123";

    private static readonly byte[] AdminSalt = "OfichinaAdminSalt"u8.ToArray();

    public static readonly string AdminPasswordHash = PasswordHasher.Hash(AdminPassword, AdminSalt);
}