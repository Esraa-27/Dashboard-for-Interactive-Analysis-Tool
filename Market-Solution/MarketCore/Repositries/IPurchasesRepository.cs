using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Repositries
{
    public interface IPurchasesRepository: IGenericRepository<Purchases>
    {
        Task<List<Purchases>> GetAllWithoutZerosAsync();
        Task<List<Purchases>> GetAllWithFilterAsync(int companies, int branches, string purchases, int month);
        Task<List<Purchases>> GetAllZerosAsync();
        double GetQuantityOfProduct(string name , List<Purchases> purchases);
        double GetValueOfProduct(string name , List<Purchases> purchases);
        decimal GetMaxPriceOfProduct(string name, List<Purchases> purchases);
    }
}
