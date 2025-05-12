namespace EShopService;

using EShop.Domain.Services;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using EShop.Domain.Repositories.Impl;
using EShop.Domain.Service.Impl;
using EShop.Domain.Seeders.Impl;
using EShop.Domain.Seeders;
using EShop.Application.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddDbContext<DataContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Transient);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText("./data/public.key"));
                var publicKey = new RsaSecurityKey(rsa);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "EShopNetCourse",
                    ValidAudience = "Eshop",
                    IssuerSigningKey = publicKey
                };
            });

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<CreditCardService, CreditCardService>();
        builder.Services.AddScoped<IEShopSeeder, EShopSeeder>();




        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Wpisz token w formacie: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
              {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Administrator"));
            options.AddPolicy("EmployeeOnly", policy =>
                policy.RequireRole("Employee"));
        });


        builder.Services.AddMemoryCache();

        var app = builder.Build();



        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            await db.Database.MigrateAsync();
            var seeder = scope.ServiceProvider.GetRequiredService<IEShopSeeder>();
            await seeder.Seed();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseCors("AllowAll");


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    }


