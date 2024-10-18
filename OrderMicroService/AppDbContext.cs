using Microsoft.EntityFrameworkCore;
using OrderMicroService.Data.Models;

namespace OrderMicroService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>().HasKey(op => new
            {
                op.OrderId,
                op.ProductId
            });
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrdersProducts { get; set; }
    }
}
