using System.IO;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Contracts;
using Shared.DataTransferObjects;
using Stripe;
using Stripe.Checkout;

namespace HalisahaOtomasyonPresentation.Controllers
{
    //TODO Bura eksik
    // ödeme ihtiyaçlarına göre yeni endpointler ekle
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IRepositoryManager _repo;
        private readonly IServiceManager _svc;

        public PaymentsController(IServiceManager svc, IRepositoryManager repo)
        {
            _svc = svc;
            _repo = repo;
        }


        [HttpPost("{roomId:int}/payment-session/user/{userId:int}")]
        public IActionResult CreatePaymentSessionForUser(int roomId, int userId, [FromBody] PaymentRequestDto dto)
        {
            var paymentService = new PaymentService();

            var url = paymentService.CreateCheckoutSession(
                dto.Amount,
                $"https://seninfrontend.com/odeme-basarili?roomId={roomId}&userId={userId}",
                $"https://seninfrontend.com/odeme-iptal?roomId={roomId}&userId={userId}",
                dto.Email,
                roomId,
                userId
            );

            return Ok(new { url });
        }


        [HttpGet("{id:int}/payment-status")]
        public async Task<IActionResult> GetPaymentStatus(int id)
        {
            var result = await _svc.RoomService.GetPaymentStatusAsync(id);
            return Ok(result);
        }

        [HttpPost("{roomId:int}/refund/user/{userId:int}")]
        public async Task<IActionResult> RefundUser(int roomId, int userId)
        {
            // DB’den kullanıcıya ait chargeId ve amount’u çek
            var paymentInfo = await _svc.RoomService.GetPaymentInfo(roomId, userId);
            if (paymentInfo is null)
                return NotFound("Ödeme bulunamadı");

            var paymentService = new PaymentService();
            var refund = paymentService.CreateRefund(paymentInfo.Value.ChargeId, paymentInfo.Value.Amount);

            return Ok(refund);
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
                    int roomId = int.Parse(session.Metadata["roomId"]);
                    int userId = int.Parse(session.Metadata["userId"]);

                    // Room’dan kişi sayısını bul → kişi başı fiyatı hesapla
                    var room = await _repo.Room.GetOneRoomAsync(roomId, false); // 🔧 DTO değil, entity alındı
                    int participantCount = room.Participants.Sum(p => p.Team.Members.Count);
                    decimal pricePerPlayer = room.PricePerPlayer;

                    decimal amount = (decimal)(session.AmountTotal ?? 0) / 100;

                    // Güvenlik: Stripe'dan gelen amount ile sistemin hesapladığı tutar eşleşiyor mu?
                    if (amount == pricePerPlayer)
                    {
                        await _svc.RoomService.PayPlayerAsync(roomId, userId, amount);
                    }
                    else
                    {
                        // log at ya da alert gönder
                        throw new InvalidOperationException("Ödeme tutarı uyuşmuyor");
                    }
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
