using MarketCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarketCore.Repositries
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();

        Task AddAsync(T entity);    

        void Update(T entity);

        void Delete(T entity);

        //Task<T> GetByIdWithSpecAsync(ISpecification<T> spec);
        //Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        //Task<int> GetCountAsync(ISpecification<T> spec);
    }
}
