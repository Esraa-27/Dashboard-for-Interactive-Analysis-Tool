using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCore.Entities
{
    public class FoodCost:BaseEntity
    {

        [Column(TypeName = "decimal(18,4)")]
        public decimal PurchasingVisa { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PurchasingCash { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CloseInventory { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal OpenInventory { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Consumption { get; set; }

        public int Month { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Sales { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }


        [ForeignKey("Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
