using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartMicroService.Data.Services;
using System.Security.Claims;

namespace ShoppingCartMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService) 
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            return Ok(cart);
        }

        [HttpPost("addItemToCart")]
        public async Task<IActionResult> AddItemToCart(int productId, int quantity)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            await _shoppingCartService.AddItemToCartAsync(userId, productId, quantity);

            return Ok();
        }

        [HttpDelete("removeItemFromCart/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            await _shoppingCartService.RemoveItemFromCartAsync(userId, productId);

            return Ok();
        }

        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearShoppingCart(int userId)
        {
            await _shoppingCartService.ClearShoppingCartAsync(userId);

            return Ok();
        }

        [HttpGet("ok")]
        public async Task<IActionResult> Okay()
        {
            return Ok("OkayVOVA");
        }
    }
}