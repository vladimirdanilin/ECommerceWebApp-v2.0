using System.ComponentModel.DataAnnotations;

namespace OrderMicroService.Data.DTOs
{
    public class CartItemDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalUnitPrice { get; set; }
    }
}