


using MagicVillaApi.Data;
using MagicVillaApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.DbInitializer
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        //private readonly ApplicationUserDAO _userDAO;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,ApplicationDbContext db /*ApplicationUserDAO userDAO*/)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            //_userDAO = userDAO;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Client")).GetAwaiter().GetResult();

                var result = _userManager.CreateAsync(new ApplicationUser
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                }, "000000&z").GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }

                ApplicationUser admin = _db.Users.FirstOrDefault(u => u.UserName == "admin@gmail.com");
                _userManager.AddToRoleAsync(admin, "Admin").GetAwaiter().GetResult();
            }

            return;
        }
    }
}
