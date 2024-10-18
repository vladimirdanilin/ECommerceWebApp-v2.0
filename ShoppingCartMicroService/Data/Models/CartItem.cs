using System.ComponentModel.DataAnnotations;

namespace ShoppingCartMicroService.Data.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalUnitPrice { get; set; }

        public int ShoppingCartId { get; set; }
    }
}
