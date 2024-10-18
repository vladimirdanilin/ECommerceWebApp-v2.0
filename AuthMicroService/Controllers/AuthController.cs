using AuthMicroService.Data.DTOs;
using AuthMicroService.Data.Services;
using AuthMicroService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        { 
            //Automatic model validity check completed

            var result = await _authService.RegisterUser(model);

            if (result == true)
            { 
                return Ok("User Registration Completed");
            }

            return BadRequest("User Registration NOT Completed");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //Automatic model validity check completed

            var jwtToken = await _authService.LoginUser(model);

            if (jwtToken != "Login Was NOT Successful")
            {
                return Ok(new { Token = jwtToken });
            }

            return Unauthorized(jwtToken);
        }

        [HttpGet("getUserData/{userId}")]
        public async Task<IActionResult> GetUserData(int userId)
        {
            var userData = await _userService.GetUserDataAsync(userId);

            return Ok(userData);
        }

        [HttpPost("editUserData/{userDTO}")]
        public async Task<IActionResult> EditUserData([FromBody]UserDTO userDTO)
        { 
            await _userService.EditUserDataAsync(userDTO);

            return Ok("User Data Has Been Successfully Edited");
        }

        [HttpPost("editUserRole/{userId}")]
        public async Task<IActionResult> EditUserRole(int userId, [FromQuery] string role)
        {
            await _userService.EditUserRoleAsync(userId, role);

            return Ok();
        }
    }
}
