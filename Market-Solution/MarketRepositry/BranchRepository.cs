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
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        private readonly MarketContext context;

        public BranchRepository(MarketContext _context) : base(_context)
        {
            context = _context;
        }



        public Branch GetBranchByName(string name)
        {
            var branch = context.Branchs.Where(B => B.Name == name).FirstOrDefault();
            return branch;

        }
    }
}
