using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FotoStorio.Server.Extensions;

public interface IUserManagerExtensionsWrapper
{
    Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(UserManager<AppUser> userManager, ClaimsPrincipal principal);
    Task<AppUser> FindUserByEmailFromClaimsPrincipal(UserManager<AppUser> userManager, ClaimsPrincipal principal);
}
