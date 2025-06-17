using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ReservationsController(IServiceManager service)
        {
            _service = service;
        }

        // Tüm rezervasyonları getir
        //günlük, aylık, haftalık, facility
        // GET /api/reservations?fieldId=1&facilityId=2&date=2025-06-11&period=day
        [HttpGet]
        public async Task<IActionResult> GetAllReservations(
            [FromQuery] int? fieldId,
            [FromQuery] int? facilityId,
            [FromQuery] DateTime? date,
            [FromQuery] string period = "all"
        )
        {
            var reservations = await _service.ReservationService.GetAllReservationsAsync();

            if (fieldId.HasValue)
                reservations = reservations.Where(r => r.FieldId == fieldId.Value);

            if (facilityId.HasValue)
            {
                var fieldDtos = await _service.FieldService.GetFieldsByFacilityIdAsync(facilityId.Value, false);
                var fieldIds = fieldDtos.Select(f => f.Id).ToList();

                reservations = reservations.Where(r => fieldIds.Contains(r.FieldId));

            }

            if (date.HasValue)
            {
                switch (period.ToLower())
                {
                    case "day":
                        reservations = reservations.Where(r => r.SlotStart.Date == date.Value.Date);
                        break;
                    case "week":
                        var dateVal = date.Value.Date;
                        reservations = reservations.Where(r =>
                        {
                            var diff = (7 + (r.SlotStart.DayOfWeek - DayOfWeek.Monday)) % 7;
                            var weekStart = dateVal.AddDays(-diff);
                            var weekEnd = weekStart.AddDays(7);
                            return r.SlotStart.Date >= weekStart && r.SlotStart.Date < weekEnd;
                        });
                        break;
                    case "month":
                        reservations = reservations.Where(r =>
                            r.SlotStart.Year == date.Value.Year && r.SlotStart.Month == date.Value.Month);
                        break;
                }
            }

            return Ok(reservations.ToList());
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
