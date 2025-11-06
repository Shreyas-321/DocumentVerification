using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApitest.DTOs;
using webApitest.Services;

namespace webApitest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userResponse = await _userService.RegisterUserAsync(userRegisterDto);
                
                return Ok(new { 
                    message = "User registered successfully", 
                    user = userResponse 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.ValidateUserAsync(userLoginDto.Email, userLoginDto.Password);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var token = _jwtService.GenerateToken(user);
                var userResponse = _userService.MapToUserResponseDto(user);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    User = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto adminLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.ValidateAdminAsync(adminLoginDto.Username, adminLoginDto.Password);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid admin credentials" });
                }

                var token = _jwtService.GenerateToken(user);
                var userResponse = _userService.MapToUserResponseDto(user);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    User = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during admin login" });
            }
        }

        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyToken()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during token verification" });
            }
        }
    }
}
