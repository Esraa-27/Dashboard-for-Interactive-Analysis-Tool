using MarketApi.Dtos.Purchases;
using MarketApi.Errors;
using MarketCore.Repositries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketCore.Entities;

namespace MarketApi.Controllers
{

    public class ValuesController : BaseApiController
    {
        private IUnitOfWork unitOfWork;
        private readonly IUserRepo userRepo;
        public AppUser user { get; set; }
        public ValuesController(
             IUnitOfWork _unitOfWork,
            IUserRepo _userRepo,
            IHttpContextAccessor httpContextAccessor
            )
        {
            unitOfWork = _unitOfWork;
            userRepo = _userRepo;
            string token = httpContextAccessor.HttpContext.Request.Headers["token"].ToString();
            user = userRepo.GetUserByToken(token);
        }


        [HttpGet("companies")]
        public async Task<ActionResult<List<EntityDto>>> GetCompanies()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var companies = await unitOfWork.CompanyRepo.GetAllAsync();
            List<EntityDto> outcompanies = new List<EntityDto>();
            foreach (var company in companies)
            {
                outcompanies.Add(new EntityDto()
                {
                    Id = company.Id,
                    Name = company.Name
                });

            }
            return Ok(outcompanies);
        }

        [HttpGet("branches")]
        public async Task<ActionResult<List<EntityDto>>> GetBranches()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var branches = await unitOfWork.BranchRepo.GetAllAsync();
            List<EntityDto> outbranches = new List<EntityDto>();
            foreach (var branch in branches)
            {
                outbranches.Add(new EntityDto()
                {
                    Id = branch.Id,
                    Name = branch.Name
                });

            }
            return Ok(outbranches);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<EntityDto>>> GetCategories()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var categories = await unitOfWork.CategoryRepo.GetAllAsync();
            List<EntityDto> outcategory = new List<EntityDto>();
            foreach (var category in categories)
            {
                outcategory.Add(new EntityDto()
                {
                    Id = category.Id,
                    Name = category.Name
                });

            }
            return Ok(outcategory);
        }
       


    }
}
