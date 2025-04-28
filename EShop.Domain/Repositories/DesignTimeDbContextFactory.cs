//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System.IO;
//using EShop.Domain.Repositories;

//namespace EShop.Domain.Repositories;

//public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
//{
//    public DataContext CreateDbContext(string[] args)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

//        //var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

//        //var connectionString = isDocker
//        //    ? "Server=db;Database=MyDatabase;User Id=sa;Password=YourPassword123;"
//        //    : "Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=YourPassword123;";
//        //optionsBuilder.UseSqlServer(connectionString);


//        optionsBuilder.UseSqlServer("Server=db;Database=MyDatabase;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;");
//        //optionsBuilder.UseSqlServer("Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;");

//        return new DataContext(optionsBuilder.Options);
//    }
//}
