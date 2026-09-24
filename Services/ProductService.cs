using Microsoft.EntityFrameworkCore;
using GoldShopAPI.Data;
using GoldShopAPI.DTOs;
using GoldShopAPI.Models;

namespace GoldShopAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts(string shopId, string? category = null);
        Task<Product> GetProduct(long id, string shopId);
        Task<Product> CreateProduct(string shopId, CreateProductDto dto);
        Task<Product> UpdateProduct(long id, string shopId, UpdateProductDto dto);
        Task<Product> UpdateStock(long id, string shopId, int quantity);
        Task DeleteProduct(long id, string shopId);
        Task<object> GetCategories(string shopId);
        Task<object> GetInventorySummary(string shopId);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetProducts(string shopId, string? category = null)
        {
            try
            {
                var query = _context.Products.Where(p => p.ShopId == shopId);
                
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    query = query.Where(p => p.Category == category);
                }

                return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for shop {ShopId}", shopId);
                throw;
            }
        }

        public async Task<Product> GetProduct(long id, string shopId)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.ShopId == shopId);

                if (product == null)
                {
                    throw new Exception($"Product with ID {id} not found for shop {shopId}");
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId} for shop {ShopId}", id, shopId);
                throw;
            }
        }

        public async Task<Product> CreateProduct(string shopId, CreateProductDto dto)
        {
            try
            {
                _logger.LogInformation("Creating product for shop {ShopId}: {@ProductDto}", shopId, dto);

                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new Exception("Product name is required");
                
                if (string.IsNullOrWhiteSpace(dto.Category))
                    throw new Exception("Product category is required");
                
                if (dto.Weight <= 0)
                    throw new Exception("Weight must be greater than 0");
                
                if (dto.Karat <= 0)
                    throw new Exception("Karat must be greater than 0");
                
                if (dto.Price <= 0)
                    throw new Exception("Price must be greater than 0");

                // Check if shop exists
                var shopExists = await _context.Shops.AnyAsync(s => s.ShopId == shopId);
                if (!shopExists)
                {
                    throw new Exception($"Shop with ID {shopId} does not exist");
                }

                var product = new Product
                {
                    ShopId = shopId,
                    Name = dto.Name.Trim(),
                    Category = dto.Category,
                    Weight = dto.Weight,
                    Karat = dto.Karat,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    Description = dto.Description ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
                return product;
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Database error creating product: {Error}", innerException);
                
                if (innerException.Contains("23505")) // Unique violation
                {
                    throw new Exception("A product with this name already exists in this shop.");
                }
                else if (innerException.Contains("23503")) // Foreign key violation
                {
                    throw new Exception("The shop ID does not exist. Please make sure you are logged in.");
                }
                else
                {
                    throw new Exception($"Database error: {innerException}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product for shop {ShopId}", shopId);
                throw;
            }
        }

        public async Task<Product> UpdateProduct(long id, string shopId, UpdateProductDto dto)
        {
            try
            {
                var product = await GetProduct(id, shopId);

                if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name.Trim();
                if (!string.IsNullOrEmpty(dto.Category)) product.Category = dto.Category;
                if (dto.Weight.HasValue) product.Weight = dto.Weight.Value;
                if (dto.Karat.HasValue) product.Karat = dto.Karat.Value;
                if (dto.Price.HasValue) product.Price = dto.Price.Value;
                if (dto.Quantity.HasValue) product.Quantity = dto.Quantity.Value;
                if (dto.Description != null) product.Description = dto.Description;

                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId} for shop {ShopId}", id, shopId);
                throw;
            }
        }

        public async Task<Product> UpdateStock(long id, string shopId, int quantity)
        {
            try
            {
                if (quantity < 0)
                {
                    throw new Exception("Quantity cannot be negative");
                }

                var product = await GetProduct(id, shopId);
                product.Quantity = quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId} in shop {ShopId}", id, shopId);
                throw;
            }
        }

        public async Task DeleteProduct(long id, string shopId)
        {
            try
            {
                var product = await GetProduct(id, shopId);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product {ProductId} deleted from shop {ShopId}", id, shopId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId} from shop {ShopId}", id, shopId);
                throw;
            }
        }

        public async Task<object> GetCategories(string shopId)
        {
            try
            {
                var categories = await _context.Products
                    .Where(p => p.ShopId == shopId)
                    .GroupBy(p => p.Category)
                    .Select(g => new
                    {
                        category = g.Key,
                        product_count = g.Count()
                    })
                    .OrderBy(c => c.category)
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories for shop {ShopId}", shopId);
                throw;
            }
        }

        public async Task<object> GetInventorySummary(string shopId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.ShopId == shopId)
                    .ToListAsync();

                var totalProducts = products.Count;
                var totalStock = products.Sum(p => p.Quantity);
                var totalValue = products.Sum(p => p.Price * p.Quantity);
                var lowStockCount = products.Count(p => p.Quantity <= 5);
                var outOfStockCount = products.Count(p => p.Quantity == 0);

                return new
                {
                    total_products = totalProducts,
                    total_stock = totalStock,
                    total_value = totalValue,
                    low_stock_count = lowStockCount,
                    out_of_stock_count = outOfStockCount,
                    low_stock_products = products.Where(p => p.Quantity <= 5).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory summary for shop {ShopId}", shopId);
                throw;
            }
        }
    }
}