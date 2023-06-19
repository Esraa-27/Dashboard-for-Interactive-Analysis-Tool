using MarketCore.Entities;
using MarketCore.Repositries;
using Market_Repositry.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRepositry
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MarketContext context;

        public GenericRepository(MarketContext _context)
        {
            context = _context;
        }
        public async Task<List<T>> GetAllAsync()
            => await context.Set<T>().ToListAsync();
        

        public async Task<T> GetByIdAsync(int id)
            => await context.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity)
        => await context.Set<T>().AddAsync(entity);

        public void Update(T entity)
        => context.Set<T>().Update(entity);

        public void Delete(T entity)
        => context.Set<T>().Remove(entity);

        //public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        //    => await ApplySpecification(spec).ToListAsync();


        //public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
        //   => await ApplySpecification(spec).FirstOrDefaultAsync();


        //public async Task<int> GetCountAsync(ISpecification<T> spec)
        //    => await ApplySpecification(spec).CountAsync();

        //private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        //{
        //    return SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), spec);
        //}


    }
}
