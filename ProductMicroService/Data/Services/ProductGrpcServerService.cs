using Grpc.Core;
using static ProductMicroService.ProductGrpcService;

namespace ProductMicroService.Data.Services
{
    public class ProductGrpcServerService : ProductGrpcServiceBase
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;

        public ProductGrpcServerService(IProductService productService, IInventoryService inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService;
        }

        public override async Task<ProductResponse> CheckProductAvailability(ProductRequest request, ServerCallContext context)
        {
            var stockQuantity = await _inventoryService.GetProductQuantityAsync(Convert.ToInt32(request.ProductId));

            var price = await _productService.GetProductPriceAsync(Convert.ToInt32(request.ProductId));

            return new ProductResponse
            {
                IsAvailable = stockQuantity >= request.RequestedQuantity,
                CurrentPrice = (double)price
            };
        }
    }
}
