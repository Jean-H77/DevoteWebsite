namespace DevoteWebsite.ViewModels
{
    public class StoreItemViewModel
    {
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public int SalePercentage { get; set; }

        public required string Thumbnail { get; set; }

        public Guid Uid { get; set; }

        public required string Description { get; set; }

        public decimal? DiscountedPrice { get; set; }
    }
}
