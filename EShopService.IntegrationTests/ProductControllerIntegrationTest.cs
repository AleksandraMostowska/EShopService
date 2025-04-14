using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace EShopService.Tests.Controllers;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public ProductControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                    .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<DataContext>));
                    if (dbContextOptions != null)
                        services.Remove(dbContextOptions);

                    services
                    .AddDbContext<DataContext>(options => options.UseInMemoryDatabase("MyDBForTest"));
                });
            });

        _client = _factory.CreateClient();

        _output = output;
    }

    [Fact]
    public async Task Get_ReturnsListOfProducts()
    {


        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            var electronics = new Category { Name = "Electronics" };
            dbContext.Categories.Add(electronics);
            await dbContext.SaveChangesAsync();

            dbContext.Products.Add(new Product
                {
                    Name = "Laptop",
                    Price = 4000,
                    Ean = "1234567890123",
                    Sku = "LAPTOP123",
                    Stock = 10,
                    Category = electronics,
                    CreatedBy = Guid.NewGuid(),
                    UpdatedBy = Guid.NewGuid()
                }
            );
            await dbContext.SaveChangesAsync();

            //dbContext.Products.RemoveRange(dbContext.Products);
            //dbContext.Categories.RemoveRange(dbContext.Categories);
        }

        var response = await _client.GetAsync("/api/Product");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<Product>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("Laptop", products[0].Name);
    }


    [Fact]
    public async Task GetById_ReturnsProduct_WhenProductExists()
    {

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            var electronics = new Category { Name = "Electronics" };
            dbContext.Categories.Add(electronics);
            await dbContext.SaveChangesAsync();

            var product = new Product
            {
                Id = 100,
                Name = "Laptop",
                Price = 4000,
                Ean = "1234567890123",
                Sku = "LAPTOP123",
                Stock = 10,
                Category = electronics,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            //dbContext.Products.RemoveRange(dbContext.Products);
            //dbContext.Categories.RemoveRange(dbContext.Categories);
        }

        var response = await _client.GetAsync("/api/Product/100");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        var productResult = JsonSerializer.Deserialize<Product>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(productResult);
        Assert.Equal("Laptop", productResult.Name);
        Assert.Equal(4000, productResult.Price);
    }


    [Fact]
    public async Task Delete_RemovesProduct()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();
            //await dbContext.Database.EnsureDeletedAsync();
            //await dbContext.Database.EnsureCreatedAsync();

            var category = new Category { Name = "SomeRandomCategory" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var product = new Product
            {
                Name = "ToDelete",
                Price = 123,
                Ean = "0000000000000",
                Sku = "DEL123",
                Stock = 1,
                Category = category,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var productId = product.Id;

            var response = await _client.DeleteAsync($"/api/Product/{productId}");
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }



    [Fact]
    public async Task Post_AddsNewProduct()
    {

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            var category = new Category { Name = "Electronics" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var newProduct = new
            {
                Name = "Laptop",
                Price = 4000,
                Ean = "1234567890123",
                Sku = "LAPTOP123",
                Stock = 10,
                Category = new { Name = "Electronics" },
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            var content = new StringContent(JsonSerializer.Serialize(newProduct), System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Product", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(product);
            Assert.Equal("Laptop", product.Name);
        }

    }

    [Fact]
    public async Task SaveProductToInMemoryDatabase_WorksCorrectly()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        await dbContext.SaveChangesAsync();

        //await dbContext.Database.EnsureDeletedAsync();
        //await dbContext.Database.EnsureCreatedAsync();

        var category = new Category { Name = "TestCategory" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "TestProduct",
            Price = 999,
            Ean = "9999999999999",
            Sku = "TEST999",
            Stock = 5,
            Category = category,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var savedProduct = await dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Sku == "TEST999");

        Assert.NotNull(savedProduct);
        Assert.Equal("TestProduct", savedProduct.Name);
        Assert.Equal("TestCategory", savedProduct.Category.Name);
    }

    [Fact]
    public async Task Put_UpdatesProduct_WithExistingCategory()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);

            var category = new Category { Name = "Tech" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 300,
                Ean = "1111111111111",
                Sku = "SKU001",
                Stock = 5,
                Category = category,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

            var updatedProduct = new Product
            {
                Id = 1,
                Name = "Smartphone",
                Price = 350,
                Ean = "1111111111111",
                Sku = "SKU001",
                Stock = 10,
                Category = new Category { Name = "Tech" },
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

        var json = JsonSerializer.Serialize(updatedProduct, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/Product/1", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesProduct_WithNewCategory()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Categories.RemoveRange(dbContext.Categories);

            var category = new Category { Name = "Tech" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 300,
                Ean = "1111111111111",
                Sku = "SKU001",
                Stock = 5,
                Category = category,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        var updatedProduct = new Product
        {
            Id = 1,
            Name = "Jersey",
            Price = 350,
            Ean = "1111111111111",
            Sku = "SKU001",
            Stock = 10,
            Category = new Category { Name = "Clothing" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var json = JsonSerializer.Serialize(updatedProduct, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/Product/1", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/Product/123456789");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task Put_ReturnsBadRequest_WhenIdMismatch()
    {
        var product = new Product
        {
            Id = 2,
            Name = "Mismatch",
            Price = 50,
            Ean = "2222222222222",
            Sku = "SKU002",
            Stock = 0,
            Category = new Category { Id = 1, Name = "Dummy" },
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var json = JsonSerializer.Serialize(product, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/Product/1", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task Post_ReturnsBadRequest_WhenProductIsNull()
    {
        var content = new StringContent("null", System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/Product", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }





    [Fact]
    public async Task Post_AddThousandsProductsAsync_ExceptedThousandsProducts()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        await dbContext.SaveChangesAsync();

        var category = new Category { Name = "MyRandomCategory" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var tasks = new List<Task>();

        for (int i = 0; i < 10000; i++)
        {
            var factory = _factory.Services;
            int productIndex = i;

            tasks.Add(Task.Run(async () =>
            {
                var innerScope = factory.CreateScope();
                var context = innerScope.ServiceProvider.GetRequiredService<DataContext>();

                var cat = await context.Categories.FirstAsync(c => c.Name == "MyRandomCategory");

                context.Products.Add(new Product
                {
                    Name = $"P: {productIndex}",
                    Price = 10 + productIndex,
                    Ean = $"{productIndex:D13}",
                    Sku = $"XXX{productIndex:D5}",
                    Stock = productIndex % 100,
                    Category = cat,
                    CreatedBy = Guid.NewGuid(),
                    UpdatedBy = Guid.NewGuid()
                });

                await context.SaveChangesAsync();
            }));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        _output.WriteLine($"ASYNC TIME: {stopwatch.ElapsedMilliseconds} ms");

        var total = await dbContext.Products.CountAsync();
        Assert.Equal(10000, total);
    }



    [Fact]
    public void Post_AddThousandsProducts_ExceptedThousandsProducts()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.SaveChanges();

        var category = new Category { Name = "MyRandomCategory" };
        dbContext.Categories.Add(category);
        dbContext.SaveChanges();

        for (int i = 0; i < 10000; i++)
        {
            var product = new Product
            {
                Name = $"P: {i}",
                Price = 10 + i,
                Ean = $"{i:D13}",
                Sku = $"XXX{i:D5}",
                Stock = i % 100,
                Category = category,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid()
            };

            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }

        //dbContext.SaveChanges();

        stopwatch.Stop();
        _output.WriteLine($"SYNC TIME: {stopwatch.ElapsedMilliseconds} ms");

        var total = dbContext.Products.Count();
        Assert.Equal(10000, total);
    }

}