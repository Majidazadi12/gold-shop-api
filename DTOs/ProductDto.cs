using System.ComponentModel.DataAnnotations;

namespace GoldShopAPI.DTOs
{
    public class ProductDto
    {
        public long Id { get; set; }  // Using long
        public string ShopId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Karat { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Weight { get; set; }

        [Required]
        [Range(10, 24)]
        public int Karat { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 0;

        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProductDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Weight { get; set; }

        [Range(10, 24)]
        public int? Karat { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        public string? Description { get; set; }
    }
}