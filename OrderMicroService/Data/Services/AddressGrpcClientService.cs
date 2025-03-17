using Grpc.Net.Client;

namespace OrderMicroService.Data.Services
{
    public class AddressGrpcClientService
    {
        private readonly AddressGrpcService.AddressGrpcServiceClient _client;

        public AddressGrpcClientService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5005");

            _client = new AddressGrpcService.AddressGrpcServiceClient(channel);
        }

        public async Task<AddressResponse> GetUserDefaultAddress(int userId)
        {
            var request = new AddressRequest
            {
                UserId = userId
            };

            return await _client.GetUserDefaultAddressAsync(request);
        }
    }
}
