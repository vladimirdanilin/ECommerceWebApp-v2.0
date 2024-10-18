using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileMicroService.Models;
using ProfileMicroService.Services;

namespace ProfileMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProfileController : Controller
    {
        private readonly IAddressService _addressService;

        public ProfileController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("addAddress")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            await _addressService.AddAddressAsync(address, userId);

            return Ok();
        }

        [HttpDelete("deleteAddress/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            await _addressService.DeleteAddressAsync(addressId, userId);

            return Ok();
        }

        [HttpGet("getUserAddresses/{userId}")]
        public async Task<IActionResult> GetUserAddresses(int userId)
        {
            var userAddresses = await _addressService.GetUserAddressesByUserIdAsync(userId);

            return Ok(userAddresses);
        }

        [HttpPut("makeAddressDefault/{userId}/{addressId}")]
        public async Task<IActionResult> MakeAddressDefault(int userId, int addressId)
        { 
            await _addressService.MakeAddressDefaultAsync(userId, addressId);

            return Ok();
        }

        [HttpGet("getDefaultAddressId/{userId}")]
        public async Task<IActionResult> GetDefaultAddressId(int userId)
        { 
            var defaultAddress = await _addressService.GetDefaultAddressIdAsync(userId);

            return Ok(defaultAddress);
        }
    }
}