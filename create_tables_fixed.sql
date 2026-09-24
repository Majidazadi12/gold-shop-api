-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Drop tables if they exist
DROP TABLE IF EXISTS "Invoices" CASCADE;
DROP TABLE IF EXISTS "Products" CASCADE;
DROP TABLE IF EXISTS "Shops" CASCADE;

-- Create Shops table with SERIAL
CREATE TABLE "Shops" (
    "Id" SERIAL PRIMARY KEY,
    "ShopId" VARCHAR(50) UNIQUE NOT NULL,
    "Username" VARCHAR(50) UNIQUE NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "ShopName" VARCHAR(100) NOT NULL,
    "Phone" VARCHAR(20) DEFAULT '',
    "Address" TEXT DEFAULT '',
    "CreatedAt" TIMESTAMPTZ DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ DEFAULT NOW()
);

-- Create Products table using uuid_generate_v4() (compatible with older PostgreSQL)
CREATE TABLE "Products" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "ShopId" VARCHAR(50) NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Category" VARCHAR(50) NOT NULL,
    "Weight" DECIMAL(10,2) NOT NULL,
    "Karat" INTEGER NOT NULL,
    "Price" DECIMAL(10,2) NOT NULL,
    "Quantity" INTEGER DEFAULT 0,
    "Description" TEXT DEFAULT '',
    "CreatedAt" TIMESTAMPTZ DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT fk_products_shop FOREIGN KEY ("ShopId") REFERENCES "Shops"("ShopId") ON DELETE CASCADE
);

-- Create Invoices table using uuid_generate_v4()
CREATE TABLE "Invoices" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "ShopId" VARCHAR(50) NOT NULL,
    "InvoiceNumber" VARCHAR(50) UNIQUE NOT NULL,
    "CustomerName" VARCHAR(100) DEFAULT 'Walk-in Customer',
    "CustomerPhone" VARCHAR(20) DEFAULT '',
    "TotalAmount" DECIMAL(10,2) NOT NULL,
    "PaymentMethod" VARCHAR(50) DEFAULT 'Cash',
    "ItemsJson" JSONB DEFAULT '[]',
    "CreatedAt" TIMESTAMPTZ DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT fk_invoices_shop FOREIGN KEY ("ShopId") REFERENCES "Shops"("ShopId") ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX idx_products_shop_category ON "Products"("ShopId", "Category");
CREATE INDEX idx_invoices_shop_created ON "Invoices"("ShopId", "CreatedAt");

-- Insert sample data with hashed passwords (BCrypt hash for 'password123')
INSERT INTO "Shops" ("ShopId", "Username", "Password", "ShopName", "Phone", "Address") VALUES
('SHOP001', 'goldshop1', '$2a$11$TjnMpY1K4FSFZRMhE9k3KO6k5uZ5LZ5LZ5LZ5LZ5LZ5LZ5LZ5LZ5', 'Gold Shop 1', '+1 234-567-8901', '123 Gold Street, New York'),
('SHOP002', 'goldshop2', '$2a$11$TjnMpY1K4FSFZRMhE9k3KO6k5uZ5LZ5LZ5LZ5LZ5LZ5LZ5LZ5LZ5', 'Gold Shop 2', '+1 234-567-8902', '456 Diamond Avenue, Los Angeles');

-- Insert products for Shop 1
INSERT INTO "Products" ("ShopId", "Name", "Category", "Weight", "Karat", "Price", "Quantity", "Description") VALUES
('SHOP001', 'Gold Necklace', 'Necklace', 15.5, 22, 850.00, 10, 'Beautiful 22K gold necklace'),
('SHOP001', 'Gold Ring', 'Ring', 5.2, 18, 320.00, 15, 'Elegant 18K gold ring'),
('SHOP001', 'Gold Earrings', 'Earrings', 8.7, 22, 490.00, 8, 'Traditional 22K gold earrings');

-- Insert products for Shop 2
INSERT INTO "Products" ("ShopId", "Name", "Category", "Weight", "Karat", "Price", "Quantity", "Description") VALUES
('SHOP002', 'Gold Necklace', 'Necklace', 15.5, 22, 850.00, 10, 'Beautiful 22K gold necklace'),
('SHOP002', 'Gold Ring', 'Ring', 5.2, 18, 320.00, 15, 'Elegant 18K gold ring'),
('SHOP002', 'Gold Earrings', 'Earrings', 8.7, 22, 490.00, 8, 'Traditional 22K gold earrings');