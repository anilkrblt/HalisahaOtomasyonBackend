using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

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
            var equipments = await _repository.Equipment.GetEquipmentsByFacilityIdAsync(facilityId, trackChanges);
            return _mapper.Map<IEnumerable<EquipmentDto>>(equipments);
        }

        public async Task<EquipmentDto> GetEquipmentAsync(int equipmentId, bool trackChanges)
        {
            var equipment = await CheckExistsEquipment(equipmentId, trackChanges);

            return _mapper.Map<EquipmentDto>(equipment);
        }

        public async Task<EquipmentDto> CreateEquipmentAsync(EquipmentForCreationDto dto, int facilityId)
        {
            var entity = _mapper.Map<Equipment>(dto);
            entity.FacilityId = facilityId;

            _repository.Equipment.CreateEquipment(entity);
            await _repository.SaveAsync();

            return _mapper.Map<EquipmentDto>(entity);
        }

        public async Task UpdateEquipmentAsync(int equipmentId, EquipmentForUpdateDto equipmentDto, bool trackChanges)
        {
            var equipment = await CheckExistsEquipment(equipmentId, trackChanges);

            _mapper.Map(equipmentDto, equipment);
            await _repository.SaveAsync();
        }

        public async Task DeleteEquipmentAsync(int equipmentId, bool trackChanges)
        {
            var entity = await CheckExistsEquipment(equipmentId, trackChanges);

            _repository.Equipment.DeleteEquipment(entity);
            await _repository.SaveAsync();
        }

        private async Task<Equipment> CheckExistsEquipment(int equipmentId, bool trackChanges)
        {
            var equipment = await _repository.Equipment.GetEquipmentByIdAsync(equipmentId, trackChanges);
            if (equipment is null)
                throw new EquipmentNotFoundException(equipmentId);

            return equipment;
        }
    }
}
