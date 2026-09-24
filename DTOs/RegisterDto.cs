using System.ComponentModel.DataAnnotations;

namespace GoldShopAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string ShopId { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShopName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}