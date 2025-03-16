using Microsoft.EntityFrameworkCore;
using OrderMicroService.Data.DTOs;
using OrderMicroService.Data.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace OrderMicroService.Data.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _shoppingCartClient;
        private readonly HttpClient _profileClient;
        private readonly HttpClient _productClient;
        private readonly MessageProducer _messageProducer;
        private readonly ProductGrpcClientService _productGrpcClientService;

        public OrderService(AppDbContext context, IHttpClientFactory httpClientFactory, MessageProducer messageProducer, ProductGrpcClientService productGrpcClientService)
        {
            _context = context;
            _shoppingCartClient = httpClientFactory.CreateClient("ShoppingCartMicroService");
            _profileClient = httpClientFactory.CreateClient("ProfileMicroService");
            _productClient = httpClientFactory.CreateClient("ProductMicroService");
            _messageProducer = messageProducer;
            _productGrpcClientService = productGrpcClientService;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var userOrders = await _context.Orders
                .Include(o => o.OrdersProducts)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return userOrders;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        { 
            var allOrders = await _context.Orders.Include(o => o.OrdersProducts).ToListAsync();

            return allOrders;
        }

        public async Task<int> PlaceOrderAndGetIdAsync(int userId)
        {
            var cartResponse = await _shoppingCartClient.GetAsync($"ShoppingCart/getByUserId/{userId}");

            if (!cartResponse.IsSuccessStatusCode)
            {
                throw new Exception("Cart is Empty or NotFound");
            }

            var shoppingCartDTO = await cartResponse.Content.ReadFromJsonAsync<ShoppingCartDTO>();

            /*foreach (var item in shoppingCartDTO.CartItemDTOs)
            {
                var productResponse = await _productClient.GetAsync($"Product/getProductQuantity/{item.ProductId}");

                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Product NotFound");
                }

                var productQuantityInStock = await productResponse.Content.ReadFromJsonAsync<int>();

                if (item.Quantity > productQuantityInStock)
                {
                    throw new Exception("Not Enough Products In Stock");
                }
            }*/

            foreach (var item in shoppingCartDTO.CartItemDTOs)
            {
                var quantityPriceResponse = await _productGrpcClientService.CheckProductAvailability(item.ProductId, item.Quantity);

                if (quantityPriceResponse.IsAvailable == false)
                {
                    throw new Exception("Product Is Not Available");
                }

                item.TotalUnitPrice = (decimal)quantityPriceResponse.CurrentPrice * item.Quantity;
            }

            var addressResponse = await _profileClient.GetAsync($"Profile/getDefaultAddressId/{userId}");

            if (!addressResponse.IsSuccessStatusCode)
            {
                throw new Exception("Address Id Was Not Found");
            }

            var selectedAddressId = await addressResponse.Content.ReadFromJsonAsync<int>();

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                ShippingAddressId = selectedAddressId,
                Status = OrderStatus.Pending,
                Total = shoppingCartDTO.CartItemDTOs.Sum(item => item.TotalUnitPrice),
                OrdersProducts = shoppingCartDTO.CartItemDTOs.Select(cartItem => new OrderProduct
                { 
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in shoppingCartDTO.CartItemDTOs)
            {
                //await _productClient.PutAsJsonAsync($"Product/decreaseProductQuantity/{item.ProductId}", item.Quantity);

                var message = new
                {
                    item.ProductId,
                    item.Quantity
                };

                var jsonMessage = JsonSerializer.Serialize(message);

                _messageProducer.SendMessage(jsonMessage, "decrease_product_quantity_queue");
            }

            await _shoppingCartClient.DeleteAsync($"ShoppingCart/clear/{userId}");

            return order.Id;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        { 
            var order = await _context.Orders.Include(o => o.OrdersProducts).FirstOrDefaultAsync(o => o.Id == orderId);

            return order;
        }

        public async Task EditOrderStatusAsync(int orderId, OrderStatus orderStatus)
        {
            var order = await _context.Orders.Include(o => o.OrdersProducts).FirstOrDefaultAsync(o => o.Id == orderId);

            if ((orderStatus == OrderStatus.Declined || orderStatus == OrderStatus.Cancelled)
                && order.Status != OrderStatus.Declined && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.OrdersProducts)
                {
                    await _productClient.PutAsJsonAsync($"Product/increaseProductQuantity/{item.ProductId}", item.Quantity);
                }
            }

            if ((orderStatus == OrderStatus.Pending || orderStatus == OrderStatus.Processing
                || orderStatus == OrderStatus.Ready ||  orderStatus == OrderStatus.Delivered) 
                && order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing
                && order.Status != OrderStatus.Ready && order.Status != OrderStatus.Delivered)
            {
                foreach (var item in order.OrdersProducts)
                {
                    await _productClient.PutAsJsonAsync($"Product/decreaseProductQuantity/{item.ProductId}", item.Quantity);
                }
            }

            order.Status = orderStatus;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
