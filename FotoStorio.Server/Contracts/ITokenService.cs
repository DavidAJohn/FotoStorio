using System.Threading.Tasks;
using FotoStorio.Shared.Models.Identity;

namespace FotoStorio.Server.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}