using ShoppingCartMicroService.Data.Models;

namespace ShoppingCartMicroService.Data.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart> GetCartByUserIdAsync(int userId);

        Task AddItemToCartAsync(int userId, int productId, int quantity);

        Task RemoveItemFromCartAsync(int userId, int productId);

        Task ClearShoppingCartAsync(int userId);
    }
}
