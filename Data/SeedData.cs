using Microsoft.EntityFrameworkCore;
using GoldShopAPI.Models;

namespace GoldShopAPI.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Check if data already exists
            if (await context.Shops.AnyAsync())
            {
                return;
            }

            // Hash passwords
            var hashedPassword1 = BCrypt.Net.BCrypt.HashPassword("password123");
            var hashedPassword2 = BCrypt.Net.BCrypt.HashPassword("password123");

            // Create shops
            var shop1 = new Shop
            {
                ShopId = "SHOP001",
                Username = "goldshop1",
                Password = hashedPassword1,
                ShopName = "Gold Shop 1",
                Phone = "+1 234-567-8901",
                Address = "123 Gold Street, New York",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var shop2 = new Shop
            {
                ShopId = "SHOP002",
                Username = "goldshop2",
                Password = hashedPassword2,
                ShopName = "Gold Shop 2",
                Phone = "+1 234-567-8902",
                Address = "456 Diamond Avenue, Los Angeles",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Shops.AddRange(shop1, shop2);
            await context.SaveChangesAsync();

            // Create products for Shop 1
            var products1 = new[]
            {
                new Product
                {
                    ShopId = shop1.ShopId,
                    Name = "Gold Necklace",
                    Category = "Necklace",
                    Weight = 15.5m,
                    Karat = 22,
                    Price = 850.00m,
                    Quantity = 10,
                    Description = "Beautiful 22K gold necklace with intricate design"
                },
                new Product
                {
                    ShopId = shop1.ShopId,
                    Name = "Gold Ring",
                    Category = "Ring",
                    Weight = 5.2m,
                    Karat = 18,
                    Price = 320.00m,
                    Quantity = 15,
                    Description = "Elegant 18K gold ring with diamond accent"
                },
                new Product
                {
                    ShopId = shop1.ShopId,
                    Name = "Gold Earrings",
                    Category = "Earrings",
                    Weight = 8.7m,
                    Karat = 22,
                    Price = 490.00m,
                    Quantity = 8,
                    Description = "Traditional 22K gold earrings with floral pattern"
                }
            };

            // Create products for Shop 2
            var products2 = new[]
            {
                new Product
                {
                    ShopId = shop2.ShopId,
                    Name = "Gold Necklace",
                    Category = "Necklace",
                    Weight = 15.5m,
                    Karat = 22,
                    Price = 850.00m,
                    Quantity = 10,
                    Description = "Beautiful 22K gold necklace with intricate design"
                },
                new Product
                {
                    ShopId = shop2.ShopId,
                    Name = "Gold Ring",
                    Category = "Ring",
                    Weight = 5.2m,
                    Karat = 18,
                    Price = 320.00m,
                    Quantity = 15,
                    Description = "Elegant 18K gold ring with diamond accent"
                },
                new Product
                {
                    ShopId = shop2.ShopId,
                    Name = "Gold Earrings",
                    Category = "Earrings",
                    Weight = 8.7m,
                    Karat = 22,
                    Price = 490.00m,
                    Quantity = 8,
                    Description = "Traditional 22K gold earrings with floral pattern"
                }
            };

            context.Products.AddRange(products1);
            context.Products.AddRange(products2);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Database seeded successfully!");
            Console.WriteLine("\n📋 Login Credentials:");
            Console.WriteLine("┌─────────────┬──────────────┬────────────────┐");
            Console.WriteLine("│ Shop        │ Username     │ Password       │");
            Console.WriteLine("├─────────────┼──────────────┼────────────────┤");
            Console.WriteLine("│ Gold Shop 1 │ goldshop1    │ password123    │");
            Console.WriteLine("│ Gold Shop 2 │ goldshop2    │ password123    │");
            Console.WriteLine("└─────────────┴──────────────┴────────────────┘");
        }
    }
}