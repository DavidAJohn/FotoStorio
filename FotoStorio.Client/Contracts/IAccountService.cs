using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Identity;

namespace FotoStorio.Client.Contracts
{
    public interface IAccountService
    {
        Task<AddressDTO> GetUserAddressAsync();
        Task<AddressDTO> SaveUserAddressAsync(AddressDTO address);
    }
}