using System.ComponentModel.DataAnnotations;

namespace ShoppingCartMicroService.Data.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public List<CartItem>? CartItems { get; set; }

        public int UserId { get; set; }
    }
}
