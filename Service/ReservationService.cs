using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class ReservationService : IReservationService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _map;

        public ReservationService(IRepositoryManager repo, IMapper map)
        {
            _repo = repo;
            _map = map;
        }

        public async Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto dto)
        {
            var entity = _map.Map<Reservation>(dto);

            var field = await _repo.Field.GetFieldAsync(dto.FieldId, false)
                ?? throw new FieldNotFoundException(dto.FieldId);

            entity.CreatedAt = DateTime.UtcNow;

            var duration = (dto.SlotEnd - dto.SlotStart).TotalHours;
            entity.PriceTotal = field.PricePerHour * (decimal)duration;

            _repo.Reservation.CreateReservation(entity);
            await _repo.SaveAsync();


            // 🔔 Bildirim ekle
            if (field.Facility != null && field.Facility.OwnerId != null)
            {
                _repo.Notification.CreateNotification(new Notification
                {
                    UserId = field.Facility.OwnerId,
                    Title = "Yeni Rezervasyon",
                    Description = $"Sahanıza {dto.SlotStart:yyyy-MM-dd HH:mm} için yeni bir rezervasyon yapıldı.",
                    EntityId = entity.Id,
                    EntityType = "Reservation",
                    Type = NotificationType.Reservation,
                    CreatedAt = DateTime.UtcNow
                });

                await _repo.SaveAsync();
            }



            return _map.Map<ReservationDto>(entity);
        }


        public async Task<ReservationPaymentDto> CreateReservationPaymentAsync(ReservationPaymentForCreationDto dto)
        {
            var entity = _map.Map<ReservationPayment>(dto);
            entity.Status = PaymentStatus.Pending;

            _repo.ReservationPayment.CreateReservationPayment(entity);
            await _repo.SaveAsync();

            return _map.Map<ReservationPaymentDto>(entity);
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var entity = await _repo.Reservation.GetReservationAsync(reservationId, trackChanges: true)
                         ?? throw new Exception("Reservation not found");

            _repo.Reservation.DeleteReservation(entity);
            await _repo.SaveAsync();
        }

        public async Task DeleteReservationPaymentAsync(int id)
        {
            var entity = await _repo.ReservationPayment.GetReservationPaymentAsync(id, trackChanges: true)
                         ?? throw new Exception("Reservation payment not found");

            _repo.ReservationPayment.DeleteReservationPayment(entity);
            await _repo.SaveAsync();
        }

        public async Task<IEnumerable<ReservationPaymentDto>> GetAllReservationPaymentsAsync(bool trackChanges = false)
        {
            var payments = await _repo.ReservationPayment.GetAllReservationPaymentsAsync(trackChanges);
            return _map.Map<IEnumerable<ReservationPaymentDto>>(payments);
        }

        public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool trackChanges = false)
        {
            var reservations = await _repo.Reservation.GetAllReservationsAsync(trackChanges);
            return _map.Map<IEnumerable<ReservationDto>>(reservations);
        }

        public async Task<IEnumerable<ReservationDto>> GetOverlappingReservationsByFieldAsync(int fieldId, DateTime slotStart)
        {
            var overlaps = await _repo.Reservation.GetByFieldAsync(fieldId, slotStart);
            return _map.Map<IEnumerable<ReservationDto>>(overlaps);
        }

        public async Task<ReservationDto> GetReservationByIdAsync(int reservationId, bool trackChanges = false)
        {
            var reservation = await _repo.Reservation.GetReservationAsync(reservationId, trackChanges)
                              ?? throw new Exception("Reservation not found");

            return _map.Map<ReservationDto>(reservation);
        }

        public async Task<ReservationPaymentDto> GetReservationPaymentByIdAsync(int id, bool trackChanges = false)
        {
            var payment = await _repo.ReservationPayment.GetReservationPaymentAsync(id, trackChanges)
                          ?? throw new Exception("Reservation payment not found");

            return _map.Map<ReservationPaymentDto>(payment);
        }
    }

}