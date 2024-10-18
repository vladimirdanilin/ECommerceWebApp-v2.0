using ProductMicroService.Data.DTOs;
using ProductMicroService.Data.Models;

namespace ProductMicroService.Data.Services
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(NewProductDTO addProductDTO);

        Task<Product> GetProductByIdAsync(int Id);

        Task<IReadOnlyCollection<Product>> SearchForProductAsync(string searchString);

        Task EditProductAsync(ProductDTO productDTO);

        Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory? productCategory);

        Task RemoveProductFromSaleAsync(int productId);

        Task ReturnProductToSaleAsync(int productId);

        Task<IEnumerable<Product>> GetNotAvailableProductsAsync();

        Task<decimal> GetProductPriceAsync(int productId);
    }
}
