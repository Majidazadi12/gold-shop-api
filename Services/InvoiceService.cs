using Microsoft.EntityFrameworkCore;
using GoldShopAPI.Data;
using GoldShopAPI.DTOs;
using GoldShopAPI.Models;

namespace GoldShopAPI.Services
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetInvoices(string shopId);
        Task<Invoice> GetInvoice(long id, string shopId);
        Task<Invoice> CreateInvoice(string shopId, CreateInvoiceDto dto);
        Task<object> GetSummary(string shopId);
        Task<object> GetSalesByPayment(string shopId);
        Task<object> GetDailySales(string shopId, int days = 7);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(ApplicationDbContext context, IProductService productService, ILogger<InvoiceService> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        public async Task<List<Invoice>> GetInvoices(string shopId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.ShopId == shopId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invoices;
        }

        public async Task<Invoice> GetInvoice(long id, string shopId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.ShopId == shopId);

            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }

            return invoice;
        }

        public async Task<Invoice> CreateInvoice(string shopId, CreateInvoiceDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new Exception("Invoice must have at least one item");
            }

            decimal totalAmount = 0;
            var invoiceItems = new List<InvoiceItem>();

            foreach (var item in dto.Items)
            {
                var product = await _productService.GetProduct(item.ProductId, shopId);
                
                if (item.Quantity > product.Quantity)
                {
                    throw new Exception($"Insufficient stock for {product.Name}. Available: {product.Quantity}");
                }

                var price = item.Price ?? product.Price;
                var subtotal = price * item.Quantity;
                totalAmount += subtotal;

                invoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Category = product.Category,
                    Weight = product.Weight,
                    Karat = product.Karat,
                    Quantity = item.Quantity,
                    Price = price,
                    Subtotal = subtotal
                });

                // Update stock
                await _productService.UpdateStock(product.Id, shopId, product.Quantity - item.Quantity);
            }

            var invoice = new Invoice
            {
                ShopId = shopId,
                InvoiceNumber = $"INV-{shopId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                CustomerName = dto.CustomerName ?? "Walk-in Customer",
                CustomerPhone = dto.CustomerPhone ?? "",
                TotalAmount = totalAmount,
                PaymentMethod = dto.PaymentMethod ?? "Cash",
                Items = invoiceItems
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<object> GetSummary(string shopId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.ShopId == shopId)
                .ToListAsync();

            var totalInvoices = invoices.Count;
            var totalRevenue = invoices.Sum(i => i.TotalAmount);
            var totalItems = invoices.Sum(i => i.Items.Count);

            return new
            {
                shopId,
                totalInvoices,
                totalRevenue,
                totalItems
            };
        }

        public async Task<object> GetSalesByPayment(string shopId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.ShopId == shopId)
                .ToListAsync();

            var paymentGroups = invoices
                .GroupBy(i => i.PaymentMethod)
                .Select(g => new
                {
                    payment_method = g.Key,
                    count = g.Count(),
                    total = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(x => x.total)
                .ToList();

            return paymentGroups;
        }

        public async Task<object> GetDailySales(string shopId, int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            var dailySales = await _context.Invoices
                .Where(i => i.ShopId == shopId && i.CreatedAt >= startDate)
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key,
                    invoice_count = g.Count(),
                    total_sales = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(x => x.date)
                .ToListAsync();

            return dailySales;
        }
    }
}