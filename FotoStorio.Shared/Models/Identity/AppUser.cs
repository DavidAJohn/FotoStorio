using Microsoft.AspNetCore.Identity;

namespace FotoStorio.Shared.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
    }
}