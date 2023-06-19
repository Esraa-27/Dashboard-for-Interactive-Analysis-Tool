using System.Collections.Generic;

namespace MarketApi.Dtos.Purchases
{
    public class PurchasesFilterDto
    {
        public int Companies { set; get; }
        public int Branches { set; get; }
        public string Purchases { set; get; }
        public int Month { set; get; }
    }
}
