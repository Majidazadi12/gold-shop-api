using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GoldShopAPI.Data;
using GoldShopAPI.DTOs;
using GoldShopAPI.Models;
using GoldShopAPI.Helpers;

namespace GoldShopAPI.Services
{
    public interface IAuthService
    {
        Task<object> Register(RegisterDto registerDto);
        Task<object> Login(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IJwtHelper jwtHelper, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<object> Register(RegisterDto registerDto)
        {
            // Check if shop exists
            var existingShop = await _context.Shops
                .FirstOrDefaultAsync(s => s.ShopId == registerDto.ShopId);
            
            if (existingShop != null)
            {
                throw new Exception("Shop ID already exists");
            }

            var existingUser = await _context.Shops
                .FirstOrDefaultAsync(s => s.Username == registerDto.Username);
            
            if (existingUser != null)
            {
                throw new Exception("Username already exists");
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create shop
            var shop = new Shop
            {
                ShopId = registerDto.ShopId,
                Username = registerDto.Username,
                Password = hashedPassword,
                ShopName = registerDto.ShopName,
                Phone = registerDto.Phone ?? "",
                Address = registerDto.Address ?? "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtHelper.GenerateToken(shop);

            return new
            {
                message = "Shop registered successfully",
                token,
                shop = new
                {
                    shop.ShopId,
                    shop.Username,
                    shop.ShopName,
                    shop.Phone,
                    shop.Address
                }
            };
        }

        public async Task<object> Login(LoginDto loginDto)
        {
            var shop = await _context.Shops
                .FirstOrDefaultAsync(s => s.Username == loginDto.Username);

            if (shop == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, shop.Password);
            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Generate token
            var token = _jwtHelper.GenerateToken(shop);

            return new
            {
                message = "Login successful",
                token,
                shop = new
                {
                    shop.ShopId,
                    shop.Username,
                    shop.ShopName,
                    shop.Phone,
                    shop.Address
                }
            };
        }
    }
}