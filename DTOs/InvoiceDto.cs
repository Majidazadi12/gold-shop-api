using System.ComponentModel.DataAnnotations;
using GoldShopAPI.Models;  // Add this using for InvoiceItem

namespace GoldShopAPI.DTOs
{
    public class InvoiceItemDto
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
    }

    public class CreateInvoiceDto
    {
        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [MaxLength(20)]
        public string? CustomerPhone { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Required]
        [MinLength(1)]
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceResponseDto
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public List<InvoiceItem> Items { get; set; } = new();  // Now InvoiceItem is accessible
        public DateTime CreatedAt { get; set; }
    }
}