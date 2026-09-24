using Microsoft.EntityFrameworkCore;
using GoldShopAPI.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GoldShopAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Shop configuration
            modelBuilder.Entity<Shop>(entity =>
            {
                entity.HasKey(s => s.Id);
                
                entity.Property(s => s.Id)
                    .UseIdentityColumn()  // For SERIAL
                    .ValueGeneratedOnAdd();

                entity.HasIndex(s => s.ShopId).IsUnique();
                entity.HasIndex(s => s.Username).IsUnique();

                entity.Property(s => s.ShopId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(s => s.ShopName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.Phone)
                    .HasMaxLength(20)
                    .HasDefaultValue("");

                entity.Property(s => s.Address)
                    .HasDefaultValue("");

                entity.Property(s => s.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(s => s.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                
                entity.Property(p => p.Id)
                    .UseIdentityColumn()  // For BIGSERIAL
                    .ValueGeneratedOnAdd();

                entity.HasIndex(p => new { p.ShopId, p.Category });

                entity.Property(p => p.ShopId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.Weight)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(p => p.Karat)
                    .IsRequired();

                entity.Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(p => p.Quantity)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(p => p.Description)
                    .HasDefaultValue("");

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(p => p.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(p => p.Shop)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.ShopId)
                    .HasPrincipalKey(s => s.ShopId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Invoice configuration
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);
                
                entity.Property(i => i.Id)
                    .UseIdentityColumn()  // For BIGSERIAL
                    .ValueGeneratedOnAdd();

                entity.HasIndex(i => i.InvoiceNumber).IsUnique();
                entity.HasIndex(i => new { i.ShopId, i.CreatedAt });

                entity.Property(i => i.ShopId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("Walk-in Customer");

                entity.Property(i => i.CustomerPhone)
                    .HasMaxLength(20)
                    .HasDefaultValue("");

                entity.Property(i => i.TotalAmount)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(i => i.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Cash");

                entity.Property(i => i.ItemsJson)
                    .IsRequired()
                    .HasColumnType("jsonb")
                    .HasDefaultValue("[]");

                entity.Property(i => i.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(i => i.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(i => i.Shop)
                    .WithMany(s => s.Invoices)
                    .HasForeignKey(i => i.ShopId)
                    .HasPrincipalKey(s => s.ShopId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}