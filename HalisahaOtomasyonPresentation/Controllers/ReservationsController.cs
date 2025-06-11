using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ReservationsController(IServiceManager service)
        {
            _service = service;
        }

        // Tüm rezervasyonları getir
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _service.ReservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        // Tek rezervasyon getir
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _service.ReservationService.GetReservationByIdAsync(id);
            return Ok(reservation);
        }

        // Yeni rezervasyon oluştur
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationForCreationDto dto)
        {
            var created = await _service.ReservationService.CreateReservationAsync(dto);
            return CreatedAtAction(nameof(GetReservationById), new { id = created.Id }, created);
        }

        // Rezervasyon sil
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _service.ReservationService.DeleteReservationAsync(id);
            return NoContent();
        }

        // Belirli sahada çakışan rezervasyonları getir
        [HttpGet("overlaps")]
        public async Task<IActionResult> GetOverlappingReservations([FromQuery] int fieldId, [FromQuery] DateTime slotStart)
        {
            var overlaps = await _service.ReservationService.GetOverlappingReservationsByFieldAsync(fieldId, slotStart);
            return Ok(overlaps);
        }

        // Tüm ödeme kayıtlarını getir
        [HttpGet("payments")]
        public async Task<IActionResult> GetAllReservationPayments()
        {
            var payments = await _service.ReservationService.GetAllReservationPaymentsAsync();
            return Ok(payments);
        }

        // Tek ödeme kaydı getir
        [HttpGet("payments/{id:int}")]
        public async Task<IActionResult> GetReservationPaymentById(int id)
        {
            var payment = await _service.ReservationService.GetReservationPaymentByIdAsync(id);
            return Ok(payment);
        }

        // Yeni ödeme kaydı oluştur
        [HttpPost("payments")]
        public async Task<IActionResult> CreateReservationPayment([FromBody] ReservationPaymentForCreationDto dto)
        {
            var created = await _service.ReservationService.CreateReservationPaymentAsync(dto);
            return CreatedAtAction(nameof(GetReservationPaymentById), new { id = created.Id }, created);
        }

        // Ödeme kaydı sil
        [HttpDelete("payments/{id:int}")]
        public async Task<IActionResult> DeleteReservationPayment(int id)
        {
            await _service.ReservationService.DeleteReservationPaymentAsync(id);
            return NoContent();
        }
    }
}
