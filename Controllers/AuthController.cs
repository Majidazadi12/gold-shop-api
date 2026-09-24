using Microsoft.AspNetCore.Mvc;
using GoldShopAPI.DTOs;
using GoldShopAPI.Services;

namespace GoldShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.Register(registerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify")]
        public IActionResult Verify()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return Ok(new
                {
                    valid = true,
                    shop = new
                    {
                        shopId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                        username = User.Identity.Name,
                        shopName = User.FindFirst("ShopName")?.Value
                    }
                });
            }

            return Unauthorized(new { message = "Invalid token" });
        }
    }
}