using Market_Repositry.Data;
using MarketCore.Entities;
using MarketCore.Repositries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRepositry
{
    public class PurchasesRepository: GenericRepository<Purchases> ,IPurchasesRepository
    {
        private readonly MarketContext context;

        public PurchasesRepository(MarketContext _context) : base(_context)
        {
            context = _context;
        }
        public async Task<List<Purchases>> GetAllZerosAsync()
          => await context.Purchases.Where(p => p.Quantity == 0).ToListAsync();
        public async Task<List<Purchases>> GetAllWithoutZerosAsync()
            => await context.Purchases.Where(p=>p.Quantity!=0).ToListAsync();

        public async Task<List<Purchases>> GetAllWithFilterAsync(int companies, int branches , string purchases , int month)
        {
            List<Purchases> products =await GetAllWithoutZerosAsync();

            if (companies != 0)
            {
                products = products.Where(p => p.CompanyId== companies).ToList();
            }
            if (branches != 0)
            {
                products = products.Where(p => p.BranchId== branches).ToList();
            }
            if (purchases !="all")
            {
                products = products.Where(p => p.Product == purchases).ToList();
            }
            if (month !=0)
            {
                products = products.Where(p => p.Month == month).ToList();
            }
            return products;
        }




        public double GetQuantityOfProduct(string name , List<Purchases> Purchases)
        {
            double quantity = 0;
            var products = Purchases.Where(p => p.Product == name).Where(p => p.Quantity != 0).ToList();
            foreach (var product in products)
            {
                quantity += product.Quantity;
            }
            return quantity;
        }
        public decimal GetMaxPriceOfProduct(string name, List<Purchases> purchases)
        {
            decimal price = 0;
            var products = purchases.Where(p => p.Product == name).Where(p => p.Quantity != 0).ToList();
            foreach (var product in products)
            {
                if(product.Price> price)
                    price = product.Price;
            }
            return price;
        }
        public double GetValueOfProduct(string name , List<Purchases> purchases)
        {
            double value = 0;
            var products = purchases.Where(p => p.Product == name).Where(p => p.Quantity != 0).ToList();
            foreach (var product in products)
            {
                value += product.Value;
            }
            return value;
        }



    }
}
