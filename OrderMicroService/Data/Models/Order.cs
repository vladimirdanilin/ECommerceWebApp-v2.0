using System.ComponentModel.DataAnnotations;

namespace OrderMicroService.Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateOnly OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Total { get; set; }

        //Relationships

        public ICollection<OrderProduct>? OrdersProducts { get; set; }

        public int UserId { get; set; }

        public int ShippingAddressId { get; set; }
    }
}
