using System.Threading.Tasks;
using FotoStorio.Shared.Auth;

namespace FotoStorio.Client.Contracts
{
    public interface IAuthenticationService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
    }
}