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
    public class FoodCostRepo : GenericRepository<FoodCost>, IFoodCostRepo
    {
        private readonly MarketContext context;

        public FoodCostRepo(MarketContext _context) : base(_context)
        {
            context = _context;
        }

     
        public async Task<List<FoodCost>> GetAllWithoutZerosAsync()
            => await context.FoodCost.ToListAsync();

        public async Task<List<FoodCost>> GetAllWithFilterAsync(int branch, int category, int month)
        {
            List<FoodCost> foodCost = await GetAllWithoutZerosAsync();

            if (branch != 0)
            {
                foodCost = foodCost.Where(s => s.BranchId == branch).ToList();
            }
            if (category != 0)
            {
                foodCost = foodCost.Where(s => s.CategoryId == category).ToList();
            }
            if (month != 0)
            {
                foodCost = foodCost.Where(s => s.Month == month).ToList();
            }
          
            return foodCost;
        }

        public decimal GetSumSalesForBranch(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result=  FoodCost.Where(f=>f.BranchId==id).ToList();

            foreach (var item in result)
            {
                sum += item.Sales;

            }
            return sum;

        }

        public decimal GetSumSalesForCategory(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result = FoodCost.Where(f => f.CategoryId == id).ToList();

            foreach (var item in result)
            {
                sum += item.Sales;

            }
            return sum;

        }

        public decimal GetSumOpenInventoryForBranch(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result = FoodCost.Where(f => f.BranchId == id).ToList();

            foreach (var item in result)
            {
                sum += item.OpenInventory;

            }
            return sum;

        }

        public decimal GetSumCloseInventoryForBranch(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result =FoodCost.Where(f => f.BranchId == id).ToList();

            foreach (var item in result)
            {
                sum += item.CloseInventory;

            }
            return sum;

        }

        public decimal GetSumOpenInventoryForCategory(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result = FoodCost.Where(f => f.CategoryId == id).ToList();

            foreach (var item in result)
            {
                sum += item.OpenInventory;

            }
            return sum;

        }

        public decimal GetSumCloseInventoryForCategory(int id, List<FoodCost> FoodCost)
        {
            decimal sum = 0m;
            var result = FoodCost.Where(f => f.CategoryId == id).ToList();

            foreach (var item in result)
            {
                sum += item.CloseInventory;

            }
            return sum;

        }


    }
}
