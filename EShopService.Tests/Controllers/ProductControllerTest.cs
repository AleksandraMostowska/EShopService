using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text.Json;
using Moq;
using EShop.Domain.Services;
using EShopService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Tests.Controllers;

public class ProductControllerTests
{
    private ProductController _controller;
    private Mock<IProductService> _mockService;

    public ProductControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductController(_mockService.Object);
    }


    [Fact]
    public async Task Get_ReturnsListOfProducts()
    {
        var products = new List<Product> {
            new Product
                    {
                        Name = "Laptop",
                        Price = 4000,
                        Ean = "1234567890123",
                        Sku = "LAPTOP123",
                        Stock = 10,
                        Category = new Category { Name = "Electronics" },
                        CreatedBy = Guid.NewGuid(),
                        UpdatedBy = Guid.NewGuid()
                    }
        };
        _mockService.Setup(p => p.GetAllProducts()).Returns(products);
        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);

    }
}