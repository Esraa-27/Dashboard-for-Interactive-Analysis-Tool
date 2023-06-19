using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarketCore.Repositries
{
    public interface IUnitOfWork:IDisposable
    {
        ICompanyRepository CompanyRepo { set; get; }
        IBranchRepository BranchRepo { set; get; }
        IPurchasesRepository PurchasesRepo { set; get; }
      
        ICategoryRepository CategoryRepo { get; set; }
        ISalesRepository SalesRepo { get; set; }
        IFoodCostRepo FoodCostRepo { set; get; }
        Task<int> Complete();
    }
}
