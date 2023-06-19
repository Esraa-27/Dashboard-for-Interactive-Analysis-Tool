using MarketCore.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarketRepositry.Data
{
    public static class MarketContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
           
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
            }
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    Email = "esraaabdou27@gmail.com",
                    UserName = "Esraa_Abdou"

                };
                await userManager.CreateAsync(user, "Esraaabdou27@");
                await userManager.AddToRoleAsync(user, "Admin");

            }
        }
    }
}
