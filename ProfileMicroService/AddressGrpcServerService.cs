using Grpc.Core;
using ProfileMicroService.Services;

namespace ProfileMicroService
{
    public class AddressGrpcServerService : AddressGrpcService.AddressGrpcServiceBase
    {
        private readonly IAddressService _addressService;

        public AddressGrpcServerService(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public override async Task<AddressResponse> GetUserDefaultAddress(AddressRequest request, ServerCallContext context)
        {
            var defaultAddressId = await _addressService.GetDefaultAddressIdAsync(request.UserId);

            return new AddressResponse
            {
                AddressId = defaultAddressId
            };
        }
    }
}
