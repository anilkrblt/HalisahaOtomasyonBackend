using Stripe;
using Stripe.Checkout;

namespace Service
{
    public class PaymentService
    {
        public PaymentService()
        {
            var stripeKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY");

            Stripe.StripeConfiguration.ApiKey = stripeKey;

        }
        public Refund CreateRefund(string chargeId, decimal amount)
        {
            var options = new RefundCreateOptions
            {
                Charge = chargeId,
                Amount = (long)(amount * 100), // kuruş cinsinden
            };

            var service = new RefundService();
            return service.Create(options);
        }
        public string CreateCheckoutSession(decimal amount, string successUrl, string cancelUrl, string customerEmail, int roomId, int userId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),
                    Currency = "try",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Maç Ücreti",
                    },
                },
                Quantity = 1,
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>  // 👈 burası eklendi
        {
            { "roomId", roomId.ToString() },
            { "userId", userId.ToString() }
        }
            };

            if (!string.IsNullOrEmpty(customerEmail))
            {
                options.CustomerEmail = customerEmail;
            }

            var service = new SessionService();
            Session session = service.Create(options);

            return session.Url;
        }
    }
}
