using MarketApi.Dtos.Sales;
using MarketApi.Errors;
using MarketCore.Entities;
using MarketCore.Repositries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Security.Policy;
using MarketApi.Dtos.Food_Cost;
using MarketApi.Dtos.Purchases;
using System.Collections.Generic;
using MarketApi.Dtos.Shared;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace MarketApi.Controllers
{

    public class FoodCostController : BaseApiController
    {
        private IUnitOfWork unitOfWork;
        private readonly IUserRepo userRepo;

        public AppUser user { get; set; }
        public FoodCostController(
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


        [HttpPost("upload-excel-file")]
        public async Task<ActionResult> UploadFile([FromForm] FileDto file)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });


            if (file.File == null || file.File.Length == 0)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileEmpty }, HasError = true });


            using var stream = new MemoryStream();
            await file.File.CopyToAsync(stream);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);


            if (!file.File.FileName.EndsWith(".xlsx"))
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });

            var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Assuming data is in the first worksheet
            int rowCount = worksheet.Dimension?.Rows ?? 0;
            int colCount = worksheet.Dimension?.Columns ?? 0;
            if (colCount != 10)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });
            if (rowCount < 1)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileEmpty }, HasError = true });

            //********

            int Consumptionindex = 0;
            int OpenInventoryindex = 0;
            int Salesindex = 0;
            int CloseInventoryindex = 0;
            int PurchasingCashindex = 0;
            int PurchasingVisaindex = 0;
            int costindex = 0;
            int Monthindex = 0;
            int Categoryindex = 0;
            int Branchindex = 0;

            string value = "";

            for (int i = 1; i <= 10; i++)
            {
                value = worksheet.Cells[1, i].GetCellValue<string>().ToLower();

                if (Regex.Match(value, "^c([a-z])*t(\\s)*$").Success)//cost
                    costindex = i;
                else if (Regex.Match(value, "^c([a-z])*y(\\s)*$").Success)//category
                    Categoryindex = i;
                else if (Regex.Match(value, "^c([a-z])*n(\\s)*$").Success)//Consumption
                    Consumptionindex = i;
                else if (Regex.Match(value, "^o([a-z])*(\\s)*([a-z])*(\\s)*$").Success)//OpenInventory
                    OpenInventoryindex = i;
                else if (Regex.Match(value, "^s([a-z])*(\\s)*$").Success)//Sales
                    Salesindex = i;
                else if (Regex.Match(value, "^c([a-z])*(\\s)*([a-z])*(\\s)*$").Success)//Close Inventory
                    CloseInventoryindex = i;
                else if (Regex.Match(value, "^p([a-z])*(\\s)*cash$").Success)//Purchasing Cash
                    PurchasingCashindex = i;
                else if (Regex.Match(value, "^p([a-z])*(\\s)*visa$").Success)//Purchasing Visa
                    PurchasingVisaindex = i;
                else if (Regex.Match(value, "^m([a-z])*(\\s)*$").Success)//month
                    Monthindex = i;
                else if (Regex.Match(value, "^b([a-z])*(\\s)*$").Success)//branch
                    Branchindex = i;
                else
                    return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });

            }

            try
            {
                for (int row = 2; row <= rowCount; row++)
                {

                    if (worksheet.Cells[row, Categoryindex].GetCellValue<string>() == null) { continue; }

                    var record = new FoodCostDto()
                    {
                        Cost = worksheet.Cells[row, costindex].GetCellValue<decimal>(),
                        Sales = worksheet.Cells[row, Salesindex].GetCellValue<decimal>(),
                        CloseInventory = worksheet.Cells[row, CloseInventoryindex].GetCellValue<decimal>(),
                        OpenInventory = worksheet.Cells[row, OpenInventoryindex].GetCellValue<decimal>(),
                        Consumption = worksheet.Cells[row, Consumptionindex].GetCellValue<decimal>(),
                        PurchasingCash = worksheet.Cells[row, PurchasingCashindex].GetCellValue<decimal>(),
                        PurchasingVisa = worksheet.Cells[row, PurchasingVisaindex].GetCellValue<decimal>(),
                        Month = worksheet.Cells[row, Monthindex].GetCellValue<int>(),
                        Category = worksheet.Cells[row, Categoryindex].GetCellValue<string>().ToLower(),
                        Branch = worksheet.Cells[row, Branchindex].GetCellValue<string>().ToLower()
                    };
                    var branch = unitOfWork.BranchRepo.GetBranchByName(record.Branch);
                    if (branch is null)
                    {
                        branch = new Branch() { Name = record.Branch };
                        await unitOfWork.BranchRepo.AddAsync(branch);
                        await unitOfWork.Complete();
                        branch = unitOfWork.BranchRepo.GetBranchByName(record.Branch);
                    }
                    var category = unitOfWork.CategoryRepo.GetCategoryByName(record.Category);
                    if (category is null)
                    {
                        category = new Category() { Name = record.Category };
                        await unitOfWork.CategoryRepo.AddAsync(category);
                        await unitOfWork.Complete();
                        category = unitOfWork.CategoryRepo.GetCategoryByName(record.Category);
                    }
                    var foodCost = new FoodCost()
                    {
                        CloseInventory = record.CloseInventory,
                        Month = record.Month,
                        Consumption = record.Consumption,
                        PurchasingVisa = record.PurchasingVisa,
                        PurchasingCash = record.PurchasingCash,
                        Cost = (record.Cost * 100),
                        OpenInventory = record.OpenInventory,
                        Sales = record.Sales,
                        Branch = branch,
                        BranchId = branch.Id,
                        Category = category,
                        CategoryId = category.Id
                    };

                    await unitOfWork.FoodCostRepo.AddAsync(foodCost);

                }
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.DataNotSuccess }, HasError = true });
            }
            return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.DataSuccess }, HasError = false });
        }

        [HttpGet("generate-excel-sheet")]
        public ActionResult GenerateExcel()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            using (var package = new ExcelPackage()) //create a new excel package
            {
                // Add a new worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("FoodCost");

                ExcelRange range = worksheet.Cells["A1:J200"];
                ExcelTable table = worksheet.Tables.Add(range, "Table");
                worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed30);
                worksheet.Cells["A1:J1"].Style.Font.Color.SetColor(ExcelIndexedColor.Indexed1);
                worksheet.Cells[range.Address].Style.Font.Size = 14;
                worksheet.Cells[range.Address].Style.Font.Name = "Calibri";
                worksheet.Cells[range.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[range.Address].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[range.Address].AutoFitColumns();

 
                //Formatting the worksheet
                worksheet.Cells["A1"].Value = "Category";
                worksheet.Cells["B1"].Value = "Branch";
                worksheet.Cells["C1"].Value = "Close Inventory";
                worksheet.Cells["D1"].Value = "Open Inventory";
                worksheet.Cells["E1"].Value ="Purchasing Cash";
                worksheet.Cells["F1"].Value = "Purchasing Visa";
                worksheet.Cells["G1"].Value = "Cost";
                worksheet.Cells["H1"].Value = "Consumption";
                worksheet.Cells["I1"].Value = "Month";
                worksheet.Cells["J1"].Value = "Sales";
                worksheet.Cells["A1:J1"].AutoFitColumns();
                worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

               

                //Save the excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);

                //Return the generated excel file
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FoodCost.xlsx");

            }
        }

        [HttpPost("sum-sales-branch")]
        public async Task<ActionResult<List<SalesBranchDto>>> GetSumSalesOfBranch(FoodCostFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<SalesBranchDto> outSalesBranches = new List<SalesBranchDto>();

            var foodCost = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month);


            foreach (var item in foodCost)
            {
                var currBranch = await unitOfWork.BranchRepo.GetByIdAsync(item.BranchId);
                if (!outSalesBranches.Any(f => f.Branch == currBranch.Name))
                {
                    var entity = new SalesBranchDto()
                    {
                        Branch = currBranch.Name,

                        Sales = unitOfWork.FoodCostRepo.GetSumSalesForBranch(currBranch.Id, foodCost)

                    };
                    outSalesBranches.Add(entity);
                }

            }
            return Ok(outSalesBranches.OrderByDescending(x => x.Sales));
        }


        [HttpPost("sum-sales-category")]
        public async Task<ActionResult<List<SalescategoryDto>>> GetSumSalesOfcategory(FoodCostFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<SalescategoryDto> outSalesCategory = new List<SalescategoryDto>();

            var foodCost = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month);

            foreach (var item in foodCost)
            {
                var currCategory = await unitOfWork.CategoryRepo.GetByIdAsync(item.CategoryId);
                if (!outSalesCategory.Any(f => f.Category == currCategory.Name))
                {
                    var entity = new SalescategoryDto()
                    {
                        Category = currCategory.Name,

                        Sales = unitOfWork.FoodCostRepo.GetSumSalesForCategory(currCategory.Id, foodCost)

                    };
                    outSalesCategory.Add(entity);
                }

            }
            return Ok(outSalesCategory.OrderByDescending(x => x.Sales));
        }


        [HttpPost("sum-inventory-branch")]
        public async Task<ActionResult<List<InventoryBranchDto>>> GetInventoryOfBranch(FoodCostFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<InventoryBranchDto> outInventoryBranches = new List<InventoryBranchDto>();

            var foodCost = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month);


            foreach (var item in foodCost)
            {
                var currBranch = await unitOfWork.BranchRepo.GetByIdAsync(item.BranchId);
                if (!outInventoryBranches.Any(f => f.Branch == currBranch.Name))
                {
                    var entity = new InventoryBranchDto()
                    {
                        Branch = currBranch.Name,

                        OpenInventory = unitOfWork.FoodCostRepo.GetSumOpenInventoryForBranch(currBranch.Id, foodCost),
                        CloseInventory = unitOfWork.FoodCostRepo.GetSumCloseInventoryForBranch(currBranch.Id, foodCost)

                    };
                    outInventoryBranches.Add(entity);
                }

            }
            return Ok(outInventoryBranches.OrderByDescending(x => x.OpenInventory));
        }


        [HttpPost("sum-inventory-category")]
        public async Task<ActionResult<List<InventoryCategoryDto>>> GetInventoryOfcategory(FoodCostFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<InventoryCategoryDto> outInventoryCategory = new List<InventoryCategoryDto>();

            var foodCost = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month);


            foreach (var item in foodCost)
            {
                var currCategory = await unitOfWork.CategoryRepo.GetByIdAsync(item.CategoryId);
                if (!outInventoryCategory.Any(f => f.Category == currCategory.Name))
                {
                    var entity = new InventoryCategoryDto()
                    {
                        Category = currCategory.Name,

                        OpenInventory = unitOfWork.FoodCostRepo.GetSumOpenInventoryForCategory(currCategory.Id, foodCost),
                        CloseInventory = unitOfWork.FoodCostRepo.GetSumCloseInventoryForCategory(currCategory.Id, foodCost)

                    };
                    outInventoryCategory.Add(entity);
                }

            }
            return Ok(outInventoryCategory.OrderByDescending(x => x.OpenInventory));
        }

        [HttpGet("monthes")]
        public async Task<ActionResult<List<int>>> GetMonth()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var foodCosts = await unitOfWork.FoodCostRepo.GetAllWithoutZerosAsync();
            List<int> outmonthes = new List<int>();

            foreach (var foodCost in foodCosts)
            {
                if (!outmonthes.Contains(foodCost.Month))
                {
                    outmonthes.Add(foodCost.Month);
                }
            }
            return Ok(outmonthes.Distinct());
        }

        //Retrieve the sum of consumption of each branch
        [HttpPost("sum-consumption-branch")]
        public async Task<ActionResult<List<ConsumptionBranchDto>>> GetTotalConsumptionBranch(FoodCostFilterDto filterdto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<ConsumptionBranchDto> outConsumptionBranch = new List<ConsumptionBranchDto>();
            var foodcosts = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterdto.Branch, filterdto.Category, filterdto.Month);

            var fc = foodcosts.GroupBy(x => x.BranchId);

            for (int i = 0; i < fc.Count(); i++)
            {
                int id = (int)fc.ElementAt(i).Key;
                outConsumptionBranch.Add(new ConsumptionBranchDto()
                {
                    Branch = unitOfWork.BranchRepo.GetByIdAsync(id).Result.Name,
                    Consumption = fc.ElementAt(i).Sum(x => x.Consumption)
                });
            }

            return Ok(outConsumptionBranch.OrderByDescending(x => x.Consumption));
        }

        //Retrieve the sum of cash purchases and Visa purchases for each branch
        [HttpPost("sum-purchases-branch")]
        public async Task<ActionResult<List<PurchasesBranchDto>>> GetTotalPurchasesBranch(FoodCostFilterDto filterdto) 
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });
            
            List<PurchasesBranchDto> outPurchasesBranch = new List<PurchasesBranchDto>();
            var foodcosts = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterdto.Branch, filterdto.Category, filterdto.Month);

            var fc = foodcosts.GroupBy(x => x.BranchId);

            for (int i = 0; i < fc.Count(); i++) 
            {
                int id = (int)fc.ElementAt(i).Key;
                outPurchasesBranch.Add(new PurchasesBranchDto()
                {
                    Branch = unitOfWork.BranchRepo.GetByIdAsync(id).Result.Name,
                    VisaPurchases = fc.ElementAt(i).Sum(x => x.PurchasingVisa),
                    CashPurchases = fc.ElementAt(i).Sum(x => x.PurchasingCash)
                });
            }

            return Ok(outPurchasesBranch.OrderByDescending(x => x.VisaPurchases));
        }

        //Retrieve the total value of Visa purchases
        [HttpPost("total-value-visa")]
        public async Task<ActionResult<List<VisaPurchasesDto>>> GetTotalVisaPurchases(FoodCostFilterDto filterdto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var foodcosts = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterdto.Branch, filterdto.Category, filterdto.Month);

            VisaPurchasesDto value = new VisaPurchasesDto() { TotalVisaPurchases = foodcosts.Sum(x => x.PurchasingVisa) };


            return Ok(value);
        }

        //Retrieve the total value of Cash purchases
        [HttpPost("total-value-cash")]
        public async Task<ActionResult<List<CashPurchasesDto>>> GetTotalCashPurchases(FoodCostFilterDto filterdto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var foodcosts = await unitOfWork.FoodCostRepo.GetAllWithFilterAsync(filterdto.Branch, filterdto.Category, filterdto.Month);

            CashPurchasesDto value = new CashPurchasesDto() { TotalCashPurchases = foodcosts.Sum(x => x.PurchasingCash) };


            return Ok(value);
        }
    }
}
