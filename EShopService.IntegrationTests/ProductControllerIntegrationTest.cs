using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text.Json;

namespace EShopService.Tests.Controllers;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ProductControllerTests(WebApplicationFactory<Program> factory)
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
    }

    [Fact]
    public async Task Get_ReturnsListOfProducts()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Products.Add(new Product
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
            );
            await dbContext.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/api/product");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<Product>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("Laptop", products[0].Name);
    }
}