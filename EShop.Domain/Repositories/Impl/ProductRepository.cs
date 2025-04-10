using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EShop.Domain.Repositories.Impl
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            //var existingCategory = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
            //if (existingCategory == null)
            //{
            //    existingCategory = new Category
            //    {
            //        Name = product.Category.Name
            //    };
            //    _context.Categories.Add(existingCategory);
            //    _context.SaveChanges();
            //}

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id); 
            if (product != null)
            {
                _context.Products.Remove(product); 
                _context.SaveChanges();            
            }
        }

        public List<Product> GetAll()
        {
            return _context.Products.Include(p => p.Category).ToList();
            //return _context.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            //return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public void Update(Product product)
        {
            _context.Update(product);
            _context.SaveChanges();
        }

        public Category GetCategoryByName(string name)
        {
            return _context.Categories.FirstOrDefault(c => c.Name == name);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
