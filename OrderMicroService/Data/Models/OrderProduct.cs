﻿namespace OrderMicroService.Data.Models
{
    public class OrderProduct
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
