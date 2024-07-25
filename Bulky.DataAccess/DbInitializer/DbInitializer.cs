using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public DbInitializer(
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        //migrations if they are not applied
        try
        {
            if (_dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                _dbContext.Database.Migrate();
            };
        }
        catch (Exception e) { }

        //create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_Cust).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Cust)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Comp)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

            //if roles are not created, then we will create admin user as well
            _userManager.CreateAsync(
                user: new ApplicationUser
                {
                    UserName = "admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    Name = "Khoi",
                    PhoneNumber = "1234567890",
                    StreetAddress = "test 123 Melbourne",
                    State = "Melbourne",
                    PostalCode = "12345",
                    City = "abc"
                },
                password: "Admin123@").GetAwaiter().GetResult();

            var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }       
    }
}
