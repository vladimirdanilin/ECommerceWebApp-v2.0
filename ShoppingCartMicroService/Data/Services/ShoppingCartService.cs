using Microsoft.EntityFrameworkCore;
using ShoppingCartMicroService.Data.Models;

namespace ShoppingCartMicroService.Data.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public ShoppingCartService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient("ProductMicroService");
        }

        public async Task<ShoppingCart> GetCartByUserIdAsync(int userId)
        {
            var cart = await _context.ShoppingCarts.Include(sc => sc.CartItems).FirstOrDefaultAsync(sc => sc.UserId == userId);

            return cart;
        }

        public async Task AddItemToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await _context.ShoppingCarts.Include(sc => sc.CartItems).FirstOrDefaultAsync(sc => sc.UserId == userId);

            decimal productPrice = 0;

            var response = await _httpClient.GetAsync($"product/getProductPrice/{productId}");

            if (response.IsSuccessStatusCode)
            {
                productPrice = await response.Content.ReadFromJsonAsync<decimal>();
            }

            if (cart == null)
            {
                cart = new ShoppingCart()
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.ShoppingCarts.Add(cart);

                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.TotalUnitPrice = Math.Round(productPrice * cartItem.Quantity, 2);
            }

            if (cartItem == null)
            {
                cartItem = new CartItem()
                {
                    ProductId = productId,
                    Quantity = quantity,
                    TotalUnitPrice = Math.Round(productPrice * quantity, 2),
                    ShoppingCartId = cart.Id
                };

                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(int userId, int productId)
        { 
            var cart = await GetCartByUserIdAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

             if (cartItem == null)
             {
                return;
             }

            decimal productPrice = 0;

            var response = await _httpClient.GetAsync($"product/getProductPrice/{productId}");

            if (response.IsSuccessStatusCode)
            {
                productPrice = await response.Content.ReadFromJsonAsync<decimal>();
            }

            if (cartItem.Quantity > 0)
            {
                cartItem.Quantity--;
                cartItem.TotalUnitPrice = Math.Round(cartItem.Quantity * productPrice, 2);
                _context.CartItems.Update(cartItem);
            }

            if (cartItem.Quantity == 0)
            { 
                //cart.CartItems.Remove(cartItem);
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearShoppingCartAsync(int userId)
        { 
            var shoppingCartToClear = await _context.ShoppingCarts.Include(sc => sc.CartItems).FirstOrDefaultAsync(sc => sc.UserId == userId);

            if (shoppingCartToClear != null)
            {
                _context.CartItems.RemoveRange(shoppingCartToClear.CartItems);

                await _context.SaveChangesAsync();
            }
        }
    }
}
