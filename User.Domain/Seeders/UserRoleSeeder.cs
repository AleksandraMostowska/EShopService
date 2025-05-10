using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserModel = User.Domain.Models.User.User;
using User.Domain.Models.User;
using User.Domain.Repositories;
using User.Domain.Security;

namespace User.Domain.Seeders;

public class UserRoleSeeder : IUserRoleSeeder
{
    private readonly DataContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserRoleSeeder(DataContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Seed()
    {
        if (!_context.Roles.Any())
        {
            var roles = new List<Role>
                {
                    new Role
                    {
                        Name = "Administrator"
                    },
                    new Role
                    {
                        Name = "Client"
                    },
                    new Role
                    {
                        Name = "Employee"
                    }
                };

            _context.Roles.AddRange(roles);

            await _context.SaveChangesAsync();
        }

        if (!_context.Users.Any())
        {
            var adminRole = _context.Roles.First(r => r.Name == "Administrator");
            var employeeRole = _context.Roles.First(r => r.Name == "Employee");
            var clientRole = _context.Roles.First(r => r.Name == "Client");

            var users = new List<UserModel>
                {
                    new UserModel
                    {
                        Username = "administrator",
                        Email = "admin@example.com",
                        PasswordHash = _passwordHasher.Hash("Admin123!"),
                        Roles = new List<Role> { adminRole, employeeRole, clientRole }
                    },
                    new UserModel
                    {
                        Username = "employee1",
                        Email = "employee1@example.com",
                        PasswordHash = _passwordHasher.Hash("Employee123!"),
                        Roles = new List<Role> { employeeRole }
                    },
                    new UserModel
                    {
                        Username = "employee2",
                        Email = "employee2@example.com",
                        PasswordHash = _passwordHasher.Hash("Employee123!"),
                        Roles = new List<Role> { employeeRole }
                    },
                    new UserModel
                    {
                        Username = "employee3",
                        Email = "employee3@example.com",
                        PasswordHash = _passwordHasher.Hash("Employee123!"),
                        Roles = new List<Role> { employeeRole }
                    }
                };

            _context.Users.AddRange(users);

            await _context.SaveChangesAsync();
        }
    }
}
