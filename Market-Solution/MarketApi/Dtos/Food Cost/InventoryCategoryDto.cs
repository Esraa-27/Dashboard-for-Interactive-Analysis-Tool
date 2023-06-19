namespace MarketApi.Dtos.Food_Cost
{
    public class InventoryCategoryDto
    {
        public string Category { get; set; }
        public decimal OpenInventory { get; set; }
        public decimal CloseInventory { get; set; }
    }
}
