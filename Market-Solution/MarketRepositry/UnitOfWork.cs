using MarketCore.Repositries;
using Market_Repositry.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketCore.Entities;

namespace MarketRepositry
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly MarketContext context;
        // public  IGenericRepository<Company> CompanyRepo { set; get; }
        public  ICompanyRepository CompanyRepo { set; get; }
        public  IBranchRepository BranchRepo { set; get; }
        public  IPurchasesRepository PurchasesRepo { set; get; }
    
        public ICategoryRepository CategoryRepo { get; set; }
        public ISalesRepository SalesRepo { get; set; }
        public IFoodCostRepo FoodCostRepo { get; set; }

        public UnitOfWork
            (
            MarketContext _context,
            IPurchasesRepository _purchasesRepo,
            ICompanyRepository _companyRepo,
            IBranchRepository _branchRepo,
           
            ICategoryRepository _categoryRepo,
            ISalesRepository _salesRepo,
            IFoodCostRepo foodCostRepo

            )
        {
            context = _context;
            PurchasesRepo = _purchasesRepo;
            CompanyRepo = _companyRepo;
            BranchRepo = _branchRepo;
            
            SalesRepo = _salesRepo;
            CategoryRepo = _categoryRepo;
            FoodCostRepo = foodCostRepo;
        }
        public async Task<int> Complete()
            => await context.SaveChangesAsync();
        

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
