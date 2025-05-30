﻿using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EShop.Domain.Service.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IDatabase _redisDb;

        public ProductService(IProductRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
            var redis = ConnectionMultiplexer.Connect("redis:6379");
            _redisDb = redis.GetDatabase();
        }

        // ZROBIC ADD NIESYNCHRONICZNY Z .RESULT
        public async Task AddProductAsync(Product product)
        {
            await _repository.AddAsync(product);
        }

        public void Add(Product product)
        {
            _repository.AddAsync(product).GetAwaiter().GetResult(); ;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            //return await _repository.GetByIdAsync(id);

            string key = $"Product:{id}";
            var redisValue = await _redisDb.StringGetAsync(key);
            Product? product = null;

            if (redisValue.HasValue)
                product = JsonSerializer.Deserialize<Product>(redisValue);

            if (product == null)
            {
                product = await _repository.GetByIdAsync(id);
                if (product != null)
                    await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(product), TimeSpan.FromDays(1));
            }

            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _repository.GetByIdAsync(product.Id);
            if (existingProduct == null) throw new Exception("Product not found");

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Ean = product.Ean;
            existingProduct.Sku = product.Sku;
            existingProduct.Stock = product.Stock;
            existingProduct.UpdatedBy = product.UpdatedBy;

            var existingCategory = await _repository.GetCategoryByNameAsync(product.Category.Name);
            existingProduct.Category = existingCategory ?? new Category { Name = product.Category.Name };

            await _repository.SaveChangesAsync();
        }
    }
}
