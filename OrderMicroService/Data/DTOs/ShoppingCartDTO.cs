using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderMicroService.Data.DTOs
{
    public class ShoppingCartDTO
    {
        [JsonPropertyName("CartItems")]
        public List<CartItemDTO>? CartItemDTOs { get; set; }

        public int UserId { get; set; }
    }
}
