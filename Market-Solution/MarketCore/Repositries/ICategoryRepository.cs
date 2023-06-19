using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Repositries
{
    public interface ICategoryRepository: IGenericRepository<Category>
    {
        Category GetCategoryByName(string name);
    }
}
