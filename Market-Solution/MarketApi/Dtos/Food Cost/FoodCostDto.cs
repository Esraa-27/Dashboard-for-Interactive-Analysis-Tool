using MarketCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketApi.Dtos.Food_Cost
{
    public class FoodCostDto
    {

        public decimal PurchasingVisa { get; set; }

        public decimal PurchasingCash { get; set; }

        public decimal CloseInventory { get; set; }

        public decimal OpenInventory { get; set; }

        public decimal Consumption { get; set; }

        public int Month { get; set; }

        public decimal Sales { get; set; }
        public decimal Cost { get; set; }


        public string Branch { get; set; }

        public string Category { get; set; }

    }
}
