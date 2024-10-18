using AuthMicroService.Data.DTOs;
using AuthMicroService.Models;

namespace AuthMicroService.Data.Services
{
    public interface IUserService
    {
        Task<User> GetUserDataAsync(int userId);

        Task EditUserDataAsync(UserDTO userDTO);

        Task EditUserRoleAsync(int userId, string role);
    }
}
