using Grpc.Net.Client;

namespace OrderMicroService.Data.Services
{
    public class ProductGrpcClientService
    {
        private readonly ProductGrpcService.ProductGrpcServiceClient _client;

        public ProductGrpcClientService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7103");

            _client = new ProductGrpcService.ProductGrpcServiceClient(channel);
        }

        public async Task<ProductResponse> CheckProductAvailability(int productId, int requestedQuantity)
        {
            var request = new ProductRequest
            {
                ProductId = productId.ToString(),
                RequestedQuantity = requestedQuantity
            };

            return await _client.CheckProductAvailabilityAsync(request);
        }
    }
}
