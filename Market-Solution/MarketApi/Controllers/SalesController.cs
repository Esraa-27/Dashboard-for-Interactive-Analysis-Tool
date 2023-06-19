using MarketApi.Dtos.Purchases;
using MarketApi.Dtos.Sales;
using MarketApi.Dtos.Shared;
using MarketApi.Errors;
using MarketCore;
using MarketCore.Entities;
using MarketCore.Repositries;
using MarketRepositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarketApi.Controllers
{

    public class SalesController : BaseApiController
    {
        private IUnitOfWork unitOfWork;
        private readonly IUserRepo userRepo;

        public AppUser user { get; set; }
        public SalesController(
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
            if (colCount != 9)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });
            if (rowCount < 1)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileEmpty }, HasError = true });

            //********

            int Itemindex = 0;
            int Quantityindex = 0;
            int SalesValueindex = 0;
            int Vatindex = 0;
            int TotalSalesindex = 0;
            int Averageindex = 0;
            int Monthindex = 0;
            int Categoryindex = 0;
            int Branchindex = 0;

            string value = "";

            for (int i = 1; i <= 9; i++)
            {
                value = worksheet.Cells[1, i].GetCellValue<string>().ToLower();

                if (Regex.Match(value, "^p([a-z])*t$").Success)//item
                    Itemindex = i;
                else if (Regex.Match(value, "^q([a-z])*(\\s)*$").Success)//Quantity
                    Quantityindex = i;
                else if (Regex.Match(value, "^s([a-z])*(\\s)*$").Success)//sales
                    SalesValueindex = i;
                else if (Regex.Match(value, "^v([a-z])*(\\s)*$").Success)//vat
                    Vatindex = i;
                else if (Regex.Match(value, "^a([a-z])*(\\s)*$").Success)//Average
                    Averageindex = i;
                else if (Regex.Match(value, "^t([a-z])*(\\s)*$").Success)//totalSales
                    TotalSalesindex = i;
                else if (Regex.Match(value, "^m([a-z])*(\\s)*$").Success)//month
                    Monthindex = i;
                else if (Regex.Match(value, "^c([a-z])*(\\s)*$").Success)//category
                    Categoryindex = i;
                else if (Regex.Match(value, "^b([a-z])*(\\s)*$").Success)//branch
                    Branchindex = i;
                else
                    return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });

            }

            try
            {
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, Itemindex].GetCellValue<string>() == null) { continue; }
                    var record = new SalesInpDto()
                    {
                        Product = worksheet.Cells[row, Itemindex].GetCellValue<string>().ToLower(),
                        Quantity = worksheet.Cells[row, Quantityindex].GetCellValue<double>(),
                        SalesValue = worksheet.Cells[row, SalesValueindex].GetCellValue<decimal>(),
                        Vat = worksheet.Cells[row, Vatindex].GetCellValue<decimal>(),
                        TotalSales = worksheet.Cells[row,TotalSalesindex].GetCellValue<decimal>(),
                        Average = worksheet.Cells[row, Averageindex].GetCellValue<decimal>(),
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
                        category = new Category() {Name= record.Category };
                        await unitOfWork.CategoryRepo.AddAsync(category);
                        await unitOfWork.Complete();
                        category = unitOfWork.CategoryRepo.GetCategoryByName(record.Category);
                    }
                    var sales = new Sales()
                    {
                        Category = category,
                        CategoryId = category.Id,
                        Branch = branch,
                        BranchId = branch.Id,
                        Product = record.Product,
                        Vat = record.Vat,
                        Month = record.Month,
                        Quantity = record.Quantity,
                        SalesValue = record.SalesValue,
                        TotalSales = record.TotalSales,
                        Average = record.Average
                       
                    };

                    await unitOfWork.SalesRepo.AddAsync(sales);

                }
                await unitOfWork.Complete();
            }
            catch (Exception)
            {
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.DataNotSuccess }, HasError = true });
            }
            return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.DataSuccess }, HasError = false });
        }

        [HttpGet("generate-excel-sheet")]
        public ActionResult GenerateExcel()
        {
            //if (user == null)
            //    return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            using (var package = new ExcelPackage()) //create a new excel package
            {
                // Add a new worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Sales");
                              
                ExcelRange range = worksheet.Cells["A1:I200"];
                ExcelTable table = worksheet.Tables.Add(range, "Table");
                worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed30);
                worksheet.Cells["A1:I1"].Style.Font.Color.SetColor(ExcelIndexedColor.Indexed1);
                worksheet.Cells[range.Address].Style.Font.Size = 14;
                worksheet.Cells[range.Address].Style.Font.Name = "Calibri";
                worksheet.Cells[range.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[range.Address].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[range.Address].AutoFitColumns();

                //Formatting the worksheet
                worksheet.Cells["A1"].Value = "Product";
                worksheet.Cells["B1"].Value = "SalesValue";
                worksheet.Cells["C1"].Value = "Vat";
                worksheet.Cells["D1"].Value = "TotalSales";
                worksheet.Cells["E1"].Value = "Quantity";
                worksheet.Cells["F1"].Value = "Average";
                worksheet.Cells["G1"].Value = "Month";
                worksheet.Cells["H1"].Value = "Category";
                worksheet.Cells["I1"].Value = "Branch";
                worksheet.Cells["A1:I1"].AutoFitColumns();
                worksheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //
                var ranges = worksheet.Cells[2, 4, 200, 4];
                ranges.Formula = string.Format("{0}+{1}", worksheet.Cells[2, 2].Address, worksheet.Cells[2, 3].Address);
                
                ranges = worksheet.Cells[2, 6, 200, 6];
                ranges.Formula = string.Format("{0}/{1}", worksheet.Cells[2, 2].Address, worksheet.Cells[2, 5].Address);

                //Save the excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);

                //Return the generated excel file
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sales.xlsx");

            }
        }


        [HttpPost("all-quantity-product")]
        public async Task<ActionResult<List<QuantityProductDto>>> GetAllQuantityproduct(SalesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Sales> Sales = new List<Sales>();
            Sales = await unitOfWork.SalesRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month, filterDto.Product);

            List<QuantityProductDto> outProducts = new List<QuantityProductDto>();

            foreach (var sale in Sales)
            {
                if (!outProducts.Any(s => s.Product == sale.Product))
                {
                    var item = new QuantityProductDto()
                    {
                        Product = sale.Product,

                        Quantity = unitOfWork.SalesRepo.GetQuantityOfProduct(sale.Product, Sales)

                    };
                    outProducts.Add(item);
                }

            }
            //outProducts= outProducts.OrderByDescending(s => s.Quantity);
            return Ok(outProducts.OrderByDescending(s => s.Quantity));
        }

        [HttpPost("all-sales-product")]
        public async Task<ActionResult<List<SaleItemDto>>> GetAllSaleItem(SalesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Sales> Sales = new List<Sales>();
            Sales = await unitOfWork.SalesRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month, filterDto.Product);

            List<SaleItemDto> outItems = new List<SaleItemDto>();

            foreach (var sale in Sales)
            {
                if (!outItems.Any(s => s.Product == sale.Product))
                {
                    var item = new SaleItemDto()
                    {
                        Product = sale.Product,

                        Sale = unitOfWork.SalesRepo.GetSaleOfProduct(sale.Product, Sales)

                    };
                    outItems.Add(item);
                }

            }

            return Ok(outItems.OrderByDescending(x=>x.Sale));
        }

        [HttpPost("all-average-product")]
        public async Task<ActionResult<List<AverageItemDto>>> GetAllAverageItem(SalesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Sales> Sales = new List<Sales>();
            Sales = await unitOfWork.SalesRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month, filterDto.Product);

            List<AverageItemDto> outItems = new List<AverageItemDto>();

            foreach (var sale in Sales)
            {
                if (!outItems.Any(s => s.Product == sale.Product))
                {
                    var item = new AverageItemDto()
                    {
                        Product = sale.Product,

                        Average = unitOfWork.SalesRepo.GetAverageOfProduct(sale.Product, Sales)

                    };
                    outItems.Add(item);
                }

            }

            return Ok(outItems.OrderByDescending(x=>x.Average));
        }

        [HttpPost("all-average-category")]
        public async Task<ActionResult<List<AverageCategoryDto>>> GetAllAverageCategory(SalesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Sales> Sales = new List<Sales>();
            Sales = await unitOfWork.SalesRepo.GetAllWithFilterAsync(filterDto.Branch, filterDto.Category, filterDto.Month, filterDto.Product);

            List<AverageCategoryDto> outItems = new List<AverageCategoryDto>();

            foreach (var sale in Sales)
            {

                var category = await unitOfWork.CategoryRepo.GetByIdAsync(sale.CategoryId);
                if (!outItems.Any(s => s.Category == category.Name))
                {
                    var item = new AverageCategoryDto()
                    {
                        Category = category.Name,

                        Average = unitOfWork.SalesRepo.GetAverageOfCategory(sale.CategoryId, Sales)

                    };
                    outItems.Add(item);
                }

            }

            return Ok(outItems.OrderByDescending(x => x.Average));
        }


        // Drop down list
 
        [HttpGet("products")]
        public async Task<ActionResult<List<string>>> GetProduct()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var sales = await unitOfWork.SalesRepo.GetAllWithoutZerosAsync();
            List<string> outItems = new List<string>();

            foreach (var sale in sales)
            {
                if (!outItems.Contains(sale.Product))
                {
                    outItems.Add(sale.Product);
                }
            }
            return Ok(outItems);
        }
        [HttpGet("months")]
        public async Task<ActionResult<List<int>>> GetMonth()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var sales = await unitOfWork.SalesRepo.GetAllWithoutZerosAsync();
            List<int> outmonthes = new List<int>();

            foreach (var sale in sales)
            {
                if (!outmonthes.Contains(sale.Month))
                {
                    outmonthes.Add(sale.Month);
                }
            }
            return Ok(outmonthes);
        }

    }
}
