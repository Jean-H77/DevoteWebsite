namespace DevoteWebsite.ViewModels
{
    public class StoreItemViewModel
    {
        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int SalePercentage { get; set; }

        public Guid Uid { get; set; }

        public string? Description { get; set; }

        public decimal? DiscountedPrice { get; set; }
    }
}
