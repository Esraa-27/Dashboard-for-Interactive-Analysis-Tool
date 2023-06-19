using MarketCore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Market_Repositry.Data
{
    public class MarketContext : IdentityDbContext<AppUser>
    {
        public MarketContext(DbContextOptions<MarketContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<Sales> Sales { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FoodCost> FoodCost { get; set; }
        

    }
}
