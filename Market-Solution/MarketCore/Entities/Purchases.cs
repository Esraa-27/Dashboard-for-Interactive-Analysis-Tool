using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Entities
{
    public class Purchases : BaseEntity
    {
        public string Product { get; set; }
        public double Quantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
        public double Value { get; set; }
        public int Month { get; set; }


        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }


        [ForeignKey("Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
