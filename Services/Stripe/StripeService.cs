using Stripe.Checkout;
namespace DevoteWebsite.Services.Stripe
{
    public class StripeService : IStripeService
    {
        public string GetCheckoutUrl(List<StripeItem> stripeItems)
        {
            if (stripeItems == null || stripeItems.Count == 0)
            {
                throw new ArgumentException("At least one item is required for checkout", nameof(stripeItems));
            }

            var session = CreateAndGetStripeSession(stripeItems);
            return session.Url;
        }

        private Session CreateAndGetStripeSession(List<StripeItem> stripeItems)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = "https://google.com", 
                CancelUrl = "https://google.net", 
                Mode = "payment",
                CustomerEmail = "temp@temp.com",     
                LineItems = new List<SessionLineItemOptions>()
            };

            foreach (var item in stripeItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Name)
                    || item.TotalPrice <= 0 || item.Quantity <= 0)
                {
                    continue; 
                }

                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (decimal)item.TotalPrice * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionListItem);
            }

            // Final validation to ensure we have line items
            if (options.LineItems.Count == 0)
            {
                throw new InvalidOperationException("No valid line items were provided for checkout");
            }

            var service = new SessionService();
            return service.Create(options);
        }
    }
}