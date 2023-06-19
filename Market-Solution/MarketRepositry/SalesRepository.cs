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
    public class SalesRepository:GenericRepository<Sales>, ISalesRepository
    {
        private readonly MarketContext context;
        public SalesRepository(MarketContext _context) : base(_context)
        {
            context = _context;
            
        }

        public async Task<List<Sales>> GetAllZerosAsync()
            => await context.Sales.Where(s => s.Quantity == 0).ToListAsync();
        public async Task<List<Sales>> GetAllWithoutZerosAsync()
            => await context.Sales.Where(s => s.Quantity != 0).ToListAsync();
        
        public async Task<List<Sales>> GetAllWithFilterAsync( int branch,int category,  int month ,string product)
        {
            List<Sales> sales = await GetAllWithoutZerosAsync();

            if (branch != 0)
            {
                sales = sales.Where(s => s.BranchId == branch).ToList();
            }
            if (category != 0)
            {
                sales = sales.Where(s => s.CategoryId == category).ToList();
            }
            if (month != 0)
            {
                sales = sales.Where(s=> s.Month == month).ToList();
            }
            if (product != "all")
            {
                sales = sales.Where(s => s.Product == product).ToList();
            }
            return sales;
        }




        public double GetQuantityOfProduct(string name, List<Sales> Sales)
        {
            double quantity = 0;
            var sales = Sales.Where(s =>s.Product == name).Where(s => s.Quantity != 0).ToList();
            foreach (var sale in sales)
            {
                quantity += sale.Quantity;
            }
            return quantity;
        }
        public decimal GetSaleOfProduct(string name, List<Sales> Sales)
        {
            decimal saleValue = 0m;
            var sales = Sales.Where(s => s.Product == name).Where(s => s.Quantity != 0).ToList();
            foreach (var sale in sales)
            {
                saleValue += sale.SalesValue;
            }
            return saleValue;
        }

        public decimal GetAverageOfProduct(string name, List<Sales> Sales)
        {
            decimal average = 0m;
            var sales = Sales.Where(s => s.Product == name).Where(s => s.Quantity != 0).ToList();
            foreach (var sale in sales)
            {
                average += sale.Average;
            }
            return average;
        }

        public decimal GetAverageOfCategory(int id, List<Sales> Sales)
        {
            decimal average = 0m;
            var sales = Sales.Where(s => s.CategoryId == id).Where(s => s.Quantity != 0).ToList();
            foreach (var sale in sales)
            {
                average += sale.Average;
            }
            return average;
        }

    }
}
