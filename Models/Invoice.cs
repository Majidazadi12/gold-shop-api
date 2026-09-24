using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GoldShopAPI.Models
{
    public class InvoiceItem
    {
        public long ProductId { get; set; }  // Using long
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Karat { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }  // Using long
        
        [Required]
        [MaxLength(50)]
        public string ShopId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "Walk-in Customer";
        
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Cash";
        
        [Required]
        [Column(TypeName = "jsonb")]
        public string ItemsJson { get; set; } = "[]";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [JsonIgnore]
        public Shop? Shop { get; set; }
        
        [NotMapped]
        public List<InvoiceItem> Items 
        { 
            get => string.IsNullOrEmpty(ItemsJson) ? new List<InvoiceItem>() : 
                   System.Text.Json.JsonSerializer.Deserialize<List<InvoiceItem>>(ItemsJson) ?? new List<InvoiceItem>();
            set => ItemsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}