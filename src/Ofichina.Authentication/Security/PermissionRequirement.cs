using Microsoft.AspNetCore.Authorization;

namespace Ofichina.Authentication.Security;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission.Trim();
    }

    public string Permission { get; }
}