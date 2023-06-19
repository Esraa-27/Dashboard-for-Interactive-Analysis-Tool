using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Repositries
{
    public interface ICompanyRepository: IGenericRepository<Company>
    {
        Company GetCompanyByName(string name);

        double GetValuesOfCompany(int id, List<Purchases> Purchases);
    }
}
