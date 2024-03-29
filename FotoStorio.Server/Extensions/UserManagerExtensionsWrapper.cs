using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FotoStorio.Server.Extensions;

public class UserManagerExtensionsWrapper : IUserManagerExtensionsWrapper
{
    public async Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(UserManager<AppUser> userManager, ClaimsPrincipal principal)
    {
        return await userManager.FindUserByClaimsPrincipalWithAddressAsync(principal);
    }

    public async Task<AppUser> FindUserByEmailFromClaimsPrincipal(UserManager<AppUser> userManager, ClaimsPrincipal principal)
    {
        return await userManager.FindUserByEmailFromClaimsPrincipal(principal);
    }
}
