using Microsoft.EntityFrameworkCore;
using ProductMicroService.Data.Models;

namespace ProductMicroService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        
        }

        public DbSet<Product> Products { get; set; }
    }
}
