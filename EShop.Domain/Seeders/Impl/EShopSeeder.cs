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
            if (!context.Categories.Any())
            {
                var category1 = new Category { Name = "Elektronika" };
                var category2 = new Category { Name = "Smartfony" };
                var category3 = new Category { Name = "Tablety" };

                context.Categories.AddRange(category1, category2, category3);
                context.SaveChanges();
            }

            if (!context.Products.Any()) 
            {

                var category1 = context.Categories.FirstOrDefault(c => c.Name == "Elektronika");
                var category2 = context.Categories.FirstOrDefault(c => c.Name == "Smartfony");
                var category3 = context.Categories.FirstOrDefault(c => c.Name == "Tablety");


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

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
