using AuthMicroService.Models;

namespace AuthMicroService.Data.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegisterModel registerModel);

        Task<string> LoginUser(LoginModel loginModel);

        string GenerateJwtToken(User user, string role);
    }
}
