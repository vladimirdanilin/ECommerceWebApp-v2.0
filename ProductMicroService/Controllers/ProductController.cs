using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductMicroService.Data;
using ProductMicroService.Data.DTOs;
using ProductMicroService.Data.Services;

namespace ProductMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public readonly IInventoryService _inventoryService;

        public ProductController(IProductService productService, IInventoryService inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService;
        }

        [HttpPost("addNew")]
        public async Task<IActionResult> AddProduct(NewProductDTO newProductDTO)
        { 
            var product = await _productService.AddProductAsync(newProductDTO);

            return Ok(product);
        }

        [HttpGet("getById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        { 
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            { 
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("getByCategory")]
        public async Task<IActionResult> GetProductsByCategory(ProductCategory? productCategory)
        {
            var productsInCategory = await _productService.GetProductsByCategoryAsync(productCategory);

            return Ok(productsInCategory);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchForProduct(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString) || searchString.Length > 255)
            { 
                return BadRequest();
            }

            var searchedProducts = await _productService.SearchForProductAsync(searchString);

            return Ok();
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditProduct(ProductDTO productDTO)
        {
            await _productService.EditProductAsync(productDTO);

            return Ok();
        }

        [HttpPost("removeFromSale/{productId}")]
        public async Task<IActionResult> RemoveProductFromSale(int productId)
        { 
            await _productService.RemoveProductFromSaleAsync(productId);

            return Ok();
        }

        [HttpPost("returnToSale/{productId}")]
        public async Task<IActionResult> ReturnProductToSale(int productId)
        { 
            await _productService.ReturnProductToSaleAsync(productId);

            return Ok();
        }

        [HttpGet("getNotAvailableProducts")]
        public async Task<IActionResult> GetNotAvailableProducts()
        { 
            var notAvailableProducts = await _productService.GetNotAvailableProductsAsync();

            return Ok(notAvailableProducts);
        }

        [HttpGet("getProductPrice/{productId}")]
        public async Task<IActionResult> GetProductPrice(int productId)
        { 
            var productPrice = await _productService.GetProductPriceAsync(productId);

            return Ok(productPrice);
        }

        [HttpPut("increaseProductQuantity/{productId}")]
        public async Task<IActionResult> IncreaseProductQuantity(int productId, [FromBody] int quantity)
        {
            await _inventoryService.IncreaseProductQuantityAsync(productId, quantity);

            return Ok();
        }

        [HttpPut("decreaseProductQuantity/{productId}")]
        public async Task<IActionResult> decreaseProductQuantity(int productId, [FromBody] int quantity)
        {
            await _inventoryService.DecreaseProductQuantityAsync(productId, quantity);

            return Ok();
        }

        [HttpPut("updateProductQuantity/{productId}")]
        public async Task<IActionResult> UpdateProductQuantity(int productId, [FromBody] int quantity)
        {
            await _inventoryService.UpdateProductQuantityAsync(productId, quantity);

            return Ok();
        }

        [HttpGet("getProductQuantity/{productId}")]
        public async Task<IActionResult> GetProductQuantity(int productId)
        { 
            var quantity = await _inventoryService.GetProductQuantityAsync(productId);

            return Ok(quantity);
        }

        [HttpGet("ok")]
        public async Task<IActionResult> Okay()
        {
            return Ok("OkayVOVA");
        }
    }
}
