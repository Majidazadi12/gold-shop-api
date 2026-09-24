using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GoldShopAPI.Models
{
    public class Shop
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ShopId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string ShopName { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}