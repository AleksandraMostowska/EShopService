using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserModel = User.Domain.Models.User.User;
using User.Domain.Models.User;


namespace User.Domain.Repositories;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; } 
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserRoles"));
    }
}
