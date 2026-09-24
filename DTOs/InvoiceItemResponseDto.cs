namespace GoldShopAPI.DTOs
{
    public class InvoiceItemResponseDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Karat { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}