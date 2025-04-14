using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Seeders.Impl
{
    public class EShopSeeder(DataContext context) : IEShopSeeder
    {
        public async Task Seed()
        {
            if (!await context.Categories.AnyAsync())
            {
                var category1 = new Category { Id = 1, Name = "Elektronika" };
                var category2 = new Category { Id = 2, Name = "Smartfony" };
                var category3 = new Category { Id = 3, Name = "Tablety" };

                await context.Categories.AddRangeAsync(category1, category2, category3);
                await context.SaveChangesAsync();
            }

            if (!await context.Products.AnyAsync())
            {
                var category1 = await context.Categories.FirstOrDefaultAsync(c => c.Id == 1);
                var category2 = await context.Categories.FirstOrDefaultAsync(c => c.Id == 2);
                var category3 = await context.Categories.FirstOrDefaultAsync(c => c.Id == 3);

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Telewizor",
                        Price = 3500m,
                        Ean = "1234567890123",
                        Sku = "LAP12345",
                        Stock = 50,
                        Category = category1
                    },
                    new Product
                    {
                        Name = "Smartphone",
                        Price = 2500m,
                        Ean = "9876543210987",
                        Sku = "SM12345",
                        Stock = 100,
                        Category = category2
                    },
                    new Product
                    {
                        Name = "Tablet",
                        Price = 1800m,
                        Ean = "4567890123456",
                        Sku = "TAB12345",
                        Stock = 30,
                        Category = category3
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
