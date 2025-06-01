using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public EquipmentService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllEquipmentsByFacilityAsync(int facilityId, bool trackChanges)
        {
            var equipments = await _repository.Equipment.GetAllByFacilityIdAsync(facilityId, trackChanges);
            return _mapper.Map<IEnumerable<EquipmentDto>>(equipments);
        }

        public async Task<EquipmentDto> GetEquipmentAsync(int equipmentId, bool trackChanges)
        {
            var equipment = await _repository.Equipment.GetEquipmentByIdAsync(equipmentId, trackChanges);
            if (equipment is null)
                throw new Exception($"Equipment with id {equipmentId} not found."); // Exception'ı özelleştirebilirsin

            return _mapper.Map<EquipmentDto>(equipment);
        }



        public async Task UpdateEquipmentAsync(int equipmentId, EquipmentForUpdateDto equipmentDto, bool trackChanges)
        {
            var equipment = await _repository.Equipment.GetEquipmentByIdAsync(equipmentId, trackChanges);
            if (equipment is null)
                throw new Exception($"Equipment with id {equipmentId} not found.");

            _mapper.Map(equipmentDto, equipment);
            await _repository.SaveAsync();
        }
        // Service/EquipmentService.cs  ─ yalnızca **iki** satır değişti
        public async Task<EquipmentDto> CreateEquipmentAsync(EquipmentForCreationDto dto, int facilityId)
        {
            var entity = _mapper.Map<Equipment>(dto);
            entity.FacilityId = facilityId;

            _repository.Equipment.CreateEquipment(entity);   // <── güncellendi
            await _repository.SaveAsync();

            return _mapper.Map<EquipmentDto>(entity);
        }

        public async Task DeleteEquipmentAsync(int equipmentId, bool trackChanges)
        {
            var entity = await _repository.Equipment.GetEquipmentByIdAsync(equipmentId, trackChanges)
                         ?? throw new Exception($"Equipment {equipmentId} not found.");

            _repository.Equipment.DeleteEquipment(entity);   // <── güncellendi
            await _repository.SaveAsync();
        }

    }
}
