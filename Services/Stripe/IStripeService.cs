namespace DevoteWebsite.Services.Stripe
{
    public interface IStripeService
    {
        string GetCheckoutUrl(List<StripeItem> stripeItems);
    }
}
