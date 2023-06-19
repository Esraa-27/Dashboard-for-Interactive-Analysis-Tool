using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Repositries
{
    public interface ISalesRepository:IGenericRepository<Sales>
    {
        Task<List<Sales>> GetAllWithFilterAsync(int branches, int category, int month, string product);
        double GetQuantityOfProduct(string name, List<Sales> Sales);
        decimal GetSaleOfProduct(string name, List<Sales> Sales);
        decimal GetAverageOfProduct(string name, List<Sales> Sales);
        decimal GetAverageOfCategory(int id, List<Sales> Sales);
        Task<List<Sales>> GetAllWithoutZerosAsync();
        Task<List<Sales>> GetAllZerosAsync();
    }
}
