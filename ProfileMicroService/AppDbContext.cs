using Microsoft.EntityFrameworkCore;
using ProfileMicroService.Models;

namespace ProfileMicroService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
        
        }

        public DbSet<Address> Addresses { get; set; }
    }
}
