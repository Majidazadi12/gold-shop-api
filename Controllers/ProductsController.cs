using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoldShopAPI.DTOs;
using GoldShopAPI.Services;

namespace GoldShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        private string GetShopId()
        {
            return User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value 
                   ?? throw new UnauthorizedAccessException("Shop not authenticated");
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? category = null)
        {
            try
            {
                var shopId = GetShopId();
                var products = await _productService.GetProducts(shopId, category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var shopId = GetShopId();
                var categories = await _productService.GetCategories(shopId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("summary/inventory")]
        public async Task<IActionResult> GetInventorySummary()
        {
            try
            {
                var shopId = GetShopId();
                var summary = await _productService.GetInventorySummary(shopId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory summary");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(long id)  // Changed from Guid to long
        {
            try
            {
                var shopId = GetShopId();
                var product = await _productService.GetProduct(id, shopId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var shopId = GetShopId();
                var product = await _productService.CreateProduct(shopId, dto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto dto)  // Changed from Guid to long
        {
            try
            {
                var shopId = GetShopId();
                var product = await _productService.UpdateProduct(id, shopId, dto);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(long id, [FromBody] int quantity)  // Changed from Guid to long
        {
            try
            {
                var shopId = GetShopId();
                var product = await _productService.UpdateStock(id, shopId, quantity);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)  // Changed from Guid to long
        {
            try
            {
                var shopId = GetShopId();
                await _productService.DeleteProduct(id, shopId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}