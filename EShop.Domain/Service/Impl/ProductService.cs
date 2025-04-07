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
            _repository.Update(product);
        }
    }
}
