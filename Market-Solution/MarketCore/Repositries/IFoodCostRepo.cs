using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Repositries
{
    public interface IFoodCostRepo:IGenericRepository<FoodCost>
    {
        Task<List<FoodCost>> GetAllWithoutZerosAsync();
        Task<List<FoodCost>> GetAllWithFilterAsync(int branch, int category, int month);
        decimal GetSumSalesForBranch(int id, List<FoodCost> FoodCost);
        decimal GetSumSalesForCategory(int id, List<FoodCost> FoodCost);
        decimal GetSumCloseInventoryForBranch(int id, List<FoodCost> FoodCost);
        decimal GetSumOpenInventoryForBranch(int id, List<FoodCost> FoodCost);
        decimal GetSumCloseInventoryForCategory(int id, List<FoodCost> FoodCost);
        decimal GetSumOpenInventoryForCategory(int id, List<FoodCost> FoodCost);
    }
}
