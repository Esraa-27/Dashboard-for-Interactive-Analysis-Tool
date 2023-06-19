using MarketCore.Entities;

namespace MarketApi.Dtos.Purchases
{
    public class ProductInpDto
    {
        public string Product { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public double Value { get; set; }
        public int Month { get; set; }

        public string Company { get; set; }
        public string Branch { get; set; }
    }
}
