using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Service.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public void AddProduct(Product product)
        {

            _repository.Add(product);
        }

        public void DeleteProduct(int id)
        {
            _repository.Delete(id);
        }

        public List<Product> GetAllProducts()
        {
            return _repository.GetAll();
        }

        public Product? GetProductById(int id)
        {
            return _repository.GetById(id);
        }

        public void UpdateProduct(Product product)
        {
            //_repository.Update(product);

            var existingProduct = _repository.GetById(product.Id);
            if (existingProduct == null) throw new Exception("Product not found");

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Ean = product.Ean;
            existingProduct.Sku = product.Sku;
            existingProduct.Stock = product.Stock;
            existingProduct.UpdatedBy = product.UpdatedBy;

            var existingCategory = _repository.GetCategoryByName(product.Category.Name);
            existingProduct.Category = existingCategory ?? new Category { Name = product.Category.Name };

            _repository.SaveChanges();
        }
    }
}
