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
    public class CategoryRepository:GenericRepository<Category>, ICategoryRepository
    {
        private readonly MarketContext context;
        public CategoryRepository(MarketContext _context) : base(_context)
        {
            context = _context;

        }
        public Category GetCategoryByName(string name)
        {
            var category = context.Categories.Where(C => C.Name == name).FirstOrDefault();
            return category;

        }
    }
}
