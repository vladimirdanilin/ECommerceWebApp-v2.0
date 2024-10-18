using OrderMicroService.Data.Models;

namespace OrderMicroService.Data.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);

        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<int> PlaceOrderAndGetIdAsync (int userId);

        Task<Order> GetOrderByIdAsync (int orderId);

        Task EditOrderStatusAsync(int orderId, OrderStatus orderStatus);
    }
}
