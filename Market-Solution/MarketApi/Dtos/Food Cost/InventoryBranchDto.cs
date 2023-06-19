namespace MarketApi.Dtos.Food_Cost
{
    public class InventoryBranchDto
    {
        public string Branch { get; set; }
        public decimal OpenInventory { get; set; }
        public decimal CloseInventory { get; set; }
    }
}
