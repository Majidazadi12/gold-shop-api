using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoldShopAPI.DTOs;
using GoldShopAPI.Services;

namespace GoldShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        private string GetShopId()
        {
            return User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value 
                   ?? throw new UnauthorizedAccessException("Shop not authenticated");
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                var shopId = GetShopId();
                var invoices = await _invoiceService.GetInvoices(shopId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoices");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("summary/stats")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var shopId = GetShopId();
                var summary = await _invoiceService.GetSummary(shopId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching summary");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("analytics/payment-methods")]
        public async Task<IActionResult> GetPaymentAnalytics()
        {
            try
            {
                var shopId = GetShopId();
                var data = await _invoiceService.GetSalesByPayment(shopId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment analytics");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("analytics/daily-sales")]
        public async Task<IActionResult> GetDailySales([FromQuery] int days = 7)
        {
            try
            {
                var shopId = GetShopId();
                var data = await _invoiceService.GetDailySales(shopId, days);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching daily sales");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(long id)  // Changed from Guid to long
        {
            try
            {
                var shopId = GetShopId();
                var invoice = await _invoiceService.GetInvoice(id, shopId);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoice");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            try
            {
                var shopId = GetShopId();
                var invoice = await _invoiceService.CreateInvoice(shopId, dto);
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}