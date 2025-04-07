namespace EShopService;

using EShop.Domain.Services;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using EShop.Domain.Repositories.Impl;
using EShop.Domain.Service.Impl;
using EShop.Application;
using EShop.Domain.Seeders.Impl;
using EShop.Domain.Seeders;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.


        //builder.Services.AddDbContext<DataContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<CreditCardService, CreditCardService>();
        builder.Services.AddScoped<IEShopSeeder, EShopSeeder>();




        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IEShopSeeder>();
        await seeder.Seed();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    }


