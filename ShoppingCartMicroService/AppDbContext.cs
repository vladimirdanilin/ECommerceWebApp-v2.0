using Microsoft.EntityFrameworkCore;
using ShoppingCartMicroService.Data.Models;

namespace ShoppingCartMicroService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
        
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
