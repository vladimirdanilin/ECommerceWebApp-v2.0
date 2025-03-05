using Grpc.Core;
using static ProductMicroService.ProductGrpcService;

namespace ProductMicroService.Data.Services
{
    public class ProductGrpcService : ProductGrpcServiceBase
    {
        public override async Task<ProductResponse> CheckProductAvailability(ProductRequest request, ServerCallContext context)
        {
            return new ProductResponse
            {
                IsAvailable = true,
                CurrentPrice = 100
            };
        }
    }
}
