﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class FieldService : IFieldService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photo;

        public FieldService(
            IRepositoryManager repositoryManager,
            ILoggerManager loggerManager,
            IMapper mapper,
            IPhotoService photoService)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _photo = photoService;
        }

        public async Task<IEnumerable<FieldDto>> GetAllFieldsAsync(bool trackChanges)
        {
            var fields = await _repositoryManager.Field.GetAllFieldsAsync(trackChanges);
            return _mapper.Map<IEnumerable<FieldDto>>(fields);
        }

        public async Task<FieldDto> GetFieldAsync(int fieldId, bool trackChanges)
        {
            var entity = await _repositoryManager.Field.GetFieldAsync(fieldId, trackChanges);
            if (entity is null)
                throw new FieldNotFoundException(fieldId);

            return _mapper.Map<FieldDto>(entity);
        }

        public async Task<IEnumerable<FieldDto>> GetFieldsByFacilityIdAsync(int facilityId, bool trackChanges)
        {
            var facility = await _repositoryManager.Facility.GetFacilityAsync(facilityId, trackChanges);
            if (facility is null)
                throw new FacilityNotFoundException(facilityId);

            var fields = await _repositoryManager.Field.GetFieldsByFacilityIdAsync(facilityId, trackChanges);
            return _mapper.Map<IEnumerable<FieldDto>>(fields);
        }

        public async Task<FieldDto> CreateFieldAsync(FieldForCreationDto dto)
        {
            var entity = _mapper.Map<Field>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            _repositoryManager.Field.CreateField(entity);
            await _repositoryManager.SaveAsync();

            var existingExceptions = await _repositoryManager.FieldException
                                                 .GetExceptionsByFieldIdAsync(entity.Id, true);

            if (existingExceptions.Any())
            {
                _repositoryManager.FieldException.DeleteFieldExceptions(existingExceptions);
                await _repositoryManager.SaveAsync();
            }

            if (dto.Exceptions is not null)
            {
                foreach (var ex in dto.Exceptions.DistinctBy(x => x.Date.Date))
                {
                    _repositoryManager.FieldException.CreateFieldException(
                        new FieldException
                        {
                            FieldId = entity.Id,
                            Date = ex.Date.Date,
                            IsOpen = ex.IsOpen
                        });
                }

                await _repositoryManager.SaveAsync();
            }

            var full = await _repositoryManager.Field.GetFieldAsync(entity.Id, trackChanges: false);
            return _mapper.Map<FieldDto>(full);
        }



        public async Task UpdateFieldAsync(int fieldId, FieldForUpdateDto dto, bool trackChanges)
        {
            var entity = await _repositoryManager.Field.GetFieldAsync(fieldId, true)
              ?? throw new FieldNotFoundException(fieldId);

            _mapper.Map(dto, entity);
            await _repositoryManager.SaveAsync();

            /* FIELD EXCEPTIONS - İki aşamalı */
            var existingExceptions = await _repositoryManager.FieldException
                                       .GetExceptionsByFieldIdAsync(entity.Id, true);

            if (existingExceptions.Any())
            {
                _repositoryManager.FieldException.DeleteFieldExceptions(existingExceptions);
                await _repositoryManager.SaveAsync();
            }

            // Sonra ekle
            foreach (var ex in dto.Exceptions.DistinctBy(x => x.Date.Date))
            {
                _repositoryManager.FieldException.CreateFieldException(
                    new FieldException
                    {
                        FieldId = entity.Id,
                        Date = ex.Date.Date,
                        IsOpen = ex.IsOpen
                    });
            }

            await _repositoryManager.SaveAsync();
        }


        public async Task PatchFieldAsync(int id, FieldPatchDto patch)
        {
            var field = await _repositoryManager.Field.GetFieldAsync(id, true)
                        ?? throw new FieldNotFoundException(id);

            if (patch.Name is not null) field.Name = patch.Name;
            if (patch.Width is not null) field.Width = patch.Width.Value;
            if (patch.Height is not null) field.Height = patch.Height.Value;
            if (patch.IsIndoor is not null) field.IsIndoor = patch.IsIndoor.Value;
            if (patch.IsAvailable is not null) field.IsAvailable = patch.IsAvailable.Value;
            if (patch.HasCamera is not null) field.HasCamera = patch.HasCamera.Value;
            if (patch.HasTribune is not null) field.HasTribune = patch.HasTribune.Value;
            if (patch.HasScoreBoard is not null) field.HasScoreBoard = patch.HasScoreBoard.Value;
            if (patch.FloorType is not null) field.FloorType = (Entities.Models.FloorType)patch.FloorType.Value;
            if (patch.Capacity is not null) field.Capacity = patch.Capacity.Value;
            if (patch.PricePerHour is not null) field.PricePerHour = patch.PricePerHour.Value;
            if (patch.LightingAvailable is not null) field.LightingAvailable = patch.LightingAvailable.Value;
            if (patch.CreatedAt is not null) field.CreatedAt = patch.CreatedAt.Value;

            // --- WEEKLY OPENINGS PATCH ---
            if (patch.WeeklyOpenings?.Any() == true)
            {
                // 1. Aynı güne birden fazla var mı? Hata fırlat!
                var distinctOpenings = patch.WeeklyOpenings
                    .GroupBy(w => w.DayOfWeek)
                    .Select(g => g.First())
                    .ToList();

                if (distinctOpenings.Count != patch.WeeklyOpenings.Count)
                    throw new Exception("Aynı gün için birden fazla çalışma saati gönderilemez!");

                // 2. Var olan tüm kayıtları sil
                var existingOpenings = await _repositoryManager.WeeklyOpening
                    .GetWeeklyOpeningsByFieldIdAsync(id, true);
                foreach (var old in existingOpenings)
                    _repositoryManager.WeeklyOpening.DeleteWeeklyOpening(old);

                // 3. Sadece tekil olanları ekle
                foreach (var w in distinctOpenings)
                {
                    _repositoryManager.WeeklyOpening.CreateWeeklyOpening(new WeeklyOpening
                    {
                        FieldId = id,
                        DayOfWeek = w.DayOfWeek,
                        StartTime = w.StartTime,
                        EndTime = w.EndTime
                    });
                }
            }

            // --- EXCEPTIONS PATCH ---
            if (patch.Exceptions?.Any() == true)
            {
                var distinctExceptions = patch.Exceptions
                    .GroupBy(e => e.Date.Date)
                    .Select(g => g.First())
                    .ToList();

                if (distinctExceptions.Count != patch.Exceptions.Count)
                    throw new Exception("Aynı gün için birden fazla exception gönderilemez!");

                var existingExceptions = await _repositoryManager.FieldException
                    .GetExceptionsByFieldIdAsync(id, true);

                foreach (var oldEx in existingExceptions)
                    _repositoryManager.FieldException.DeleteFieldException(oldEx);

                foreach (var ex in distinctExceptions)
                {
                    _repositoryManager.FieldException.CreateFieldException(new FieldException
                    {
                        FieldId = id,
                        Date = ex.Date.Date,
                        IsOpen = ex.IsOpen
                    });
                }
            }

            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteFieldAsync(int fieldId)
        {
            var entity = await _repositoryManager.Field.GetFieldAsync(fieldId, true)
                          ?? throw new FieldNotFoundException(fieldId);

            _repositoryManager.Field.DeleteField(entity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<bool> IsFieldOpenAsync(int fieldId, DateTime dateTime)
        {
            // 1) Özel tarihli istisna kontrolü
            var exception = await _repositoryManager.FieldException
                .GetExceptionByDateAsync(fieldId, dateTime.Date, trackChanges: false);
            if (exception is not null)
                return exception.IsOpen;

            // 2) Haftalık rutin programa bakış
            var weeklyOpenings = await _repositoryManager.WeeklyOpening
                .GetWeeklyOpeningsByFieldIdAsync(fieldId, trackChanges: false);

            var todayOpening = weeklyOpenings
                .FirstOrDefault(w => w.DayOfWeek == dateTime.DayOfWeek);
            if (todayOpening is null)
                return false;

            var currentTime = dateTime.TimeOfDay;
            return currentTime >= todayOpening.StartTime
                && currentTime <= todayOpening.EndTime;
        }

    }
}
