using MarketCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketApi.Dtos.Sales
{
    public class SalesInpDto
    {
        public string Product { get; set; }
        public double Quantity { get; set; }
        public decimal SalesValue { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Average { get; set; }
        public int Month { get; set; }


        public string Branch { get; set; }

        public string Category { get; set; }
    }
}
