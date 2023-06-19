using Market_Repositry.Data;
using MarketCore.Entities;
using MarketCore.Repositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRepositry
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly MarketContext context;

        public CompanyRepository(MarketContext _context) : base(_context)
        {
            context = _context;
        }

        public Company GetCompanyByName(string name)
        {
            var company = context.Companies.Where(C => C.Name == name).FirstOrDefault();
            return company;

        }

        public double GetValuesOfCompany(int id, List<Purchases> Purchases)
        {
            double value = 0;
            var products = Purchases.Where(p => p.CompanyId == id).Where(p => p.Value != 0).ToList();
            foreach (var product in products)
            {
                value += product.Value;
            }
            return value;
        }


    }
}
