using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderMicroService.Data;
using OrderMicroService.Data.Services;

namespace OrderMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        { 
            _orderService = orderService;
        }

        [HttpGet("getUserOrders/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var userOrders = await _orderService.GetOrdersByUserIdAsync(userId);

            return Ok(userOrders);
        }

        [HttpGet("getAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        { 
            var allOrders = await _orderService.GetAllOrdersAsync();

            return Ok(allOrders);
        }

        [HttpPost("placeOrder")]
        public async Task<IActionResult> PlaceOrderAndGetId()
        {
            //int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            var orderId = await _orderService.PlaceOrderAndGetIdAsync(1);

            return Ok(orderId);
        }

        [HttpGet("getOrderById/{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(order);
        }

        [HttpPost("editOrderStatus")]
        public async Task<IActionResult> EditOrderStatus(int orderId, OrderStatus orderStatus)
        { 
            await _orderService.EditOrderStatusAsync(orderId, orderStatus);

            return Ok();
        }
    }
}
