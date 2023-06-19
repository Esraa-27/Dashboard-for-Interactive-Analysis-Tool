using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Entities
{
    public class Sales:BaseEntity
    {
        public string Product { get; set; }
        public double Quantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SalesValue { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Vat { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalSales { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Average { get; set; }
        public int Month { get; set; }


        [ForeignKey("Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
