namespace MarketApi.Dtos.Sales
{
    public class SalesFilterDto
    {
        public int Category { set; get; }
        public int Branch { set; get; }
        public string Product { set; get; }
        public int Month { set; get; }
    }
}
