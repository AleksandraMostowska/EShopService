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
    public async Task Get_ReturnsList_WhenProducts()
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
        _mockService.Setup(p => p.GetAllProductsAsync()).ReturnsAsync(products);

        var result = await _controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task Get_ReturnsEmptyList_WhenNoProducts()
    {
        var emptyList = new List<Product>();
        _mockService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(emptyList);

        var result = await _controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedList = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Empty(returnedList);
    }

    [Fact]
    public async Task GetById_ReturnsProduct_WhenExist()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };
        _mockService.Setup(p => p.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.Get(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductNotExists()
    {
        _mockService.Setup(s => s.GetProductByIdAsync(99)).ReturnsAsync((Product)null);

        var result = await _controller.Get(99);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Post_AddsProduct_WhenPassedProductCorrect()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        _mockService.Setup(p => p.AddProductAsync(product)).Returns(Task.CompletedTask);

        var result = await _controller.Post(product);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.Get), createdAtActionResult.ActionName);
        Assert.Equal(product, createdAtActionResult.Value);
        _mockService.Verify(s => s.AddProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenProductIsNull()
    {
        var result = await _controller.Put(1, null);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Product is null or ID mismatch.", badRequest.Value);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenSuccessful()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        _mockService.Setup(s => s.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _mockService.Setup(s => s.UpdateProductAsync(product)).Returns(Task.CompletedTask);

        var result = await _controller.Put(product.Id, product);

        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.UpdateProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound_WhenProductNotExists()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        _mockService.Setup(s => s.GetProductByIdAsync(product.Id)).ReturnsAsync((Product)null);

        var result = await _controller.Put(product.Id, product);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenProductIsNullOrIdMismatch()
    {
        var product = new Product
        {
            Id = 2,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var result = await _controller.Put(1, product);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Product is null or ID mismatch.", badRequest.Value);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenSuccessful()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 4000,
            Ean = "1234567890123",
            Sku = "LAPTOP123",
            Stock = 10,
            Category = new Category { Name = "Electronics" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };
        _mockService.Setup(s => s.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _mockService.Setup(s => s.DeleteProductAsync(product.Id)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(product.Id);
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteProductAsync(product.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductNotExists()
    {
        _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync((Product)null);

        var result = await _controller.Delete(1);
        Assert.IsType<NotFoundResult>(result);
    }
}
