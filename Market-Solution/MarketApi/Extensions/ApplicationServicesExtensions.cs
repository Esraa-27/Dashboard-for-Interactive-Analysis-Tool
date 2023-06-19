using MarketCore.Repositries;
using MarketRepositry;
using Microsoft.Extensions.DependencyInjection;

namespace MarketApi.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepo, UserRepo>();

            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IPurchasesRepository, PurchasesRepository>(); 
            services.AddScoped<ISalesRepository, SalesRepository>();
            services.AddScoped<IFoodCostRepo, FoodCostRepo>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}
