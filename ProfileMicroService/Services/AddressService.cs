using Microsoft.EntityFrameworkCore;
using ProfileMicroService.Models;

namespace ProfileMicroService.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAddressAsync(Address address, int userId)
        { 
            await _context.Addresses.AddAsync(address);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(int addressId, int userId)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address != null)
            {
                _context.Addresses.Remove(address);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Address>> GetUserAddressesByUserIdAsync(int userId)
        { 
            var userAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();

            return userAddresses.AsEnumerable();
        }

        public async Task MakeAddressDefaultAsync(int userId, int addressId)
        {
            var currentDefaultAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefaultAddress == true);

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (currentDefaultAddress != null)
            {
                currentDefaultAddress.IsDefaultAddress = false;

                _context.Addresses.Update(currentDefaultAddress);
            }

            address.IsDefaultAddress = true;

            _context.Addresses.Update(address);

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetDefaultAddressIdAsync(int userId)
        { 
            var defaultAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefaultAddress == true);

            return defaultAddress.Id;
        }
    }
}
