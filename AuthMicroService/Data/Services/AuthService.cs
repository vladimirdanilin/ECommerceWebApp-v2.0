using AuthMicroService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroService.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        { 
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUser(RegisterModel registerModel)
        {
            var normalizedEmail = _userManager.NormalizeEmail(registerModel.Email);

            User user = new User
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                NormalizedEmail = normalizedEmail,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
                if (!roleResult.Succeeded)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public async Task<string> LoginUser(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null)
            {
                return "Login Was NOT Successful";
            }

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

            if (!result.Succeeded)
            {
                return "Login Was NOT Successful";
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return GenerateJwtToken(user, role);
        }

        public string GenerateJwtToken(User user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
