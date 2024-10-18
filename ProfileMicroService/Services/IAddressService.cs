using ProfileMicroService.Models;

namespace ProfileMicroService.Services
{
    public interface IAddressService
    {
        Task AddAddressAsync(Address address, int userId);

        Task DeleteAddressAsync(int addressId, int userId);

        Task<IEnumerable<Address>> GetUserAddressesByUserIdAsync(int userId);

        Task MakeAddressDefaultAsync(int userId, int addressId);

        Task<int> GetDefaultAddressIdAsync(int userId);
    }
}
