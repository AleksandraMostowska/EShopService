using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories
{
    public interface IProductRepository
    {

        public List<Product> GetAll();

        public  Product? GetById(int id);

        public void Add(Product product);

        public void Update(Product product);

        public void Delete(int id);

        Category GetCategoryByName(string name);

        void SaveChanges();
    }
}
