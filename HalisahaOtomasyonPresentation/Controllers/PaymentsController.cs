using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using Shared.DataTransferObjects;
using Stripe;
using Stripe.Checkout;

namespace HalisahaOtomasyonPresentation.Controllers
{
    // ödeme ihtiyaçlarına göre yeni endpointler ekle
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        // POST api/rooms/42/payment-session?teamId=5
        [HttpPost("{id:int}/payment-session")]
        public IActionResult CreatePaymentSession(int id, [FromQuery] int teamId, [FromBody] PaymentRequestDto dto)
        {
            // Room, Team vs. kontrolünü yapabilirsin burada
            var paymentService = new PaymentService();
            var url = paymentService.CreateCheckoutSession(
                dto.Amount, // Veya oda üzerinden fiyatı çekebilirsin
                $"https://seninfrontend.com/odeme-basarili?roomId={id}&teamId={teamId}",
                $"https://seninfrontend.com/odeme-iptal?roomId={id}&teamId={teamId}",
                dto.Email
            );
            return Ok(new { url });
        }


        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            // Bunu Stripe panelinden bulabilirsin
            var webhookSecret = Environment.GetEnvironmentVariable("WEB-HOOK-SECRET");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    // == Burada kendi uygulamana özel işlemleri yap! ==
                    // - session.Id
                    // - session.CustomerEmail
                    // - session.AmountTotal, session.ClientReferenceId vs.
                    // Senin PaymentStatus'unu güncelle!
                }
                // Diğer event tiplerine de bakabilirsin (örn: payment_intent.succeeded)
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }

            return Ok();
        }
    }
}
