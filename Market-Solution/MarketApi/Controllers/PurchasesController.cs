using Market_Repositry.Data;
using MarketApi.Dtos.Purchases;
using MarketApi.Dtos.Shared;
using MarketApi.Errors;
using MarketCore.Entities;
using MarketCore.Repositries;
using MarketRepositry;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MarketApi.Controllers
{

    public class PurchasesController : BaseApiController
    { 
        private  IUnitOfWork unitOfWork;
        
        private readonly IUserRepo userRepo;

        public AppUser user { get; set; }
        public PurchasesController(
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
            

            if ( !file.File.FileName.EndsWith(".xlsx"))
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });

            var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Assuming data is in the first worksheet
            int rowCount = worksheet.Dimension?.Rows ?? 0;
            int colCount = worksheet.Dimension?.Columns ?? 0;
            if (colCount != 7)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });
            if (rowCount < 1)
                return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileEmpty }, HasError = true });

            //********
            int Productindex = 0;
            int Companyindex = 0;
            int Priceindex = 0;
            int Quantityindex = 0;
            int Valueindex = 0;
            int Monthindex = 0;
            int Branchindex = 0;

            string value = "";

            for (int i = 1; i <= 7; i++)
            {
                value = worksheet.Cells[1, i].GetCellValue<string>().ToLower();

                if (Regex.Match(value, "^p([a-z])*t$").Success)//Product
                    Productindex = i;
                else if (Regex.Match(value, "^q([a-z])*(\\s)*$").Success)//Quantity
                    Quantityindex = i;
                else if (Regex.Match(value, "^c([a-z])*(\\s)*$").Success)//Company
                    Companyindex = i;
                else if (Regex.Match(value, "^v([a-z])*(\\s)*$").Success)//value
                    Valueindex = i;
                else if (Regex.Match(value, "^m([a-z])*(\\s)*$").Success)//month
                    Monthindex = i;
                else if (Regex.Match(value, "^p([a-z])*(\\s)*$").Success)//price
                    Priceindex = i;
                else if (Regex.Match(value, "^b([a-z])*(\\s)*$").Success)//branch
                    Branchindex = i;
                else
                    return Ok(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.FileNotValid }, HasError = true });

            }

            try
            {
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, Productindex].GetCellValue<string>() == null) { continue; }
                    var record = new ProductInpDto()
                    {
                        Product = worksheet.Cells[row, Productindex].GetCellValue<string>().ToLower(), // Assuming Name is in column A
                        Company = worksheet.Cells[row, Companyindex].GetCellValue<string>().ToLower(),
                        Branch = worksheet.Cells[row, Branchindex].GetCellValue<string>().ToLower(),
                        Price = worksheet.Cells[row, Priceindex].GetCellValue<decimal>(),
                        Quantity = worksheet.Cells[row, Quantityindex].GetCellValue<double>(),       // Assuming Age is in column B
                        Value = worksheet.Cells[row, Valueindex].GetCellValue<double>(),
                        Month = worksheet.Cells[row, Monthindex].GetCellValue<int>()
                    };
                    var company = unitOfWork.CompanyRepo.GetCompanyByName(record.Company);
                    if (company is null)
                    {
                        company = new Company() { Name = record.Company };
                        await unitOfWork.CompanyRepo.AddAsync(company);
                        await unitOfWork.Complete();
                        company = unitOfWork.CompanyRepo.GetCompanyByName(record.Company);
                    }
                    var branch = unitOfWork.BranchRepo.GetBranchByName(record.Branch);
                    if (branch is null)
                    {
                        branch = new Branch() { Name = record.Branch };
                        await unitOfWork.BranchRepo.AddAsync(branch);
                        await unitOfWork.Complete();
                        branch = unitOfWork.BranchRepo.GetBranchByName(record.Branch);
                    }
                    var purchase = new Purchases()
                    {
                        CompanyId = company.Id,
                        BranchId = branch.Id,
                        Month = record.Month,
                        Price = record.Price,
                        Quantity = record.Quantity,
                        Value = record.Value,
                        Product = record.Product
                    };

                    await unitOfWork.PurchasesRepo.AddAsync(purchase);
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
            //if (user == null)
            //    return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            using (var package = new ExcelPackage()) //create a new excel package
            {
                // Add a new worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Purchases");

                ExcelRange range = worksheet.Cells["A1:G200"];
                ExcelTable table = worksheet.Tables.Add(range, "Table");
                worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed30);
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(ExcelIndexedColor.Indexed1);
                worksheet.Cells[range.Address].Style.Font.Size = 14;
                worksheet.Cells[range.Address].Style.Font.Name = "Calibri";
                worksheet.Cells[range.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[range.Address].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[range.Address].AutoFitColumns();



                //Formatting the worksheet
                worksheet.Cells["A1"].Value = "Product";
                worksheet.Cells["B1"].Value = "quantity";
                worksheet.Cells["C1"].Value = "Price";
                worksheet.Cells["D1"].Value = "Value";
                worksheet.Cells["E1"].Value = "Branch"; 
                worksheet.Cells["F1"].Value = "Company";
                worksheet.Cells["G1"].Value = "Month";
                worksheet.Cells["A1:G1"].AutoFitColumns();
                worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                var ranges = worksheet.Cells[2, 4, 200, 4];
                ranges.Formula = string.Format("{0}*{1}", worksheet.Cells[2, 2].Address, worksheet.Cells[2, 3].Address);



                //Save the excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);

                //Return the generated excel file
                
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Purchases.xlsx");

            }
        }


        //charts
        [HttpPost("all-quantity-product")]
        public async Task<ActionResult<List<QuantityProductDto>>> GetAllQuantityProduct(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches , filterDto.Purchases, filterDto.Month);

            List<QuantityProductDto> outProducts = new List<QuantityProductDto>();

            foreach (var Purchase in Purchases)
            {
                if (!outProducts.Any(p => p.Product == Purchase.Product))
                {
                    var product = new QuantityProductDto()
                    {
                        Product = Purchase.Product,

                        Quantity = unitOfWork.PurchasesRepo.GetQuantityOfProduct(Purchase.Product, Purchases)

                    };
                    outProducts.Add(product);
                }

            }

            return Ok(outProducts.OrderByDescending(x => x.Quantity));
        }
        [HttpPost("max-price-product")]
        public async Task<ActionResult<List<PriceProductDto>>> GetMaxPriceProduct(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            List<PriceProductDto> outProducts = new List<PriceProductDto>();

            foreach (var Purchase in Purchases)
            {
                if (!outProducts.Any(p => p.Product == Purchase.Product))
                {
                    var product = new PriceProductDto()
                    {
                        Product = Purchase.Product,

                        Price = unitOfWork.PurchasesRepo.GetMaxPriceOfProduct(Purchase.Product, Purchases)
                    };
                    outProducts.Add(product);
                }
            }
            return Ok(outProducts.OrderByDescending(x => x.Price).ThenBy(x => x.Product));
        }

        [HttpPost("all-value-product")]
        public async Task<ActionResult<List<ValueProductDto>>> GetAllValueProduct(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            List<ValueProductDto> outProducts = new List<ValueProductDto>();

            foreach (var Purchase in Purchases)
            {
                if (!outProducts.Any(p => p.Product == Purchase.Product))
                {
                    var product = new ValueProductDto()
                    {
                        Product = Purchase.Product,

                        Value = unitOfWork.PurchasesRepo.GetValueOfProduct(Purchase.Product, Purchases)
                    };

                    outProducts.Add(product);
                }
            }
            return Ok(outProducts.OrderByDescending(x => x.Value));
        }


        [HttpPost("total-value-company")]
        public async Task<ActionResult<List<ValueCompanyDto>>> GetSumValuesForCompany(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            List<ValueCompanyDto> outProducts = new List<ValueCompanyDto>();

            foreach (var Purchase in Purchases)
            {
                var currCompany = await unitOfWork.CompanyRepo.GetByIdAsync(Purchase.CompanyId);
                if (!outProducts.Any(p => p.Company == currCompany.Name))
                {
                    var company = new ValueCompanyDto()
                    {
                        Company = currCompany.Name,

                        Value = unitOfWork.CompanyRepo.GetValuesOfCompany(currCompany.Id, Purchases)

                    };
                    outProducts.Add(company);
                }

            }
            return Ok(outProducts.OrderByDescending(x => x.Value));
        }


        // crud 
        [HttpPost("sum-value")]
        public async Task<ActionResult<SumValueDto>> GetsumValue(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var value = new SumValueDto(){SumValue=0 };
            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            foreach (var Purchase in Purchases)
            {
                value.SumValue += Purchase.Value;

            }
            return Ok(value);
        }

        [HttpPost("total-quantity")]
        public async Task<ActionResult<TotalQuantityDto>> GetTotalQuantity(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var quantity = new TotalQuantityDto() { TotalQuantity=0};
            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            foreach (var Purchase in Purchases)
            {
                quantity.TotalQuantity += Purchase.Quantity;

            }
            return Ok(quantity);
        }

        [HttpPost("max-price")]
        public async Task<ActionResult<MaxPriceDto>> GetMaxPrice(PurchasesFilterDto filterDto)
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var price = new MaxPriceDto() { MaxPrice = 0 };
            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);

            foreach (var Purchase in Purchases)
            {
                if (Purchase.Price > price.MaxPrice)
                    price.MaxPrice = Purchase.Price;

            }
            return Ok(price);
        }

        // Drop down list

        [HttpGet("products")]
        public async Task<ActionResult<List<string>>> GetProducts()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var products = await unitOfWork.PurchasesRepo.GetAllWithoutZerosAsync();
            List<string> outProducts = new List<string>();

            foreach (var product in products)
            {
                if (!outProducts.Contains(product.Product))
                {
                    outProducts.Add(product.Product);
                }
            }
            return Ok(outProducts.Distinct());
        }

        [HttpGet("months")]
        public async Task<ActionResult<List<int>>> GetMonth()
        {
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.InvalidToken }, HasError = true });

            var purchases = await unitOfWork.PurchasesRepo.GetAllWithoutZerosAsync();
            List<int> outmonthes = new List<int>();

            foreach (var purchase in purchases)
            {
                if (!outmonthes.Contains(purchase.Month))
                {
                    outmonthes.Add(purchase.Month);
                }
            }
            return Ok(outmonthes.Distinct());
        }


        [HttpPost("all-product")]
        public async Task<ActionResult<List<ProductInpDto>>> GetAllProduct(PurchasesFilterDto filterDto)
        {
            List<Purchases> Purchases = new List<Purchases>();
            Purchases = await unitOfWork.PurchasesRepo.GetAllWithFilterAsync(filterDto.Companies, filterDto.Branches, filterDto.Purchases, filterDto.Month);


            List<ProductInpDto> outProducts = new List<ProductInpDto>();

            foreach (var Purchase in Purchases)
            {
                var product = new ProductInpDto()
                {
                    Product = Purchase.Product,
                    Price = Purchase.Price,
                    Quantity = Purchase.Quantity,
                    Value = Purchase.Value,
                    Month = Purchase.Month,
                    Branch = unitOfWork.BranchRepo.GetByIdAsync(Purchase.BranchId).Result.Name,
                    Company = unitOfWork.CompanyRepo.GetByIdAsync(Purchase.CompanyId).Result.Name
                };

                outProducts.Add(product);
            }
            return Ok(outProducts);
        }

        [HttpGet("all-null-product")]
        public async Task<ActionResult<List<ProductInpDto>>> GetAllNullProduct()
        {
            List<Purchases> Purchases = await unitOfWork.PurchasesRepo.GetAllZerosAsync();
            List<ProductInpDto> outProducts = new List<ProductInpDto>();

            foreach (var Purchase in Purchases)
            {
                var product = new ProductInpDto()
                {
                    Product = Purchase.Product,
                    Price = Purchase.Price,
                    Quantity = Purchase.Quantity,
                    Value = Purchase.Value,
                    Month = Purchase.Month,
                    Branch = unitOfWork.BranchRepo.GetByIdAsync(Purchase.BranchId).Result.Name,
                    Company = unitOfWork.CompanyRepo.GetByIdAsync(Purchase.CompanyId).Result.Name
                };

                outProducts.Add(product);
            }
            return Ok(outProducts);
        }

    }
}
