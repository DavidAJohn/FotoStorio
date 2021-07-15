using System.Threading.Tasks;
using FotoStorio.Shared.Entities;

namespace FotoStorio.Client.Contracts
{
    public interface IAuthenticationService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
    }
}