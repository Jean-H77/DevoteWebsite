namespace DevoteWebsite.Services.Stripe
{
    public class StripeItem
    {

        public required int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Name { get; set; }
    }
}
