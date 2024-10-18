using Microsoft.EntityFrameworkCore;
using ProductMicroService.Data.DTOs;
using ProductMicroService;
using ProductMicroService.Data.Models;

namespace ProductMicroService.Data.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddProductAsync(NewProductDTO newProductDTO)
        {
            var product = new Product
            {
                Name = newProductDTO.Name,
                Description = newProductDTO.Description,
                PictureURL = newProductDTO.PictureURL,
                Price = newProductDTO.Price,
                Quantity = newProductDTO.Quantity,
                ProductCategory = newProductDTO.ProductCategory,
                AvailableForSale = newProductDTO.AvailableForSale,
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == Id);

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory? productCategory)
        {
            var productsInCategory = await _context.Products
                .Where(p => p.AvailableForSale == true && (p.ProductCategory == productCategory || productCategory == null))
                .ToListAsync();

            return productsInCategory;
        }

        public async Task<IReadOnlyCollection<Product>> SearchForProductAsync(string searchString)
        {
            var searchedProducts = await _context.Products
                .Where(p => p.Name.ToLower().Contains(searchString.ToLower()) && p.AvailableForSale == true)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    PictureURL = p.PictureURL,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    ProductCategory = p.ProductCategory,
                    TotalNumberOfStars = p.TotalNumberOfStars,
                    TotalNumberOfRates = p.TotalNumberOfRates,
                    AvailableForSale = p.AvailableForSale
                }).ToListAsync();

            return searchedProducts;
        }

        public async Task EditProductAsync(ProductDTO productDTO)
        {
            var productToEdit = await _context.Products.FirstOrDefaultAsync(p => p.Id == productDTO.Id);

            productToEdit.Name = productDTO.Name;
            productToEdit.Description = productDTO.Description;
            productToEdit.PictureURL = productDTO.PictureURL;
            productToEdit.Price = productDTO.Price;
            productToEdit.Quantity = productDTO.Quantity;
            productToEdit.ProductCategory = productDTO.ProductCategory;
            productToEdit.AvailableForSale = productDTO.AvailableForSale;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromSaleAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            product.AvailableForSale = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task ReturnProductToSaleAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            product.AvailableForSale = true;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetNotAvailableProductsAsync()
        { 
            var notAvailableProducts = await _context.Products
                .Where(p => p.AvailableForSale == false)
                .ToListAsync();

            return notAvailableProducts;
        }

        public async Task<decimal> GetProductPriceAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            return product.Price;
        }
    }
}
