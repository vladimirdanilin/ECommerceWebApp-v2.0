using Microsoft.EntityFrameworkCore;

namespace ProductMicroService.Data.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context) 
        {
            _context = context;
        }

        public async Task IncreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            product.Quantity += quantity;

            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        public async Task DecreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            product.Quantity -= quantity;

            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductQuantityAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            product.Quantity = quantity;

            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetProductQuantityAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            return product.Quantity;
        }
    }
}
