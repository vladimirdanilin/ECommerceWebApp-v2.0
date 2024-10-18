using AuthMicroService.Data.DTOs;
using AuthMicroService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroService.Data.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        { 
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> GetUserDataAsync(int userId)
        {
            var userData = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return userData;
        }

        public async Task EditUserDataAsync(UserDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(userDTO.UserId.ToString());

            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.PhoneNumber = userDTO.PhoneNumber;
            user.ProfilePictureURL = userDTO.ProfilePictureURL;

            await _userManager.UpdateAsync(user);
        }

        public async Task EditUserRoleAsync(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Contains(role) && await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
